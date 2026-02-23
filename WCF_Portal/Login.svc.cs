using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.ServiceModel.Activation;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Login" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Login.svc or Login.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Login : ILogin
    {
        Conexao conexao = new Conexao();
       
        public List<CNPJ_VINCULADO> ObterCNPJsVinculados(long codcli)
        {
            conexao.AbrirConexao();
            List<CNPJ_VINCULADO> lista = new List<CNPJ_VINCULADO>();
            try
            {
                string sql = @"select GC_CODCLI_LOGIN, GC_CODCLI_VINCULADO, GC_CNPJ_LOGIN, GC_CNPJ_VINCULADO,
                                  CRAZAO, CDENOM
                           from PS_GRUPO_CLI, CADCLI
                           where CCODCLI = GC_CODCLI_VINCULADO
                             and GC_CODCLI_LOGIN=" + codcli.ToString();
                OracleDataReader r = conexao.FazerLeitura(sql);
                Func func = new Func();
                while (r.Read())
                {
                    CNPJ_VINCULADO cnpj = new CNPJ_VINCULADO();
                    cnpj.Login_Codigo = codcli;
                    cnpj.Login_CNPJ = func.ParaLongo(r["GC_CNPJ_LOGIN"]);
                    cnpj.Vinculado_Codigo = func.ParaLongo(r["GC_CODCLI_VINCULADO"]);
                    cnpj.Vinculado_CNPJ = func.ParaLongo(r["GC_CNPJ_VINCULADO"]);
                    cnpj.Vinculado_Razao = func.ParaString(r["CRAZAO"]);
                    cnpj.Vinculado_Fantasia = func.ParaString(r["CDENOM"]);
                    lista.Add(cnpj);
                }                                
            }
            catch (Exception ex)
            {
                StreamWriter escritor = new StreamWriter("C:\\SGDAT\\LOG\\login_CNPJs_Vinculados.log");
                escritor.WriteLine(ex.Message);
                escritor.Close();
                escritor.Dispose();
            }

            return lista;
        }

        public List<PARAMETRO> ObterParametros()
        {
            string sql, log;
            conexao.AbrirConexao();
            log = "";
            List<PARAMETRO> tbPar = new List<PARAMETRO>();
            
            string mp2_D123PU = "N";
            try
            {
                sql = "select min(PA2_VALOR) from CADPAR2"
                    + " where PA2_CAMPO='DPCOUNIT'";
                mp2_D123PU = conexao.Resultado(sql).ToString();
            }
            catch (Exception ex)
            {
                mp2_D123PU = "N";
                log += ex.Message;
            }

            string mcalcularST = conexao.DadosPadrao("API_PS", "CBxCalcularST", "N");
            string msomarST_preco = conexao.DadosPadrao("API_PS", "CBxSomarST_Preco", "N");
            string mostrarLimCre = conexao.DadosPadrao("API_PS", "PS_MostrarLimCre", "N");            
            string NaoVenderSemPRM = conexao.DadosPadrao("API_PS", "PS_NaoVenderSemPrm", "N");
            string NaoVenderSemEst = conexao.DadosPadrao("API_PS", "PS_NaoVenderSemEst", "N");
            string AcessoStatu8 = conexao.DadosPadrao("API_PS", "PS_AcessoStatus8", "N");
            string DIAS_VCTO = conexao.DadosPadrao("API_PS", "DIAS_VCTO", "30");
            string AlterarPV = conexao.DadosPadrao("API_PS", "PS_AlterarPV", "N");
            string ProdComEstoque = conexao.DadosPadrao("API_PS", "PS_ProdComEstoque", "N");

            tbPar.Add(new PARAMETRO("CalcularST", mcalcularST));
            tbPar.Add(new PARAMETRO("SomarST_Preco", msomarST_preco));
            tbPar.Add(new PARAMETRO("P2_D123PU", mp2_D123PU));
            tbPar.Add(new PARAMETRO("MostrarLimCre", mostrarLimCre));
            tbPar.Add(new PARAMETRO("NaoVenderSemPrm", NaoVenderSemPRM));
            tbPar.Add(new PARAMETRO("NaoVenderSemEst", NaoVenderSemEst));
            tbPar.Add(new PARAMETRO("AcessoStatu8", AcessoStatu8));
            tbPar.Add(new PARAMETRO("DIAS_VCTO", DIAS_VCTO));
            tbPar.Add(new PARAMETRO("AlterarPV", AlterarPV));
            tbPar.Add(new PARAMETRO("ProdComEstoque", ProdComEstoque));

            if (!String.IsNullOrEmpty(log))
            {
                StreamWriter escritor = new StreamWriter("C:\\SGDAT\\LOG\\login_par.log");
                escritor.WriteLine(log);
                escritor.Close();
                escritor.Dispose();
            }

            return tbPar;
        }

        public DataSet DadosDoCliente(string cnpj)
        {
            string log = "";
            Func func = new Func();

            int codemp = conexao.codEmp;
            log += Environment.NewLine + "Carregamento CADEMP";

            /*
            string arqPortalSeller = @"C:\SGDAT\PortalSeller.INI";
            if (File.Exists(arqPortalSeller))
            {
                log += Environment.NewLine + "Achou o arquivo INI";
                StreamReader sr = new StreamReader(arqPortalSeller);
                while (!sr.EndOfStream)
                {
                    string str = sr.ReadLine();
                    log += Environment.NewLine + "Conteudo: " + str;
                    if (str.StartsWith("[CODEMP]"))
                    {
                        str = str.Substring(8).Trim();
                        codemp = func.ParaInteiro(str);
                        codemp = conexao.codEmp;
                    }
                }
            }
            */

            log += Environment.NewLine + "CODEMP: " + codemp.ToString("000");

            DataSet dsDadosCliente = null;
            try
            {
                conexao.AbrirConexao();
                string sql;

                sql = "select CCODCLI, CELETRON, CCODVEN, CCGC, CIE, CRAZAO, CDENOM, CENDER, NOMBAI as CBAIRRO, CCEP, CLCREC, "
                    + "       LNOME, LEST, CPVTIP, CPVTAB, CDESC, CPCOPED, PTIPOPC,"
                           + "       PTIPOPCO, CCODLOC, CCODBCO, CTIPOCOB, CREDE, CPADTP, CSTATUS, "
                           + "       CRAMO, CVDAMEN, CDVALV, CDTVALANVISA, CDTLICUFSAUDE, CDOC01_VALIDADE, CDOC02_VALIDADE, CDATALV_P344,"
                           + "       CCNAE, CALIQ, CALIQDEB, CALIQRST, CALIQRTR, CICMSRET, CREPAS, CCONSFIN, CREDTRIB, CBSTPCOLIQ,"
                           + "       CCONV_8702, CDDEC5825, CDEC5834, CSSIMPLES, CVALMINPED,"
                           + "       CCODBAI, CNAOCONTRIBUINTE, PIMPADPRO, PIMPADPRO2, PAUMPCO_PROIMP,"
                           + "       PIMPADPRO_CF_C, PIMPADPRO_CF_C2, PAUMPCO_PROIMP_CF_C, PIMPADPRONC, PIMPADPRONC2, PAUMPCO_PROIMP_CF_NC,"
                           + "       PCALALIQDEB, PTIPCAL, PBSTPCOLIQ,"
                           + "       PICMSD, PREDBCR_P, PICMCREDST, PCREDBCCREDST, PICMSC,"
                           + "       PCCONV79_94, PC_MGDEC44823, PIVA, PREDBCST_P, PIVANEG,"
                           + "       PREDBCST_N, PIVANEU, PREDBCST_I, PREPAS, PRED0P, PRED0N,"
                           + "       PRED0O, PRED1P, PRED1N, PRED1O, PRED0P_CF, PRED0N_CF,"
                           + "       PRED0O_CF, PRED1P_CF, PRED1N_CF, PRED1O_CF"
                           + " from CADCLI, CADLOC, CADPUF, CADBAI"
                           + " where CCODLOC=LCODLOC"
                           + "   and PUF = LEST"
                           + $"  and (PCODEMP is null or PCODEMP = {codemp})"
                           + "   and CODBAI = CCODBAI";
                if (!conexao.DadosPadrao("API_PS", "PS_AcessoStatus8", "N").Equals("S"))
                {
                    sql += "   and CSTATUS < 8";
                }
                log += " passei 3 ";
                dsDadosCliente = new DataSet();
                string sql2 = sql + $" and (CCGC = {cnpj} or CCPF = {cnpj})";
                OracleCommand comando = new OracleCommand(sql2, conexao.ConOra);
                OracleDataAdapter da = new OracleDataAdapter(comando);
                log += " passei 4 " + sql2;
                da.Fill(dsDadosCliente, "CADCLI");
                comando.Dispose();
                da.Dispose();

                if (dsDadosCliente.Tables["CADCLI"].Rows.Count <= 0)
                {
                    StreamWriter sw1 = new StreamWriter("C:\\SGDAT\\LOG\\login_cliente.log");
                    sw1.WriteLine("Nenhum cliente selecionado!");
                    sw1.WriteLine(log);
                    sw1.Close();
                    sw1.Dispose();
                }

                log += " passei 5";
                if (dsDadosCliente.Tables["CADCLI"].Rows.Count == 0)
                {
                    log += " passei 6";
                    sql += " and CCPF = 0" + cnpj;
                    dsDadosCliente.Dispose();
                    da = new OracleDataAdapter(sql, conexao.ConOra);
                    dsDadosCliente = new DataSet();
                    da.Fill(dsDadosCliente, "CADCLI");
                    da.Dispose();
                    log += " passei 7";
                }
                else
                {
                    sql = "select IUF, ICNAE, IIVA, IRED" +
                          " from IVA_CNAE_UF" +
                          " where IUF = '" + dsDadosCliente.Tables["CADCLI"].Rows[0]["LEST"].ToString() + "'" +
                          "   and ICNAE = '" + dsDadosCliente.Tables["CADCLI"].Rows[0]["CCNAE"].ToString() + "'";
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsDadosCliente, "IVA_CNAE_UF");
                    da.Dispose();
                    log += " passei 8";
                }
                
                sql = "select ECODEMP, ECGC, EINSCEST, ERAZAO, EFANTASIA, EUF, EEMAIL,"
                        + "       EENDERECO, ENUMERO, EBAIRRO, ECIDADE, EFONE1"
                        + " from CADEMP"
                        + $" where ECODEMP = {codemp}";
                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsDadosCliente, "CADEMP");
                da.Dispose();
                
                try
                {                    
                    sql = "select EA_CHAVE, EA_VALOR from ESM_PAR";
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsDadosCliente, "ESM_PAR");
                    da.Dispose();

                    sql = "select EX_INDICE, EX_CODPRO, EX_DTLAN, EX_DTEXP, EX_QTDESM, EX_STATUS, EX_DETALHES"
                        + " from ESM_EXT"
                        + " where EX_CODCLI = " + dsDadosCliente.Tables["CADCLI"].Rows[0]["CCODCLI"].ToString();
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsDadosCliente, "ESM_EXT");
                    da.Dispose();

                    sql = "select sum(EX_QTDESM) as QTDTOTAL"
                        + " from ESM_EXT"
                        + " where EX_CODCLI = " + dsDadosCliente.Tables["CADCLI"].Rows[0]["CCODCLI"].ToString()
                        + "   and EX_DTEXP >= sysdate";
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsDadosCliente, "ESM_TOTAL");
                    da.Dispose();
                }
                catch { }

                sql = "select sum(CRVALOR-Decode(CRPAGO,NULL,0,CRPAGO)) as Total "
                    + "from CADDAR "
                    + "where CRCODCLI = " + dsDadosCliente.Tables["CADCLI"].Rows[0]["CCODCLI"].ToString() + " "
                    + "and CRSTATUS<=1";
                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsDadosCliente, "CADDAR");
                comando.Dispose();
                da.Dispose();

                /*
                StreamWriter sw2 = new StreamWriter("C:\\SGDAT\\LOG\\login_cliente_final.log");
                sw2.WriteLine($"Cliente: {dsDadosCliente.Tables["CADCLI"].Rows[0]["CRAZAO"]}");
                sw2.WriteLine(log);
                sw2.Close();
                sw2.Dispose();
                */
            }
            catch (Exception ex)
            {
                if (!Directory.Exists("C:\\SGDAT\\Log"))
                {
                    Directory.CreateDirectory("C:\\SGDAT\\Log");
                }
                StreamWriter escritor = new StreamWriter("C:\\SGDAT\\Log\\wcf_Portal_login.log");
                escritor.WriteLine(ex.Message);
                escritor.WriteLine("string de conexão: " + conexao.ConOra.ConnectionString);
                escritor.WriteLine(log);
                escritor.Close();
                escritor.Dispose();
                return null;
            }
            return dsDadosCliente;
        }
    }
}
