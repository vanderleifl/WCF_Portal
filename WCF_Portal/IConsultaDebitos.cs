using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IConsultaDebitos" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IConsultaDebitos
    {
        [WebInvoke(Method = "POST", UriTemplate = "ListaTitulos?codcli={codcli}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        IEnumerable<DEBITO> ListaTitulos(long codcli);
    }

    [DataContract]
    public class DEBITO
    {
        [DataMember]
        public int CRNUMERO { get; set; }
        [DataMember]
        public string CRDESD { get; set; }
        [DataMember]
        public int CRDUP { get; set; }
        [DataMember]
        public int CRTIPO { get; set; }
        [DataMember]
        public int CRSTATUS { get; set; }
        [DataMember]
        public DateTime CRDTEMIS { get; set; }
        [DataMember]
        public DateTime CRDTVCTO { get; set; }
        [DataMember]
        public double CRVALOR { get; set; }
        [DataMember]
        public double CRPAGO { get; set; }
        [DataMember]
        public int CRNPED { get; set; }
    }
}
