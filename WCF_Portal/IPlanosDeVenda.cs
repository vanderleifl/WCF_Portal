using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IPlanosDeVenda" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IPlanosDeVenda
    {
        [WebInvoke(Method = "POST", UriTemplate = "PlanosDeVendaDisponiveis?codcli={codcli}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        IEnumerable<PV_DISPONIVEL> PlanosDeVendaDisponiveis(long codcli);
    }

    [DataContract]
    public class PV_DISPONIVEL
    {
        [DataMember]
        public string PVTIP { get; set; }
        [DataMember]
        public string PVTAB { get; set; }
        [DataMember]
        public string NOME { get; set; }
    }
}
