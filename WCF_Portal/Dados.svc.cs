using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ServiceModel.Activation;
using Dapper;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Produtos" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Produtos.svc ou Produtos.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Dados : IDados
    {           
        public DADOS CarregarDados(long codcli, long codpro, string filtroProdutos, string pvTip, string pvTab, bool somenteComEstoque = false, bool CalculaST = false)
        {
            DADOS dados = null;
            bool erro = false;
            string log = "";
            try
            {
                Conexao conexao = new Conexao();
                IEnumerable<IVA_RP_UF> iva_rp_uf = null;
                IEnumerable<CADIPCLI> cadipcli = null;
                IEnumerable<CADCFISCAL> cadcfiscal = null;
                IEnumerable<CAD_ST_UF> cad_st_uf = null;
                CADCLI cliente = null;
                IEnumerable<CADPRO> produtos;
                IEnumerable<CADPPR> promocoes;
                CADPV planoVenda;
                IEnumerable<CADEXC> cadexc = null;
                IEnumerable<CADPPR_EXC> cadppr_exc = null;
                CADEMP cademp;

                string sql;

                Func func = new Func();
                long cnpjEmpresa = func.ParaLongo(conexao.Resultado("select min(CGC) from CADFIR"));

                log += Environment.NewLine + $"Verificando CNPJ da empresa: {cnpjEmpresa.ToString("00000000000000")}";

                if (func.Autorizado(cnpjEmpresa, "PS"))
                {
                    // Carregando dados da empresa
                    int codemp = conexao.codEmp;
                    string arqPortalSeller = @"C:\SGDAT\PortalSeller.INI";
                    log += Environment.NewLine + "Carregamento CADEMP";
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
                                /*if (codemp == 0)
                                {
                                    codemp = 1;
                                }
                                */
                                codemp = conexao.codEmp;
                            }
                        }
                    }

                    log += Environment.NewLine + "CODEMP: " + codemp.ToString("000");
                    cademp = null;
                    sql = $"select ECODEMP, ECGC, ERAZAO, EFANTASIA from CADEMP where ECODEMP = {codemp}";
                    try
                    {
                        cademp = conexao.ConOra.Query<CADEMP>(sql).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        log += Environment.NewLine + "Erro: " + ex.Message + " sql: " + sql;
                    }
                    if (cademp == null)
                    {
                        log += Environment.NewLine + "CADEMP null";
                    }
                    else
                    {
                        log += Environment.NewLine + "CADEMP carregado " + cademp.ECODEMP.ToString("000") + " - " + cademp.EFANTASIA;
                    }

                    // Carregando cliente
                    sql = "select CCODCLI, CPVTIP, CPVTAB, CBLQPVCTO, CDESC, CPCOPED, CCODBAI, PTIPOPC,"
                                   + "   PTIPOPCO, CCODLOC, LEST, CCODBCO, CTIPOCOB, CREDE, CPADTP, CCODVEN, CCODVEN2, CCODVEN3, CCODVEN4, CCODVEN5,"
                                   + "   CCONSFIN, CNAOCONTRIBUINTE, PIMPADPRO, PIMPADPRO2, PAUMPCO_PROIMP,"
                                   + "   PIMPADPRO_CF_C, PIMPADPRO_CF_C2, PAUMPCO_PROIMP_CF_C, PIMPADPRONC, PIMPADPRONC2, PAUMPCO_PROIMP_CF_NC"
                                   + " from  CADCLI, CADLOC, CADPUF"
                                   + " where LCODLOC = CCODLOC"
                                   + "   and PUF = LEST"
                                   + "   and PCODEMP = 0" + codemp.ToString()
                                   + "   and CCODCLI = 0" + codcli.ToString();

                    try
                    {
                        cliente = conexao.ConOra.Query<CADCLI>(sql).FirstOrDefault();
                        log += Environment.NewLine + " cliente carregado";
                        cliente.UF_Empresa = conexao.Resultado("select max(UF) UF from CADFIR").ToString();
                        log += Environment.NewLine + " UF empresa carregada: " + cliente.UF_Empresa;
                        cliente.FiltrarProdutos = func.ParaString(conexao.Resultado("select max(PP_VALOR) from PARPADRAO where PP_FORM = 'Exp_XML' and PP_CAMPO = 'FILTRAR_PRO'"));
                        log += Environment.NewLine + " FiltrarProdutos carregado: " + cliente.FiltrarProdutos;
                        cliente.ProdutosStatus8 = func.ParaString(conexao.Resultado("select PP_VALOR from PARPADRAO where PP_FORM = 'Exp_XML' and PP_CAMPO = 'CBxProStatus8'"));
                        log += Environment.NewLine + " ProdutosStatus8 carregado: " + cliente.ProdutosStatus8;
                    }
                    catch (Exception ex)
                    {
                        log += Environment.NewLine + "Erro cliente: " + ex.Message + " Sql: " + sql;
                        // throw new Exception(ex.Message);
                    }

                    if (cliente == null)
                    {
                        StreamWriter sw1 = new StreamWriter("C:\\SGDAT\\Log\\_svc_dados_clienteNulo.log");
                        sw1.WriteLine(log);
                        sw1.Close();
                        sw1.Dispose();
                        return null;
                    }

                    // Seleção de produtos
                    string sqlPro = "";
                    // Verificando filtro, caso não seja necessário selecionar todos os produtos
                    if (codpro > 0)
                    {
                        sqlPro = " and PDCODPRO = " + codpro.ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filtroProdutos))
                        {
                            sqlPro = " and (upper(PDNOME) like '%" + filtroProdutos.ToUpper() + "%' or upper(PDNOME2) like '%" + filtroProdutos.ToUpper() + "%' or upper(PDMARCA) like '%" + filtroProdutos.ToUpper() + "%')";
                        }
                    }

                    string par_filtroProdutos = conexao.DadosPadrao("API_PS", "FILTRAR_PRO", "");

                    if (!string.IsNullOrEmpty(par_filtroProdutos))
                    {
                        sqlPro += $" and ({par_filtroProdutos})"; 
                    }

                    string ufCli, ufEmp, cTipoCob, cCodBco, codVen, vendedores, rede, cPadTp, preco, codSup;
                    double cDes;
                    int tipPco, tipPcouf, tipPcuf;

                    ufCli = cliente.LEST;
                    ufEmp = cliente.UF_Empresa;

                    if (String.IsNullOrEmpty(cliente.CTIPOCOB))
                    {
                        cTipoCob = "0";
                    }
                    else
                    {
                        cTipoCob = cliente.CTIPOCOB.ToString();
                    }

                    cCodBco = cliente.CCODBCO.ToString();
                    cDes = cliente.CDESC;
                    codVen = cliente.CCODVEN.ToString();
                    rede = cliente.CREDE.ToString();
                    tipPco = cliente.CPCOPED;
                    cPadTp = "0";

                    codSup = conexao.ResultadoInteiro("select CFCODSUP from CADVEN where CFCODVEN = 0" + codVen.ToString()).ToString("0");

                    try
                    {
                        vendedores = codVen;
                        if (cliente.CCODVEN2 > 0)
                        {
                            vendedores += "," + cliente.CCODVEN2.ToString();
                        }
                        if (cliente.CCODVEN3 > 0)
                        {
                            vendedores += "," + cliente.CCODVEN3.ToString();
                        }
                        if (cliente.CCODVEN4 > 0)
                        {
                            vendedores += "," + cliente.CCODVEN4.ToString();
                        }
                        if (cliente.CCODVEN5 > 0)
                        {
                            vendedores += "," + cliente.CCODVEN5.ToString();
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
                        vendedores = codVen;
                    }

                    if (!String.IsNullOrEmpty(cliente.CPADTP) && cliente.CPADTP.Length > 1)
                    {
                        cPadTp = cliente.CPADTP.Substring(0, 1);
                    }
                    preco = "PDPRECO";

                    if (tipPco == 0)
                    {
                        tipPcouf = cliente.PTIPOPCO;
                        tipPcuf = cliente.PTIPOPC;

                        if (tipPcuf == 0 && tipPcouf > 1)
                        {
                            preco += tipPcouf.ToString();
                        }
                    }
                    else if (tipPco <= 3)
                    {
                        if (tipPco > 1)
                        {
                            preco += tipPco.ToString();
                        }
                    }
                    else
                    {
                        preco = "PDPCONS";
                        if (tipPco == 5) preco += "2";
                        else if (tipPco == 6) preco += "3";
                        else if (tipPco == 7) preco = "decode(PDPCUSTO, 0, PDPRECO, PDPCUSTO)";
                    }

                    var sqlParamEstoqueComp = "select PA2_VALOR from CADPAR2 where PA2_CAMPO = 'CbxEstComp'";

                    var estoqueNaoCompartilhado = conexao.ResultadoTexto(sqlParamEstoqueComp);

                    string estoque = "(select sum(LPQTD) from LOTPRO where LPCODPRO = PDCODPRO and (LPDATVEN >= (select max(DATA) from FDIA) or LPDATVEN is null)";

                    if(estoqueNaoCompartilhado == "S")
                    {
                        estoque += $" and LPCODEMP = {conexao.codEmp}";
                    }

                    estoque += ")";

                    // Verificar exclusividades de promoções
                    string sqlPrmExc = "(select count(*) from CADPPR_EXC where CPEPROMOCAO = PPPROMOCAO"
                                     + " and ((CPETIPOEXC = 'C' and CPECODIGO = " + codcli.ToString() + ")" // cliente
                                     + "   or (CPETIPOEXC = 'RC' and CPECODIGO = " + cliente.CREDE.ToString() + ")" // rede de clientes
                                     + "   or (CPETIPOEXC = 'R' and CPECODIGO in " + vendedores + ")" // representante
                                     + (codSup == "0" ? "" : " or(CPETIPOEXC = 'S' and CPECODIGO = " + codSup + ")") // supervisor
                                     + "   or(CPETIPOEXC = 'UF' and CPEREFER = '" + ufCli + "')" // UF
                                     + "   or(CPETIPOEXC = 'PV' and CPEREFER = '" + pvTip + "/" + pvTab + "')" // PV
                                     + "   or (CPETIPOEXC = 'GF'))) > 0" // grupo de fornecedores
                                     + " or (select count(*) from CADPPR_EXC where CPEPROMOCAO = PPPROMOCAO) = 0";

                    string sqlPrm = "decode((select count(*) from CADPPR"
                                      + " where PPCODPRO = PDCODPRO"
                                      + "   and PPDATINI <= (select max(DATA) from FDIA)"
                                      + "   and PPDATFIN >= (select max(DATA) from FDIA)"
                                      + "   and ((trim(PPPVTIP) is null and trim(PPPVTAB) is null) or (PPPVTIP = '" + pvTip + "' and PPPVTAB = '" + pvTab + "'))"
                                      + "   and PPEXPORTAVEL in ('S','C','1','2')"
                                      + "   and (" + sqlPrmExc + ")), 0, 'N', 'S') as TEM_PROMOCAO";

                    sql = " as ORDEM, PDCODPRO, PDCODBARRA, PDNOME, PDMARCA, PDUND, PDPCUSTO, PDMULTIPLO, PDVDAMAX, PDVDAMIN,"
                                   + "       " + estoque + " as ESTOQUE, PDESTMIN,"
                                   + "       " + preco + " as PRECO, " + (tipPco == 7 ? "0 as PDDESC, 0 as PDDES1, 0 as PDDES2, 0 as PDDESCF" : "PDDESC, PDDES1, PDDES2, PDDESCF")
                                   + "       , PDTIPO, PDSTATUS, PDQTDD02, "
                                   + " (select sum(EP_QTDESM) from ESM_PRO where EP_CODPRO = PDCODPRO) as QTDESM, " // Seleção de esmeraldas
                                   + "       PDTP,  PDPCOENT, PDPCONS, FICMSC, PDPROSER, PDCOD_NCM, PDSIT_UF, PDPRECO_PAUTA, "
                                   + "       PDSIT_1, PDSIT_2, PDSIT_3, PDSIT_4, PDREDBCR_P, PDALIIVA_P, PDALIQDEB_P, PDALIQCRE_P, "
                                   + "       PDPERM_REPAS, PDRAMO, PDCLASSIF, "
                                   + "       PDNOME2, " + (tipPco == 7 ? "'S' as PDPADRAO_D, 'S' as PDPADRAO_DF, 0 as PDDESC02" : "PDPADRAO_D, PDPADRAO_DF, PDDESC02")
                                   + "       , PDPAUMPCO, FDESCFIN, " + sqlPrm
                                   + "       , decode((select min(LPDATVEN) from LOTPRO where LPCODPRO=PDCODPRO and LPDATVEN > SYSDATE), null, 0, round((select min(LPDATVEN) from LOTPRO where LPCODPRO=PDCODPRO and LPDATVEN > SYSDATE) - SYSDATE)) MENOR_VENCIMENTO_DIAS"
                                   + " from  CADPRO, CADFOR "
                                   + " where FCODFOR = PDCODFOR "
                                   + "   and not PDNOME is null"
                                   + "   and (PDSTATUS < 8 or PDSTATUS is null) "
                                   + "   and (PDTIPO_ITEM = '00' or PDTIPO_ITEM is null) "
                                   + sqlPro;

                    string sqlComEstoque = " and " + estoque + " > 0";
                    string sqlSemEstoque = " and (" + estoque + " <= 0 or " + estoque + " is null)";

                    string sqlauxpro = "select 0 " + sql + sqlComEstoque;

                    if (!somenteComEstoque)
                    {
                        sqlauxpro += " union select 1 " + sql + sqlSemEstoque;
                    }
                    sql = sqlauxpro;

                    sql += " order by ORDEM, PDNOME, PDMARCA";
                    // Carregando produtos
                    log += Environment.NewLine + "Produtos: " + sql;
                    produtos = conexao.ConOra.Query<CADPRO>(sql);
                    /*
                    try
                    {
                        var teste = produtos.Where(p => p.PDCODPRO.Equals(19623));
                        StreamWriter ss = new StreamWriter("C:\\SGDAT\\Log\\__Produto19623.log");
                        ss.WriteLine("Menor Vencimento Dias: " + teste.FirstOrDefault().MENOR_VENCIMENTO_DIAS.ToString());
                        ss.Close();
                    }
                    catch
                    {

                    }
                    */

                    // Seleção de promoções
                    sql = "select PPPROMOCAO, PPCODPRO, PPVALMIN, PPPVTIP, PPPVTAB, PPPCOVDA, PPDESC_1, PPDESC_2,"
                        + "       PPDESC_3, PPDESC_F, PPQTDMIN, PPQTDMAX, PPKIT, PPBONIFIC, PPLOTE, PPOBS"
                        + " from  CADPPR, CADPRO"
                        + " where PPCODPRO = PDCODPRO"
                        + " and   PPDATINI <= (select max(DATA) from FDIA)"
                        + " and   PPDATFIN >= (select max(DATA) from FDIA)"
                        + " and   ((trim(PPPVTIP) is null and trim(PPPVTAB) is null) or (PPPVTIP = '" + pvTip + "' and PPPVTAB = '" + pvTab + "'))"
                        + " and   PPEXPORTAVEL in ('S','C','1','2')"
                        + " and (" + sqlPrmExc + ")"
                        + " " + sqlPro;

                    // Carregando promoções
                    log += Environment.NewLine + "Promoções: " + sql;
                    promocoes = conexao.ConOra.Query<CADPPR>(sql);

                    // Seleção de dados fiscais
                    if (CalculaST && produtos.Count() > 0)
                    {
                        sql = "select IUF, IRAMO, IIVAPOS, IIVANEG, IIVAOUT, IRED" +
                              " from IVA_RP_UF" +
                              " where IUF = '" + ufCli + "'";
                        iva_rp_uf = conexao.ConOra.Query<IVA_RP_UF>(sql);

                        sql = "select ICCODCLI, ICPRO_CODNCM, ICPRO_SIT_2, ICPRO_SIT_3," +
                             " ICPRO_CODPC, ICPRO_CREDSN, ICPRO_ALIQCOFINS, ICPRO_ALIQPIS" +
                             " from CADIPCLI" +
                             " where ICCODCLI = 0" + codcli;
                        cadipcli = conexao.ConOra.Query<CADIPCLI>(sql);

                        sql = "select CFUF, CFCLAS_FISCAL, CFALIQICMS, CFALIQICMS_CF," +
                             " CFREDUZ_POS, CFREDUZ_NEG, CFREDUZ_OUT, CFTIPOCLI" +
                             " from CADCFISCAL" +
                             " where CFUF = '" + ufCli + "'";
                        cadcfiscal = conexao.ConOra.Query<CADCFISCAL>(sql);

                        sql = "select ISIT_UF, ISITCODEMP, ISIT_REF, ISIT_SIT_2, ISIT_SIT_3" +
                             " from CAD_ST_UF" +
                             " where ISIT_UF = '" + ufCli + "'";
                        cad_st_uf = conexao.ConOra.Query<CAD_ST_UF>(sql);
                    }

                    // Seleção de PVs
                    if (pvTip == "PC")
                    {
                        sql = "select '" + pvTip + "' as PVTIP, '" + pvTab + "' as PVTAB, 'Exclusivo' as PVNOME, PCTPDC as PVTPDC, PCDES as PVDES, 0" + cDes.ToString() + " as DESCLI,"
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
                            + " '" + ufCli + "' as UFCLI,"
                            + " '" + ufEmp + "' as UFEMP,"
                            + " PCMEDPRZ as PVMEDPRZ, 0 as PVREDE, 'N' as PVSTAEXC, 0 as PVGRUFOR, PCDTINI as PVDATINI, PCDTVENC as PVDATFIN"
                            + " from CADPVC"
                            + " where PCCODCLI = 0" + codcli.ToString()
                            + "   and PCTAB = '" + pvTab + "'";
                    }
                    else
                    {
                        sql = "select '" + pvTip + "' as PVTIP, '" + pvTab + "' as PVTAB, PVNOM as PVNOME, PVTPDC, PVDES, 0" + cDes.ToString() + " as DESCLI,"
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
                            + " PVMEDPRZ, 0 as PVREDE, PVSTAEXC, 0 as PVGRUFOR, (SYSDATE-30) as PVDATINI, (SYSDATE+30) as PVDATFIN"
                            + " from CADPV"
                            + " where PVTIP = '" + pvTip + "'"
                            + "   and PVTAB = '" + pvTab + "'";
                    }

                    log += Environment.NewLine + "PV: " + sql;
                    planoVenda = conexao.ConOra.Query<CADPV>(sql).FirstOrDefault();

                    try
                    {
                        sql = "select CPEPROMOCAO, CPETIPOEXC, CPECODIGO, CPEREFER"
                           + " from CADPPR_EXC"
                           + " order by CPEPROMOCAO";
                        cadppr_exc = conexao.ConOra.Query<CADPPR_EXC>(sql);
                    }
                    catch (Exception ex)
                    {
                        log += Environment.NewLine + "CADPPR_EXC:" + ex.Message;
                    }

                    try
                    {
                        sql = "select ECONTROLE, EREF1, EREF2, ETIPO from CADEXC where ECONTROLE > 0";
                        cadexc = conexao.ConOra.Query<CADEXC>(sql);
                    }
                    catch (Exception ex)
                    {
                        log += Environment.NewLine + "CADEXC:" + ex.Message;
                    }

                    conexao.FecharConexao();

                    dados = new DADOS();
                    dados.cliente = cliente;
                    dados.planoVenda = planoVenda;
                    dados.produtos = produtos;
                    dados.promocoes = promocoes;
                    dados.cadcfiscal = cadcfiscal;
                    dados.cadipcli = cadipcli;
                    dados.cad_st_uf = cad_st_uf;
                    dados.iva_rp_uf = iva_rp_uf;
                    dados.cadexc = cadexc;
                    dados.cadppr_exc = cadppr_exc;
                    dados.cademp = cademp;
                }
            }
            catch (Exception ex)
            {
                log += Environment.NewLine + $"Erro geral: {ex.Message}";
                erro = true;
            }

            if (dados == null || erro)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\wcf_Portal_dados.svc.log");
                sw.WriteLine(log);
                sw.Close();
                sw.Dispose();
            }

            return dados;
        }


    }
}
