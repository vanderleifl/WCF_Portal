using System;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.ServiceModel.Activation;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "CreditoCliente" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione CreditoCliente.svc ou CreditoCliente.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CreditoCliente : ICreditoCliente
    {
        public CREDITO Obter(long cnpj)
        {
            CREDITO credito = null;
            string log = "Inicio";

            try
            {

                Conexao con = new Conexao();

                log += " Conexão aberta: " + con.ConOra.ConnectionString;

                Func func = new Func();
                double mlimcre = 0, mvaldisp = 0, mvdamax = 0, mcprmes = 0, mtotAb = 0, mvencido = 0, mvencer = 0, mchdevAb = 0, mcreDev = 0, mchPre = 0;
                string sql = "";

                string codcli = "";

                sql = "select CCODCLI, CLCREC, CVDAMEN"
                    + " from CADCLI"
                    + " where CCGC = " + cnpj.ToString("00000000000000")
                    + "    or CCPF = " + cnpj.ToString("00000000000000");
                DataSet ds1 = con.FazerLeituraDataSet(sql);

                log += " Abertura do banco de dados";

                if (ds1.Tables.Count > 0)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        log += " Cliente localizado";

                        codcli = ds1.Tables[0].Rows[0]["CCODCLI"].ToString();
                        mlimcre = func.ParaDouble(ds1.Tables[0].Rows[0]["CLCREC"]);
                        mvdamax = func.ParaDouble(ds1.Tables[0].Rows[0]["CVDAMEN"]);
                    }
                }

log += " Ponto de verificação 0001";


                sql = "select max(PA2_VALOR2) from CADPAR2 where PA2_CAMPO='BCSCND'";
                int mdiascred = func.ParaInteiro(con.Resultado(sql));

                sql = "select max(DATA) from FDIA";
                DateTime mdiatrab = func.ParaData(con.Resultado(sql));
                if (mdiatrab <= DateTime.MinValue)
                {
                    mdiatrab = DateTime.Now;
                }

                sql = "select sum((Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)"
                    + "             -((Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)*D2DES1/100)"
                    + "             -(((Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)"
                    + "             -(Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)*D2DES1/100)*decode(D1D2A,1,0,D2DES2)/100)"
                    + "             -(((Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)"
                    + "             -((Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)*D2DES1/100)"
                    + "             -(((Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)"
                    + "             -(Round(D2PRECO*D2QTD,2)-Round(D2PRECO*D2QTD,2)*D2DESC/100)*D2DES1/100)*decode(D1D2A,1,0,D2DES2)/100))*D1DES1/100)"
                    + "             ) TOTLIQ"
                    + "  from PEDDIA1, PEDDIA2, CADVEN"
                    + "  where D2NUMERO=D1CHAVE"
                    + "    and CFCODVEN=D1CODVEN"
                    + "    and D1CODCLI = 0" + codcli;
                double mvalpedpen = func.ParaDouble(con.Resultado(sql));

log += " Ponto de verificação 0002";

                if (mvalpedpen < 0)
                {
                    mvalpedpen = 0;
                }
                mvaldisp = mlimcre - mvalpedpen;

log += " Ponto de verificação 1";

                DateTime md1 = mdiatrab;
                md1 = new DateTime(md1.Year, md1.Month, 1);
                DateTime md2 = md1.AddMonths(1);
                md2.AddDays(-1);
                OracleParameter[] par = new OracleParameter[2];
                par[0] = new OracleParameter("MDT1", md1);
                par[1] = new OracleParameter("MDT2", md2);

                // Compra Maxima Mensal
                if (mvdamax > 0)
                {
log += " Ponto de verificação 2";

                    sql += " and D1DTEMIS >= :MDT1"
                         + " and D1DTEMIS <= :MDT2";
                    mcprmes = func.ParaDouble(con.Resultado(sql, par));

log += " Ponto de verificação 3";

                    sql = "select sum(P1TOTNOT) from PED01"
                        + " where P1VDORD in ('N','V')"
                        + "   and P1CODCLI=0" + codcli
                        + "   and P1DTEMIS >= :MDT1"
                        + "   and P1DTEMIS <= :MDT2";
                    mcprmes += func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 3";

                    sql = "select sum(P1TOTNOT) from PED51"
                        + " where P1VDORD in ('N','V')"
                        + "   and P1CODCLI=0" + codcli
                        + "   and P1DTEMIS >= :MDT1"
                        + "   and P1DTEMIS <= :MDT2";
                    mcprmes += func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 4";
                }

                // Data da última compra
log += " Ponto de verificação 5";
                sql = "select max(P1DTEMIS) from PED01"
                    + " where P1VDORD in ('N','V')"
                    + "   and P1CODCLI=0" + codcli;
                DateTime mdtuc = func.ParaData(con.Resultado(sql));
log += " Ponto de verificação 6";

                sql = "select max(P1DTEMIS) from PED51"
                    + " where P1VDORD in ('N','V')"
                    + "   and P1CODCLI=0" + codcli;
                DateTime mdtuc2 = func.ParaData(con.Resultado(sql));
log += " Ponto de verificação 7";

                if (mdtuc2 > mdtuc)
                {
                    mdtuc = mdtuc2;
                }

                // Data da primeira compra
log += " Ponto de verificação 8";
                sql = "select Min(P1DTEMIS) from PED01"
                    + " where P1VDORD in ('N','V')"
                    + "   and P1CODCLI=0" + codcli;
                DateTime mdtpc = func.ParaData(con.Resultado(sql));
log += " Ponto de verificação 9";

                sql = "select Min(P1DTEMIS) from PED51"
                    + " where P1VDORD in ('N','V')"
                    + "   and P1CODCLI=0" + codcli;
                DateTime mdtpc2 = func.ParaData(con.Resultado(sql));
log += " Ponto de verificação 10";

                if (mdtpc2 > DateTime.MinValue)
                {
                    if (mdtpc <= DateTime.MinValue || mdtpc2 < mdtpc)
                    {
                        mdtpc = mdtpc2;
                    }
                }

log += " Ponto de verificação 11";

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

log += " Ponto de verificação 12";
                //Duplicatas vencidas
                sql = "select sum(CRVALOR-decode(CRPAGO,null,0,CRPAGO)-decode(CRDEVOL,null,0,CRDEVOL)) from CADDAR"
                    + " where CRCODCLI = 0" + codcli
                    + "   and CRDUP <> 1"
                    + "   and CRSTATUS < 2"
                    + "   and CRDTVCTO < :MDT1";
                mvencido = func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 13";

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

                sql = "select sum(CRVALOR-decode(CRPAGO,null,0,CRPAGO)-decode(CRDEVOL,null,0,CRDEVOL)) from CADDAR50"
                    + " where CRCODCLI = 0" + codcli
                    + "   and CRDUP <> 1"
                    + "   and CRSTATUS < 2"
                    + "   and CRDTVCTO < :MDT1";
                mvencido += func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 14";

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

                //Duplicatas a vencer
                sql = "select sum(CRVALOR-decode(CRPAGO,null,0,CRPAGO)-decode(CRDEVOL,null,0,CRDEVOL)) from CADDAR"
                    + " where CRCODCLI = 0" + codcli
                    + "   and CRDUP <> 1"
                    + "   and CRSTATUS < 2"
                    + "   and CRDTVCTO >= :MDT1";
                mvencer = func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 15";

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

                sql = "select sum(CRVALOR-decode(CRPAGO,null,0,CRPAGO)-decode(CRDEVOL,null,0,CRDEVOL)) from CADDAR50"
                    + " where CRCODCLI = 0" + codcli
                    + "   and CRDUP <> 1"
                    + "   and CRSTATUS < 2"
                    + "   and CRDTVCTO >= :MDT1";
                mvencer += func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 16";

                mtotAb = mvencido + mvencer;

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

                //Cheques devolvidos em aberto
                sql = "select sum(CRVALOR-decode(CRPAGO,null,0,CRPAGO)-decode(CRDEVOL,null,0,CRDEVOL)) from CADDAR"
                    + " where CRCODCLI = 0" + codcli
                    + "   and CRDUP = 1"
                    + "   and CRSTATUS < 2"
                    + "   and CRDTVCTO < :MDT1";
                mchdevAb = func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 17";

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

                sql = "select sum(CRVALOR-decode(CRPAGO,null,0,CRPAGO)-decode(CRDEVOL,null,0,CRDEVOL)) from CADDAR50"
                    + " where CRCODCLI = 0" + codcli
                    + "   and CRDUP = 1"
                    + "   and CRSTATUS < 2"
                    + "   and CRDTVCTO < :MDT1";
                mchdevAb += func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 18";

                mtotAb += mchdevAb;

                par = new OracleParameter[1];
                par[0] = new OracleParameter("MDT1", mdiatrab);

                //Cheques pré - datados
                sql = "select sum(CHVALOR) from CHEQUE"
                    + " where CHCODCLI = 0" + codcli
                    + "   and CHDTDEP >= :MDT1";
                mchPre = func.ParaDouble(con.Resultado(sql, par));
log += " Ponto de verificação 19";

                mtotAb += mchPre;

                // Verificando se tem credito de devolucao de cliente
                sql = "select sum(decode(N1VLUSADO,Null,0,N1VLUSADO)) - sum(N1VALTOT) VALCRE"
                    + " from CADNCR"
                    + " where N1STATUS=1 and N1VALTOT>0"
                    + "   and not N1VALTOT is Null and N1BLQ_CRED is Null"
                    + "   and N1CODCLI=0" + codcli;
                mcreDev = func.ParaDouble(con.Resultado(sql));
log += " Ponto de verificação 20";

                sql = "select (CCVALOR - CCVALQUI) VALCRE from CREDCLI"
                    + " where CCSTATUS<2 "
                    + "   and CCDBCR='C'"
                    + "   and CCNUMERO>0"
                    + "   and CCCODCLI=0" + codcli;
                mcreDev += func.ParaDouble(con.Resultado(sql));

log += " Ponto de verificação 21";

                mvaldisp = mlimcre - mtotAb;

log += " Ponto de verificação 22";

                credito = new CREDITO();
                credito.LIMITE_CREDITO = mlimcre;
                credito.CHEQUES_DEVOLVIDOS = mchdevAb;
                credito.CHEQUES_PRE_DATADOS = mchPre;
                credito.COMPRAS_NO_MES = mcprmes;
                credito.COMPRA_MENSAL_PERMITIDA = mvdamax;
                credito.CREDITO_DISPONIVEL = mvaldisp;
                credito.CREDITO_POR_DEVOLUCOES = mcreDev;
                credito.TITULOS_A_VENCER = mvencer;
                credito.TITULOS_VENCIDOS = mvencido;
                credito.TOTAL_EM_ABERTO = mtotAb;

log += " Ponto de verificação 23";

            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("c:\\SGDAT\\Log\\CreditoCliente.log");
                sw.WriteLine(log);
                sw.WriteLine("Erro: " + ex.Message);
                sw.Close();
            }
            
            return credito;
        }
    }
}
