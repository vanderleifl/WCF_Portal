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
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "ICreditoCliente" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface ICreditoCliente
    {
        [WebInvoke(Method = "POST", UriTemplate = "Obter?cnpj={cnpj}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        CREDITO Obter(long cnpj);
    }
    
    [DataContract]
    public class CREDITO
    {
        [DataMember]
        public double LIMITE_CREDITO { get; set; }

        [DataMember]
        public double CREDITO_DISPONIVEL { get; set; }

        [DataMember]
        public double COMPRA_MENSAL_PERMITIDA { get; set; }

        [DataMember]
        public double COMPRAS_NO_MES { get; set; }

        [DataMember]
        public double TITULOS_VENCIDOS { get; set; }

        [DataMember]
        public double TITULOS_A_VENCER { get; set; }

        [DataMember]
        public double CHEQUES_DEVOLVIDOS { get; set; }

        [DataMember]
        public double CHEQUES_PRE_DATADOS { get; set; }

        [DataMember]
        public double CREDITO_POR_DEVOLUCOES { get; set; }

        [DataMember]
        public double TOTAL_EM_ABERTO { get; set; }
    }
}
