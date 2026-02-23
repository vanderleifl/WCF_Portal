using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Produto" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Produto.svc or Produto.svc.cs at the Solution Explorer and start debugging.
    public class Produto : IProduto
    {
        string cTipoCob, cCodBco, preco, codVen, cPadTp;
        int tipPco, cRede, tipPcouf, tipPcuf;
        double cDes;

        public DataSet Produtos(string codCli, string pvTip, string pvTab, string filtroProdutos, double codpro, bool CalculaST, bool somenteComEstoque)
        {
            Conexao conexao = new Conexao();

            DataSet dsProduto = null;
            OracleCommand comando = null;
            OracleDataAdapter da = null;
            
            string log = "inicio";
            DataSet dsUtil = new DataSet();
            dsProduto = new DataSet();
            string sql = "";
            string sqlPro = "";
            string ufCli = "";
            string ufEmp = "";
            string vendedores = "";
            if (codpro > 0)
            {
                sqlPro = " and PDCODPRO = " + codpro.ToString();
            }
            else if (!String.IsNullOrEmpty(filtroProdutos))
            {
                sqlPro = " and (upper(PDNOME) like '%" + filtroProdutos.ToUpper() + "%' or upper(PDNOME2) like '%" + filtroProdutos.ToUpper() + "%' or upper(PDMARCA) like '%" + filtroProdutos.ToUpper() + "%')";
            }

            conexao.AbrirConexao();
            log += " passei 1 ";
            try
            {
                string sqlUtil = "select CPVTIP, CPVTAB, CBLQPVCTO, CDESC, CPCOPED, CCODBAI, PTIPOPC,"
                               + "       PTIPOPCO, CCODLOC, LEST, CCODBCO, CTIPOCOB, CREDE, CPADTP, CCODVEN, CCODVEN2, CCODVEN3, CCODVEN4, CCODVEN5"
                               + " from  CADCLI, CADLOC, CADPUF"
                               + " where LCODLOC = CCODLOC"
                               + "   and PUF = LEST"
                               + "   and CCODCLI = 0" + codCli.ToString();
                comando = new OracleCommand(sqlUtil, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsProduto, "CADCLI");
                da.Dispose();
                da = null;

                if (dsProduto.Tables["CADCLI"].Rows.Count == 0)
                {
                    return null;
                }

                try
                {
                    ufCli = dsProduto.Tables["CADCLI"].Rows[0]["LEST"].ToString();
                    ufEmp = conexao.Resultado("select max(UF) UF from CADFIR").ToString();
                }
                catch (Exception ex)
                {
                    log += " erro UFs: " + ex.Message;
                }
                
                try
                {
                    cTipoCob = dsProduto.Tables["CADCLI"].Rows[0]["CTIPOCOB"].ToString();
                    cCodBco = dsProduto.Tables["CADCLI"].Rows[0]["CCODBCO"].ToString();
                }
                catch
                {
                    cTipoCob = "";
                    cCodBco = "0";
                }

                try
                {
                    cDes = Convert.ToDouble(dsUtil.Tables["CADCLI"].Rows[0]["CDESC"]);
                }
                catch
                {
                    cDes = 0;
                }

                try
                {
                    codVen = dsProduto.Tables["CADCLI"].Rows[0]["CCODVEN"].ToString();
                }
                catch
                {
                    codVen = "0";
                }

                try
                {
                    vendedores = codVen;
                    string v = dsProduto.Tables["CADCLI"].Rows[0]["CCODVEN2"].ToString();
                    if (!String.IsNullOrEmpty(v))
                    {
                        if (Convert.ToInt32(v) > 0)
                        {
                            vendedores += "," + v;
                        }
                    }

                    v = dsProduto.Tables["CADCLI"].Rows[0]["CCODVEN3"].ToString();
                    if (!String.IsNullOrEmpty(v))
                    {
                        if (Convert.ToInt32(v) > 0)
                        {
                            vendedores += "," + v;
                        }
                    }

                    v = dsProduto.Tables["CADCLI"].Rows[0]["CCODVEN4"].ToString();
                    if (!String.IsNullOrEmpty(v))
                    {
                        if (Convert.ToInt32(v) > 0)
                        {
                            vendedores += "," + v;
                        }
                    }

                    v = dsProduto.Tables["CADCLI"].Rows[0]["CCODVEN5"].ToString();
                    if (!String.IsNullOrEmpty(v))
                    {
                        if (Convert.ToInt32(v) > 0)
                        {
                            vendedores += "," + v;
                        }
                    }

                    if (vendedores.Equals("0"))
                    {
                        vendedores = "";
                    }
                    else
                    {
                        vendedores = " (" + vendedores + ")";
                    }
                }
                catch
                {
                    vendedores = "";
                }

                string rede = "";
                try
                {
                    rede = dsProduto.Tables["CADCLI"].Rows[0]["CREDE"].ToString();
                }
                catch { }

                try
                {
                    tipPco = Convert.ToInt32(dsProduto.Tables["CADCLI"].Rows[0]["CPCOPED"]);
                }
                catch (Exception ex)
                {
                    log += "Erro ao obter tipo de preço do cliente: " + ex.Message;
                    tipPco = 0;
                }

                try
                {
                    cRede = Convert.ToInt32(dsProduto.Tables["CADCLI"].Rows[0]["CREDE"]);
                }
                catch
                {
                    cRede = 0;
                }

                try
                {
                    cPadTp = dsProduto.Tables["CADCLI"].Rows[0]["CPADTP"].ToString().Substring(0, 1);
                }
                catch
                {
                    cPadTp = "0";
                }

                preco = "PDPRECO";

                if (tipPco == 0)
                {
                    try
                    {
                        tipPcouf = Convert.ToInt32(dsProduto.Tables["CADCLI"].Rows[0]["PTIPOPCO"]);
                        tipPcuf = Convert.ToInt32(dsProduto.Tables["CADCLI"].Rows[0]["PTIPOPC"]);
                    }
                    catch (Exception ex)
                    {
                        log += "Erro ao busca tipo de preço pelo estado: " + ex.Message;
                        tipPcouf = 1;
                        tipPcuf = 0;
                    }
                    if (tipPcuf == 0 && tipPcouf > 1)
                    {
                        preco += tipPcouf.ToString();
                    }
                }
                else if (tipPco <= 3)
                {
                    if (tipPco > 1) preco += tipPco.ToString();
                }
                else
                {
                    preco = "PDPCONS";
                    if (tipPco == 5) preco += "2";
                    else if (tipPco == 6) preco += "3";
                    else if (tipPco == 7) preco = "decode(PDPCUSTO, 0, PDPRECO, PDPCUSTO)";
                }

                log += " Preço padrão: " + preco;

                sqlUtil = "select PP_VALOR from PARPADRAO"
                    + " where PP_FORM = 'Exp_XML'"
                    + "   and PP_CAMPO = 'FILTRAR_PRO'";

                comando = new OracleCommand(sqlUtil, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsUtil, "CADPAR");
                da.Dispose();
                da = null;
                comando = null;

                sqlUtil = "select UF from CADFIR";
                comando = new OracleCommand(sqlUtil, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsUtil, "CADFIR");
                da.Dispose();
                da = null;

                sqlUtil = "select PP_VALOR from PARPADRAO"
                    + " where PP_FORM = 'Exp_XML'"
                    + "   and PP_CAMPO = 'CBxProStatus8'";
                comando = new OracleCommand(sqlUtil, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsUtil, "CADPAR1");
                da.Dispose();
                da = null;
                comando = null;

                // Verifica se o estoque é compartilhado
                var sqlEstqComp = "select PA2_VALOR from CADPAR2 where PA2_CAMPO = 'CbxEstComp'";
                
                var estoqueCompartilhado = conexao.ResultadoTexto(sqlEstqComp);

                string estoque = "(select sum(LPQTD) from LOTPRO where LPCODPRO = PDCODPRO and (LPDATVEN >= (select max(DATA) from FDIA) or LPDATVEN is null)";

                if (estoqueCompartilhado == "S")
                {
                    estoque += $" and LPCODEMP = {conexao.codEmp}";
                }

                estoque += ")";

                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\TESTE.log");
                sw.WriteLine($"{estoque}   {estoqueCompartilhado}   {conexao.codEmp}");
                sw.Close();
                sw.Dispose();

                string sqlPrmExc = "(select count(*) from CADPPR_EXC where CPEPROMOCAO = PPPROMOCAO and ((CPETIPOEXC = 'C' and CPECODIGO = " + codCli + ") or (CPETIPOEXC = 'RC' and CPECODIGO = " + cRede.ToString() + ") or (CPETIPOEXC = 'R' and CPECODIGO in " + vendedores + ") or (CPETIPOEXC = 'GF'))) > 0"
                                 + " or (select count(*) from CADPPR_EXC where CPEPROMOCAO = PPPROMOCAO) = 0";


                string sqlPrm = "decode((select count(*) from CADPPR"
                              + " where PPCODPRO = PDCODPRO"
                              + "   and PPDATINI <= (select max(DATA) from FDIA)"
                              + "   and PPDATFIN >= (select max(DATA) from FDIA)"
                              + "   and ((trim(PPPVTIP) is null and trim(PPPVTAB) is null) or (PPPVTIP = '" + pvTip + "' and PPPVTAB = '" + pvTab + "'))"
                              + "   and (PPCODRED is null or PPCODRED = 0 or PPCODRED = 0" + rede.ToString() + ")"
                              + "   and PPEXPORTAVEL in ('S','C','1','2')"
                              + "   and (PPCODVEN = 0 or PPCODVEN is null or PPCODVEN in " + vendedores + ")"
                              + "   and (" + sqlPrmExc + ")"
                              + "   and (trim(PPMULTIVEND) is null or "
                              + "       PPMULTIVEND like '%" + codVen + "%')), 0, 'N', 'S') as TEM_PROMOCAO";
                sql = " as ORDEM, PDCODPRO, PDCODBARRA, PDNOME, PDMARCA, PDUND, PDPCUSTO,"
                           + "       " + estoque + " as ESTOQUE, PDESTMIN,"
                           + "       " + preco + " as PRECO, " + (tipPco == 7 ? "0 as PDDESC, 0 as PDDES1, 0 as PDDES2, 0 as PDDESCF" : "PDDESC, PDDES1, PDDES2, PDDESCF")
                           + "       , PDTIPO, PDSTATUS, PDQTDD02, "
                           + " (select sum(EP_QTDESM) from ESM_PRO where EP_CODPRO = PDCODPRO) as QTDESM, " // Seleção de esmeraldas
                           + "       PDTP,  PDPCOENT, PDPCONS, FICMSC, PDPROSER, PDCOD_NCM, PDSIT_UF, PDPRECO_PAUTA, "
                           + "       PDSIT_2, PDSIT_3, PDSIT_4, PDREDBCR_P, PDALIIVA_P, PDALIQDEB_P, PDALIQCRE_P, "
                           + "       PDPERM_REPAS, PDRAMO, PDCLASSIF, "
                           + "       PDNOME2, " + (tipPco == 7 ? "'S' as PDPADRAO_D, 'S' as PDPADRAO_DF, 0 as PDDESC02" : "PDPADRAO_D, PDPADRAO_DF, PDDESC02")
                           + "       , PDPAUMPCO, FDESCFIN, " + sqlPrm
                           + " from  CADPRO, CADFOR "
                           + " where FCODFOR = PDCODFOR "
                    //+ "   and (NOT (select sum(LPQTD) from LOTPRO where LPCODPRO = PDCODPRO and (LPDATVEN >= (select max(DATA) from FDIA) or LPDATVEN is null)) IS NULL "
                    //+ "   OR ((select max(DATA) from FDIA) - pddtultsai) <= 60) "
                           + "   and  (PDSTATUS < 8 or PDSTATUS is null) "
                           + sqlPro;

                string sqlComEstoque = " and " + estoque + " > 0";
                string sqlSemEstoque = " and (" + estoque + " <= 0 or " + estoque + " is null)";

                string sqlauxpro = "select 0 " + sql + sqlComEstoque;

                if (!somenteComEstoque)
                {
                    sqlauxpro += " union select 1 " + sql + sqlSemEstoque;
                }
                sql = sqlauxpro;

                sql += " order by ORDEM, PDNOME";
                
                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsProduto, "CADPRO");
                da.Dispose();
                da = null;
                
                sql = "select PPPROMOCAO, PPCODPRO, PPVALMIN, PPCODRED, PPPVTIP, PPPVTAB, PPPCOVDA, PPDESC_1, PPDESC_2,"
                           + "       PPDESC_3, PPDESC_F, PPQTDMIN, PPQTDMAX, PPKIT, PPBONIFIC, PPLOTE, PPMULTIVEND"
                           + " from  CADPPR, CADPRO"
                           + " where PPCODPRO = PDCODPRO"
                           + " and   PPDATINI <= (select max(DATA) from FDIA)"
                           + " and   PPDATFIN >= (select max(DATA) from FDIA)"
                           + " and   ((trim(PPPVTIP) is null and trim(PPPVTAB) is null) or (PPPVTIP = '" + pvTip + "' and PPPVTAB = '" + pvTab + "'))"
                           + " and   (PPCODRED is null or PPCODRED = 0 or PPCODRED = 0" + rede.ToString() + ")"
                           + " and   PPEXPORTAVEL in ('S','C','1','2')"
                           + " and (PPCODVEN = 0 or PPCODVEN is null or PPCODVEN in " + vendedores + ")"
                           + " and (" + sqlPrmExc + ")"
                           + " and   (trim(PPMULTIVEND) is null or "
                           + "       PPMULTIVEND like '%" + codVen.ToString() + "%')"
                           + " " + sqlPro;
                
                log += " passei 2 ";

                try
                {
                    Directory.CreateDirectory("C:\\SGDAT\\LOG");
                    StreamWriter swTemp = new StreamWriter("C:\\SGDAT\\LOG\\sqlPrm.sql");
                    swTemp.WriteLine(sql);
                    swTemp.Close();
                    swTemp.Dispose();
                }
                catch { }

                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsProduto, "CADPPR");
                da.Dispose();
                da = null;

                if (CalculaST && dsProduto.Tables["CADPRO"].Rows.Count > 0)
                {
                    log += " passei 3 ";
                    sql = "select IUF, IRAMO, IIVAPOS, IIVANEG, IIVAOUT, IRED" +
                          " from IVA_RP_UF" +
                          " where IUF = '" + ufCli + "'" +
                          "   and IRAMO = 0" + dsProduto.Tables["CADPRO"].Rows[0]["PDRAMO"].ToString();
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsProduto, "IVA_RP_UF");
                    da.Dispose();
                    da = null;
                    log += " passei 3.1 ";
                    sql = "select ICCODCLI, ICPRO_CODNCM, ICPRO_SIT_2, ICPRO_SIT_3," +
                         " ICPRO_CODPC, ICPRO_CREDSN, ICPRO_ALIQCOFINS, ICPRO_ALIQPIS" +
                         " from CADIPCLI" +
                         " where ICCODCLI = 0" + codCli +
                         "   and ICPRO_CODNCM = '" + dsProduto.Tables["CADPRO"].Rows[0]["PDCOD_NCM"].ToString() + "'";
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsProduto, "CADIPCLI");
                    da.Dispose();
                    da = null;
                    log += " passei 3.2 ";
                    sql = "select CFUF, CFCLAS_FISCAL, CFALIQICMS, CFALIQICMS_CF," +
                         " CFREDUZ_POS, CFREDUZ_NEG, CFREDUZ_OUT, CFTIPOCLI" +
                         " from CADCFISCAL" +
                         " where CFUF = '" + ufCli + "'" +
                         "   and CFCLAS_FISCAL = '" + dsProduto.Tables["CADPRO"].Rows[0]["PDCLASSIF"].ToString() + "'";
                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsProduto, "CADCFISCAL");
                    da.Dispose();
                    da = null;
                    log += " passei 3.3 ";
                    sql = "select ISIT_UF, ISITCODEMP, ISIT_REF, ISIT_SIT_2, ISIT_SIT_3" +
                         " from CAD_ST_UF" +
                         " where ISIT_UF = '" + ufCli + "'" +
                         "   and ISIT_REF = '" + dsProduto.Tables["CADPRO"].Rows[0]["PDSIT_UF"].ToString() + "'" +
                         $"   and ISITCODEMP = {conexao.codEmp}";

                    comando = new OracleCommand(sql, conexao.ConOra);
                    da = new OracleDataAdapter(comando);
                    da.Fill(dsProduto, "CAD_ST_UF");
                    da.Dispose();
                    da = null;
                    log += " passei 3.4 ";
                }
                                
                if (pvTip == "PC")
                {
                    log += " passei 4 ";
                    sql = "select '" + pvTip + "' as PVTIP, '" + pvTab + "' as PVTAB, PCTPDC as PVTPDC, PCDES as PVDES, 0" + cDes.ToString() + " as DESCLI,"
                        + " PCTPDB as PVTPDB, PCTPD1 as PVTPD1, PCDES1 as PVDES1, PCTPD2 as PVTPD2, PCD2A as PVD2A,"
                        + " PCDF01A as PVDF01A, PCDF01B as PVDF01B, PCDF01 as PVDF01,"
                        + " PCDF02A as PVDF02A, PCDF02B as PVDF02B, PCDF02 as PVDF02,"
                        + " PCDF03A as PVDF03A, PCDF03B as PVDF03B, PCDF03 as PVDF03,"
                        + " PCDF04A as PVDF04A, PCDF04B as PVDF04B, PCDF04 as PVDF04,"
                        + " PCTPDESCF as PVTPDESF, PCDESCF as PVDESCF, PCDDESCF as PVDDESCF, PCDIASDESCF as PVDIASDESCF, "
                        + " PCD0S0 as PVD0S0, PCD0S1 as PVD0S1, PCD0S2 as PVD0S2, PCD0S3 as PVD0S3, PCD0S4 as PVD0S4, PCD0S5 as PVD0S5, PCD0S6 as PVD0S6, PCD0S7 as PVD0S7,"
                        + " PCD1S0 as PVD1S0, PCD1S1 as PVD1S1, PCD1S2 as PVD1S2, PCD1S3 as PVD1S3, PCD1S4 as PVD1S4, PCD1S5 as PVD1S5, PCD1S6 as PVD1S6, PCD1S7 as PVD1S7,"
                        + " PCD2S0 as PVD2S0, PCD2S1 as PVD2S1, PCD2S2 as PVD2S2, PCD2S3 as PVD2S3, PCD2S4 as PVD2S4, PCD2S5 as PVD2S5, PCD2S6 as PVD2S6, PCD2S7 as PVD2S7,"
                        + " PCDFS0 as PVDFS0, PCDFS1 as PVDFS1, PCDFS2 as PVDFS2, PCDFS3 as PVDFS3, PCDFS4 as PVDFS4, PCDFS5 as PVDFS5, PCDFS6 as PVDFS6, PCDFS7 as PVDFS7,"
                        + " PCPRZ01 as PVPRZ01, PCPRZ02 as PVPRZ02, PCPRZ03 as PVPRZ03, PCPRZ04 as PVPRZ04, PCPRZ05 as PVPRZ05, PCPRZ06 as PVPRZ06, PCPRZ07 as PVPRZ07, PCPRZ08 as PVPRZ08,"
                        + " PCTEMPRM as PVTEMPRM, PCPROMOCAO as PVPROMOCAO, 0 as PVVALMIN, 0" + cCodBco + " as PVTIPOCOB,"
                        + " PCPRM_PCO as PVPRM_PCO, PCPRM_D1 as PVPRM_D1, PCPRM_D2 as PVPRM_D2, PCPRM_D3 as PVPRM_D3, PCFINPRM as PVFINPRM,"
                        + " PCAPLIB as PVAPLIB, PCAPMON as PVAPMON, PCAPOUT as PVAPOUT, 0" + tipPco.ToString() + " as PVPRECO, " + cPadTp + " as PADTP,"
                        + " '" + dsProduto.Tables["CADCLI"].Rows[0]["LEST"].ToString() + "' as UFCLI,"
                        + " '" + dsProduto.Tables["CADFIR"].Rows[0]["UF"].ToString() + "' as UFEMP,"
                        + " PCMEDPRZ as PVMEDPRZ, 0 as PVREDE, 'N' as PVSTAEXC, 0 as PVGRUFOR"
                        + " from CADPVC"
                        + " where PCCODCLI = 0" + codCli.ToString()
                        + "   and PCTAB = '" + pvTab + "'";
                }
                else
                {
                    log += " passei 5 ";
                    sql = "select '" + pvTip + "' as PVTIP, '" + pvTab + "' as PVTAB, PVTPDC, PVDES, 0" + cDes.ToString() + " as DESCLI,"
                        + " PVTPDB, PVTPD1, PVDES1, PVTPD2, PVD2A, PVDF01A, PVDF01B, PVDF01,"
                        + " PVDF02A, PVDF02B, PVDF02, PVDF03A, PVDF03B, PVDF03, PVDF04A, PVDF04B, PVDF04,"
                        + " PVTPDESF, PVDESCF, PVDDESCF, PVDIASDESCF, "
                        + " PVD0S0, PVD0S1, PVD0S2, PVD0S3, PVD0S4, PVD0S5, PVD0S6, PVD0S7,"
                        + " PVD1S0, PVD1S1, PVD1S2, PVD1S3, PVD1S4, PVD1S5, PVD1S6, PVD1S7,"
                        + " PVD2S0, PVD2S1, PVD2S2, PVD2S3, PVD2S4, PVD2S5, PVD2S6, PVD2S7,"
                        + " PVDFS0, PVDFS1, PVDFS2, PVDFS3, PVDFS4, PVDFS5, PVDFS6, PVDFS7,"
                        + " PVPRZ01, PVPRZ02, PVPRZ03, PVPRZ04, PVPRZ05, PVPRZ06, PVPRZ07, PVPRZ08,"
                        + " PVTEMPRM, PVPROMOCAO, PVVALMIN, " + (cTipoCob == "S" ? "0" + cCodBco + " as PVTIPOCOB" : "PVTIPOCOB") + ","
                        + " PVPRM_PCO, PVPRM_D1, PVPRM_D2, PVPRM_D3, PVFINPRM,"
                        + " PVAPLIB, PVAPMON, PVAPOUT, 0" + tipPco.ToString() + " as PVPRECO, " + cPadTp + " as PADTP,"
                        + " '" + ufCli + "' as UFCLI,"
                        + " '" + ufEmp + "' as UFEMP,"
                        + " PVMEDPRZ, PVREDE, PVSTAEXC, PVGRUFOR"
                        + " from CADPV"
                        + " where PVTIP = '" + pvTip + "'"
                        + "   and PVTAB = '" + pvTab + "'";
                }

                log += " passei 6 ";

                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsProduto, "CADPV");
                
                log += " passei 7 ";
                da.Dispose();
                dsUtil.Dispose();
                da = null;
                comando = null;                                
                dsUtil = null;
                log += " passei 8 ";
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\LOG\\Erro_Produto_Portal.log");
                sw.Write(ex.Message);
                sw.Write(log);
                sw.Write(sql);
                sw.Close();

                return null;
            }

            StreamWriter sw1 = new StreamWriter("C:\\SGDAT\\LOG\\Produto.svc.log");
            sw1.Write(log);
            sw1.Close();

            return dsProduto;
        }        

        public DataSet SelecionaProduto(string codigoProduto)
        {
            Conexao conexao = new Conexao();
            
            OracleCommand comando = null;
            OracleDataAdapter da = null;
            
            DataSet dsSelecionaProduto = null;
            try
            {
                conexao.AbrirConexao();
                dsSelecionaProduto = new DataSet();

                string sql = "select PDCODPRO, PDCODBARRA, PDNOME, PDMARCA, PDUND, PDPCUSTO,"
                           + "       (select sum(LPQTD) from LOTPRO where LPCODPRO = PDCODPRO and (LPDATVEN >= (select max(DATA) from FDIA) or LPDATVEN is null)) ESTOQUE, PDESTMIN,"
                           + "       " + preco + " as PRECO, " + (tipPco == 7 ? "0 as PDDESC, 0 as PDDES1, 0 as PDDES2, 0 as PDDESCF" : "PDDESC, PDDES1, PDDES2, PDDESCF")
                           + "       , PDTIPO, PDSTATUS, PDQTDD02, "
                           + "       PDNOME2, " + (tipPco == 7 ? "'S' as PDPADRAO_D, 'S' as PDPADRAO_DF, 0 as PDDESC02" : "PDPADRAO_D, PDPADRAO_DF, PDDESC02")
                           + "       , PDQTDD02, PDPAUMPCO, FDESCFIN "
                           + "from  CADPRO, CADFOR "
                           + "where FCODFOR = PDCODFOR "
                           + "and   (PDSTATUS < 8 or PDSTATUS is null) "
                           + "and   PDCODPRO = " + codigoProduto + " "
                           + "order by PDNOME";
                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsSelecionaProduto, "CADPRO");
                comando.Dispose();
                comando = null;
                da.Dispose();
                da = null;

                return dsSelecionaProduto;
            }
            catch
            {
                return null;
            }
        }

        public DataSet Promocao(string codCli, string pvTip, string pvTab, double codPro, string promocao)
        {
            Conexao conexao = new Conexao();
            OracleCommand comando = null;
            OracleDataAdapter da = null;
            
            DataSet dsPromocao = null;
            conexao.AbrirConexao();
            try
            {
                dsPromocao = new DataSet();
                string rede = "", vendedores = "";
                codVen = "";

                string sql2 = "select CREDE, CCODVEN, CCODVEN2, CCODVEN3, CCODVEN4, CCODVEN5 from CADCLI where CCODCLI = 0" + codCli.ToString();
                OracleDataReader leitor = conexao.FazerLeitura(sql2);
                while (leitor.Read())
                {
                    rede = leitor["CREDE"].ToString();
                    codVen = leitor["CCODVEN"].ToString();

                    try
                    {
                        vendedores = codVen;
                        string v = leitor["CCODVEN2"].ToString();
                        if (!String.IsNullOrEmpty(v))
                        {
                            if (Convert.ToInt32(v) > 0)
                            {
                                vendedores += "," + v;
                            }
                        }

                        v = leitor["CCODVEN3"].ToString();
                        if (!String.IsNullOrEmpty(v))
                        {
                            if (Convert.ToInt32(v) > 0)
                            {
                                vendedores += "," + v;
                            }
                        }

                        v = leitor["CCODVEN4"].ToString();
                        if (!String.IsNullOrEmpty(v))
                        {
                            if (Convert.ToInt32(v) > 0)
                            {
                                vendedores += "," + v;
                            }
                        }

                        v = leitor["CCODVEN5"].ToString();
                        if (!String.IsNullOrEmpty(v))
                        {
                            if (Convert.ToInt32(v) > 0)
                            {
                                vendedores += "," + v;
                            }
                        }

                        if (vendedores.Equals("0"))
                        {
                            vendedores = "";
                        }
                        else
                        {
                            vendedores = " (" + vendedores + ")";
                        }
                    }
                    catch
                    {
                        vendedores = "";
                    }
                }
                string sqlPrmExc = "(select count(*) from CADPPR_EXC where CPEPROMOCAO = PPPROMOCAO and ((CPETIPOEXC = 'C' and CPECODIGO = " + codCli + ") or (CPETIPOEXC = 'RC' and CPECODIGO = " + rede.ToString() + ") or (CPETIPOEXC = 'R' and CPECODIGO in " + vendedores + ") or (CPETIPOEXC = 'GF'))) > 0"
                                 + " or (select count(*) from CADPPR_EXC where CPEPROMOCAO = PPPROMOCAO) = 0";
                string sql = "select PPPROMOCAO, PPCODPRO, PPVALMIN, PPCODRED, PPPVTIP, PPPVTAB, PPPCOVDA, PPDESC_1, PPDESC_2,"
                           + "       PPDESC_3, PPDESC_F, PPQTDMIN, PPQTDMAX, PPKIT, PPBONIFIC, PPLOTE, PPMULTIVEND"
                           + " from  CADPPR"
                           + " where PPDATINI <= (select max(DATA) from FDIA)"
                           + " and   PPDATFIN >= (select max(DATA) from FDIA)"
                           + " and   ((trim(PPPVTIP) is null and trim(PPPVTAB) is null) or (PPPVTIP = '" + pvTip + "' and PPPVTAB = '" + pvTab + "'))"
                           + " and   (PPCODRED is null or PPCODRED = 0 or PPCODRED = 0" + rede.ToString() + ")"
                           + " and   PPEXPORTAVEL in ('S','C','1','2')"
                           + " and (PPCODVEN = 0 or PPCODVEN is null or PPCODVEN in " + vendedores + ")"
                           + " and (" + sqlPrmExc + ")"
                           + " and   (trim(PPMULTIVEND) is null or "
                           + "       PPMULTIVEND like '%" + codVen.ToString() + "%')";

                if (codPro > 0)
                {
                    sql += " and PPCODPRO = 0" + codPro.ToString();
                }

                if (!String.IsNullOrEmpty(promocao))
                {
                    sql += " and PPPROMOCAO = '" + promocao + "'";
                }

                comando = new OracleCommand(sql, conexao.ConOra);
                da = new OracleDataAdapter(comando);
                da.Fill(dsPromocao, "CADPPR");
                da.Dispose();
                da = null;

                return dsPromocao;
            }
            catch
            {
                return null;
            }
        }        
    }
}
