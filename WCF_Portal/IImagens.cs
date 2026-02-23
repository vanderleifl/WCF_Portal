using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IImagens" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IImagens
    {
        [WebInvoke(Method = "POST", UriTemplate = "ObterLista", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        IEnumerable<PS_IMAGENS> ObterLista();
    }

    [DataContract]
    public class PS_IMAGENS
    {
        [DataMember]
        public string PS_TIPO { get; set; }
        [DataMember]
        public string PS_NOME { get; set; }
        [DataMember]
        public int PS_ORDEM { get; set; }
        [DataMember]
        public string PS_IMAGEM { get; set; }
        [DataMember]
        public int PS_STATUS { get; set; }
    }
}
