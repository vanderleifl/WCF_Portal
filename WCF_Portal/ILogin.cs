using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.ServiceModel.Web;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILogin" in both code and config file together.
    [ServiceContract]
    public interface ILogin
    {
        [WebInvoke(Method = "POST", UriTemplate = "DadosDoCliente?cnpj={cnpj}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        DataSet DadosDoCliente(string cnpj);

        [WebInvoke(Method = "POST", UriTemplate = "ObterParametros", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        List<PARAMETRO> ObterParametros();

        [WebInvoke(Method = "POST", UriTemplate = "ObterCNPJsVinculados", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        List<CNPJ_VINCULADO> ObterCNPJsVinculados(long codcli);
    }

    [DataContract]
    public class CNPJ_VINCULADO
    {
        [DataMember]
        public long Login_Codigo { get; set; }
        [DataMember]
        public long Login_CNPJ { get; set; }
        [DataMember]
        public long Vinculado_Codigo { get; set; }
        [DataMember]
        public long Vinculado_CNPJ { get; set; }
        [DataMember]
        public string Vinculado_Razao { get; set; }
        [DataMember]
        public string Vinculado_Fantasia { get; set; }
    }

    [DataContract]
    public class PARAMETRO
    {
        string chave, valor;

        public PARAMETRO(string chave, string valor)
        {
            this.chave = chave;
            this.valor = valor;
        }

        [DataMember]
        public string CHAVE
        { 
            get { return chave; } 
            set { chave = value; } 
        }
        [DataMember]
        public string VALOR
        {
            get { return valor; }
            set { valor = value; }
        }
    }
}
