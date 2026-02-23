using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IListaDePedidos" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IListaDePedidos
    {
        [WebInvoke(Method = "POST", UriTemplate = "ObterPedidos?codcli={codcli}&dtIni={dtIni}&dtFin={dtFin}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        IEnumerable<M_PED01> ObterPedidos(long codcli, DateTime dtIni, DateTime dtFin);
        [WebInvoke(Method = "POST", UriTemplate = "ObterItens?indice={indice}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        IEnumerable<M_PED02> ObterItens(int indice);
    }

    [DataContract]
    public class M_PED01
    {
        [DataMember]
        public int P1INDICE { get; set; }
        [DataMember]
        public int P1NUMERO { get; set; }
        [DataMember]
        public DateTime P1DATA { get; set; }
        [DataMember]
        public double P1DESC { get; set; }
        [DataMember]
        public double P1VALBRU { get; set; }
        [DataMember]
        public double P1VALDES { get; set; }
        [DataMember]
        public double P1VALFIN { get; set; }
        [DataMember]
        public double P1VALIMP { get; set; }
        [DataMember]
        public double P1VALLIQ { get; set; }
        [DataMember]
        public int P1PEDGER { get; set; }
        [DataMember]
        public int P1ORCGER { get; set; }
        [DataMember]
        public int P1STATUS { get; set; }
        [DataMember]
        public string P1CASAS { get; set; }
        [DataMember]
        public string P1ORIGEM { get; set; }
        [DataMember]
        public string P1LSEP { get; set; }
        [DataMember]
        public string P1HORREC { get; set; }
        [DataMember]
        public string P1OBS { get; set; }
    }

    [DataContract]
    public class M_PED02
    {
        [DataMember]
        public int P2INDICE { get; set; }
        [DataMember]
        public int P2NUMERO { get; set; }
        [DataMember]
        public string P2ITEM { get; set; }
        [DataMember]
        public int P2CODPRO { get; set; }
        [DataMember]
        public double P2QTD { get; set; }
        [DataMember]
        public double P2QTDFAT { get; set; }
        [DataMember]
        public double P2PRECO { get; set; }
        [DataMember]
        public double P2DES1 { get; set; }
        [DataMember]
        public double P2DES2 { get; set; }
        [DataMember]
        public double P2DES3 { get; set; }
        [DataMember]
        public double P2DESF { get; set; }
        [DataMember]
        public double P2TOTLIQ { get; set; }
        [DataMember]
        public string P2PROMOCAO { get; set; }
        [DataMember]
        public string P2KIT { get; set; }
        [DataMember]
        public string P2BON { get; set; }
        [DataMember]
        public string PDNOME { get; set; }
        [DataMember]
        public string PDMARCA { get; set; }
        [DataMember]
        public string PDUND { get; set; }
    }
}
