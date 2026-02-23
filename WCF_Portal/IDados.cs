using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IProdutos" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IDados
    {
        [WebInvoke(Method = "POST", UriTemplate = "CarregarDados?codcli={codcli}&codpro={codpro}&filtroProdutos={filtroProdutos}&pvTip={pvTip}&pvTab={pvTab}&somenteComEstoque={somenteComEstoque}&CalculaST={CalculaST}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        DADOS CarregarDados(long codcli, long codpro, string filtroProdutos, string pvTip, string pvTab, bool somenteComEstoque = false, bool CalculaST = false);
    }

    [DataContract]
    public class DADOS
    {
        [DataMember]
        public CADEMP cademp { get; set; }
        [DataMember]
        public CADCLI cliente { get; set; }
        [DataMember]
        public IEnumerable<CADPRO> produtos { get; set; }
        [DataMember]
        public IEnumerable<CADPPR> promocoes { get; set; }
        [DataMember]
        public IEnumerable<IVA_RP_UF> iva_rp_uf { get; set; }
        [DataMember]
        public IEnumerable<CADIPCLI> cadipcli { get; set; }
        [DataMember]
        public IEnumerable<CADCFISCAL> cadcfiscal { get; set; }
        [DataMember]
        public IEnumerable<CAD_ST_UF> cad_st_uf { get; set; }
        [DataMember]
        public CADPV planoVenda { get; set; }
        [DataMember]
        public IEnumerable<CADEXC> cadexc { get; set; }
        [DataMember]
        public IEnumerable<CADPPR_EXC> cadppr_exc { get; set; }
    }

    [DataContract]
    public class CADEMP
    {
        [DataMember]
        public int ECODEMP { get; set; }
        [DataMember]
        public long ECGC { get; set; }
        [DataMember]
        public string ERAZAO { get; set; }
        [DataMember]
        public string EFANTASIA { get; set; }
    }

    [DataContract]
    public class IVA_RP_UF
    {
        [DataMember]
        public string IUF { get; set; }
        [DataMember]
        public int IRAMO { get; set; }
        [DataMember]
        public double IIVAPOS { get; set; }
        [DataMember]
        public double IIVANEG { get; set; }
        [DataMember]
        public double IIVAOUT { get; set; }
        [DataMember]
        public double IRED { get; set; }
    }

    [DataContract]
    public class CAD_ST_UF
    {
        [DataMember]
        public string ISIT_UF { get; set; }
        [DataMember]
        public int ISITCODEMP { get; set; }
        [DataMember]
        public string ISIT_REF { get; set; }
        [DataMember]
        public string ISIT_SIT_2 { get; set; }
        [DataMember]
        public string ISIT_SIT_3 { get; set; }
    }

    [DataContract]
    public class CADIPCLI
    {
        [DataMember]
        public long ICCODCLI { get; set; }
        [DataMember]
        public string ICPRO_CODNCM { get; set; }
        [DataMember]
        public string ICPRO_SIT_2 { get; set; }
        [DataMember]
        public string ICPRO_SIT_3 { get; set; }
        [DataMember]
        public string ICPRO_CODPC { get; set; }
        [DataMember]
        public double ICPRO_CREDSN { get; set; }
        [DataMember]
        public double ICPRO_ALIQCOFINS { get; set; }
        [DataMember]
        public double ICPRO_ALIQPIS { get; set; }
    }

    [DataContract]
    public class CADCFISCAL
    {
        [DataMember]
        public string CFUF { get; set; }
        [DataMember]
        public string CFCLAS_FISCAL { get; set; }
        [DataMember]
        public double CFALIQICMS { get; set; }
        [DataMember]
        public double CFALIQICMS_CF { get; set; }
        [DataMember]
        public double CFREDUZ_POS { get; set; }
        [DataMember]
        public double CFREDUZ_NEG { get; set; }
        [DataMember]
        public double CFREDUZ_OUT { get; set; }
        [DataMember]
        public string CFTIPOCLI { get; set; }
    }
    
    [DataContract]
    public class CADPV
    {
        [DataMember]
        public string PVTIP { get; set; }
        [DataMember]
        public string PVTAB { get; set; }
        [DataMember]
        public string PVNOME { get; set; }
        [DataMember]
        public int PVTPDC { get; set; }
        [DataMember]
        public double PVDES { get; set; }
        [DataMember]
        public double DESCLI { get; set; }
        [DataMember]
        public int PVTPDB { get; set; }
        [DataMember]
        public int PVTPD1 { get; set; }
        [DataMember]
        public double PVDES1 { get; set; }
        [DataMember]
        public int PVTPD2 { get; set; }
        [DataMember]
        public int PVD2A { get; set; }
        [DataMember]
        public double PVDF01A { get; set; }
        [DataMember]
        public double PVDF01B { get; set; }
        [DataMember]
        public double PVDF01 { get; set; }
        [DataMember]
        public double PVDF02A { get; set; }
        [DataMember]
        public double PVDF02B { get; set; }
        [DataMember]
        public double PVDF02 { get; set; }
        [DataMember]
        public double PVDF03A { get; set; }
        [DataMember]
        public double PVDF03B { get; set; }
        [DataMember]
        public double PVDF03 { get; set; }
        [DataMember]
        public double PVDF04A { get; set; }
        [DataMember]
        public double PVDF04B { get; set; }
        [DataMember]
        public double PVDF04 { get; set; }
        [DataMember]
        public int PVTPDESF { get; set; }
        [DataMember]
        public double PVDESCF { get; set; }
        [DataMember]
        public int PVDDESCF { get; set; }
        [DataMember]
        public int PVDIASDESCF { get; set; }
        [DataMember]
        public double PVD0S0 { get; set; }
        [DataMember]
        public double PVD0S1 { get; set; }
        [DataMember]
        public double PVD0S2 { get; set; }
        [DataMember]
        public double PVD0S3 { get; set; }
        [DataMember]
        public double PVD0S4 { get; set; }
        [DataMember]
        public double PVD0S5 { get; set; }
        [DataMember]
        public double PVD0S6 { get; set; }
        [DataMember]
        public double PVD0S7 { get; set; }
        [DataMember]
        public double PVD1S0 { get; set; }
        [DataMember]
        public double PVD1S1 { get; set; }
        [DataMember]
        public double PVD1S2 { get; set; }
        [DataMember]
        public double PVD1S3 { get; set; }
        [DataMember]
        public double PVD1S4 { get; set; }
        [DataMember]
        public double PVD1S5 { get; set; }
        [DataMember]
        public double PVD1S6 { get; set; }
        [DataMember]
        public double PVD1S7 { get; set; }
        [DataMember]
        public double PVD2S0 { get; set; }
        [DataMember]
        public double PVD2S1 { get; set; }
        [DataMember]
        public double PVD2S2 { get; set; }
        [DataMember]
        public double PVD2S3 { get; set; }
        [DataMember]
        public double PVD2S4 { get; set; }
        [DataMember]
        public double PVD2S5 { get; set; }
        [DataMember]
        public double PVD2S6 { get; set; }
        [DataMember]
        public double PVD2S7 { get; set; }
        [DataMember]
        public double PVDFS0 { get; set; }
        [DataMember]
        public double PVDFS1 { get; set; }
        [DataMember]
        public double PVDFS2 { get; set; }
        [DataMember]
        public double PVDFS3 { get; set; }
        [DataMember]
        public double PVDFS4 { get; set; }
        [DataMember]
        public double PVDFS5 { get; set; }
        [DataMember]
        public double PVDFS6 { get; set; }
        [DataMember]
        public double PVDFS7 { get; set; }
        [DataMember]
        public int PVPRZ01 { get; set; }
        [DataMember]
        public int PVPRZ02 { get; set; }
        [DataMember]
        public int PVPRZ03 { get; set; }
        [DataMember]
        public int PVPRZ04 { get; set; }
        [DataMember]
        public int PVPRZ05 { get; set; }
        [DataMember]
        public int PVPRZ06 { get; set; }
        [DataMember]
        public int PVPRZ07 { get; set; }
        [DataMember]
        public int PVPRZ08 { get; set; }
        [DataMember]
        public int PVTEMPRM { get; set; }
        [DataMember]
        public string PVPROMOCAO { get; set; }
        [DataMember]
        public double PVVALMIN { get; set; }
        [DataMember]
        public int PVTIPOCOB { get; set; }
        [DataMember]
        public int PVPRM_PCO { get; set; }
        [DataMember]
        public int PVPRM_D1 { get; set; }
        [DataMember]
        public int PVPRM_D2 { get; set; }
        [DataMember]
        public int PVPRM_D3 { get; set; }
        [DataMember]
        public int PVFINPRM { get; set; }
        [DataMember]
        public double PVAPLIB { get; set; }
        [DataMember]
        public double PVAPMON { get; set; }
        [DataMember]
        public double PVAPOUT { get; set; }
        [DataMember]
        public int PVPRECO { get; set; }
        [DataMember]
        public string PADTP { get; set; }
        [DataMember]
        public string UFCLI { get; set; }
        [DataMember]
        public string UFEMP { get; set; }
        [DataMember]
        public string PVMEDPRZ { get; set; }
        [DataMember]
        public int PVREDE { get; set; }
        [DataMember]
        public string PVSTAEXC { get; set; }
        [DataMember]
        public int PVGRUFOR { get; set; }
        [DataMember]
        public System.DateTime PVDATINI { get; set; }
        [DataMember]
        public System.DateTime PVDATFIN { get; set; }
    }

    [DataContract]
    public class CADPPR
    {
        [DataMember]
        public string PPPROMOCAO { get; set; }
        [DataMember]
        public long PPCODPRO { get; set; }
        [DataMember]
        public double PPVALMIN { get; set; }
        [DataMember]
        public string PPPVTIP { get; set; }
        [DataMember]
        public string PPPVTAB { get; set; }
        [DataMember]
        public double PPPCOVDA { get; set; }
        [DataMember]
        public double PPDESC_1 { get; set; }
        [DataMember]
        public double PPDESC_2 { get; set; }
        [DataMember]
        public double PPDESC_3 { get; set; }
        [DataMember]
        public double PPDESC_F { get; set; }
        [DataMember]
        public double PPQTDMIN { get; set; }
        [DataMember]
        public double PPQTDMAX { get; set; }
        [DataMember]
        public string PPKIT { get; set; }
        [DataMember]
        public string PPBONIFIC { get; set; }
        [DataMember]
        public string PPLOTE { get; set; }
        [DataMember]        
        public string PPOBS { get; set; }
    }

    [DataContract]
    public class CADCLI
    {
        [DataMember]
        public long CCODCLI { get; set; }
        [DataMember]
        public string CPVTIP { get; set; }
        [DataMember]
        public string CPVTAB { get; set; }
        [DataMember]
        public string CBLQPVCTO { get; set; }
        [DataMember]
        public double CDESC { get; set; }
        [DataMember]
        public int CPCOPED { get; set; }
        [DataMember]
        public int PTIPOPC { get; set; }
        [DataMember]
        public int CCODBAI { get; set; }
        [DataMember]
        public int PTIPOPCO { get; set; }
        [DataMember]
        public int CCODLOC { get; set; }
        [DataMember]
        public string LEST { get; set; }
        [DataMember]
        public int CCODBCO { get; set; }
        [DataMember]
        public string CTIPOCOB { get; set; }
        [DataMember]
        public int CREDE { get; set; }
        [DataMember]
        public string CPADTP { get; set; }
        [DataMember]
        public int CCODVEN { get; set; }
        [DataMember]
        public int CCODVEN2 { get; set; }
        [DataMember]
        public int CCODVEN3 { get; set; }
        [DataMember]
        public int CCODVEN4 { get; set; }
        [DataMember]
        public int CCODVEN5 { get; set; }

        //     CCONSFIN - 0 = Consumidor final, 1 = Revendedor
        [DataMember]
        public int CCONSFIN { get; set; }
        //     CNAOCONTRIBUINTE - "S" = não contribuinte
        [DataMember]
        public string CNAOCONTRIBUINTE { get; set; }

        //     PIMPADPRO, PIMPADPRO2, PAUMPCO_PROIMP - Aumento de preços - Opção 1, 2 e importados para Revendedor
        [DataMember]
        public double PIMPADPRO { get; set; }
        [DataMember]
        public double PIMPADPRO2 { get; set; }
        [DataMember]
        public double PAUMPCO_PROIMP { get; set; }

        //     PIMPADPRO_CF_C, PIMPADPRO_CF_C2, PAUMPCO_PROIMP_CF_C - Aumento de preços - Opção 1, 2 e importados para Consumidor Contribuinte
        [DataMember]
        public double PIMPADPRO_CF_C { get; set; }
        [DataMember]
        public double PIMPADPRO_CF_C2 { get; set; }
        [DataMember]
        public double PAUMPCO_PROIMP_CF_C { get; set; }

        //     PIMPADPRONC, PIMPADPRONC2, PAUMPCO_PROIMP_CF_NC - Aumento de preços - Opção 1, 2 e importados para Consumidor não Contribuinte
        [DataMember]
        public double PIMPADPRONC { get; set; }
        [DataMember]
        public double PIMPADPRONC2 { get; set; }
        [DataMember]
        public double PAUMPCO_PROIMP_CF_NC { get; set; }

        [DataMember]
        public string UF_Empresa { get; set; }
        [DataMember]
        public string FiltrarProdutos { get; set; }
        [DataMember]
        public string ProdutosStatus8 { get; set; }
    }

    [DataContract]
    public class CADPRO
    {
        [DataMember]
        public int ORDEM { get; set; }
        [DataMember]
        public long PDCODPRO { get; set; }
        [DataMember]
        public string PDCODBARRA { get; set; }
        [DataMember]
        public string PDNOME { get; set; }
        [DataMember]
        public string PDMARCA { get; set; }
        [DataMember]
        public string PDUND { get; set; }
        [DataMember]
        public double PDPCUSTO { get; set; }
        [DataMember]
        public double PDMULTIPLO { get; set; }
        [DataMember]
        public double PDVDAMAX { get; set; }
        [DataMember]
        public double PDVDAMIN { get; set; }
        [DataMember]
        public double ESTOQUE { get; set; }
        [DataMember]
        public double PDESTMIN { get; set; }
        [DataMember]
        public double PRECO { get; set; }
        [DataMember]
        public double PDDESC { get; set; }
        [DataMember]
        public double PDDES1 { get; set; }
        [DataMember]
        public double PDDES2 { get; set; }
        [DataMember]
        public double PDDESCF { get; set; }
        [DataMember]
        public int PDTIPO { get; set; }
        [DataMember]
        public int PDSTATUS { get; set; }
        [DataMember]
        public double PDQTDD02 { get; set; }
        [DataMember]
        public int QTDESM { get; set; }
        [DataMember]
        public int PDTP { get; set; }
        [DataMember]
        public double PDPCOENT { get; set; }
        [DataMember]
        public double PDPCONS { get; set; }
        [DataMember]
        public double FICMSC { get; set; }
        [DataMember]
        public string PDPROSER { get; set; }
        [DataMember]
        public string PDCOD_NCM { get; set; }
        [DataMember]
        public string PDSIT_UF { get; set; }
        [DataMember]
        public double PDPRECO_PAUTA { get; set; }
        [DataMember]
        public string PDSIT_1 { get; set; }
        [DataMember]
        public string PDSIT_2 { get; set; }
        [DataMember]
        public string PDSIT_3 { get; set; }
        [DataMember]
        public string PDSIT_4 { get; set; }
        [DataMember]
        public double PDREDBCR_P { get; set; }
        [DataMember]
        public double PDALIIVA_P { get; set; }
        [DataMember]
        public double PDALIQDEB_P { get; set; }
        [DataMember]
        public double PDALIQCRE_P { get; set; }
        [DataMember]
        public string PDPERM_REPAS { get; set; }
        [DataMember]
        public int PDRAMO { get; set; }
        [DataMember]
        public string PDCLASSIF { get; set; }
        [DataMember]
        public string PDNOME2 { get; set; }
        [DataMember]
        public string PDPADRAO_D { get; set; }
        [DataMember]
        public string PDPADRAO_DF { get; set; }
        [DataMember]
        public double PDDESC02 { get; set; }
        [DataMember]
        public double PDPAUMPCO { get; set; }
        [DataMember]
        public double FDESCFIN { get; set; }
        [DataMember]
        public string TEM_PROMOCAO { get; set; }
        [DataMember]
        public int MENOR_VENCIMENTO_DIAS { get; set; }
    }

    [DataContract]
    public class CADEXC
    {
        [DataMember]
        public int ECONTROLE { get; set; }
        [DataMember]
        public string EREF1 { get; set; }
        [DataMember]
        public string EREF2 { get; set; }
        [DataMember]
        public string ETIPO { get; set; }
    }

    [DataContract]
    public class CADPPR_EXC
    {
        [DataMember]
        public string CPEPROMOCAO { get; set; }
        [DataMember]
        public string CPETIPOEXC { get; set; }
        [DataMember]
        public long CPECODIGO { get; set; }
        [DataMember]
        public string CPEREFER { get; set; }
    }
}
