using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IRecuperaSenha" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IRecuperaSenha
    {
        [WebInvoke(Method = "POST", UriTemplate = "EnviarEmailConfirmacao?cnpj={cnpj}&endereco={endereco}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        int EnviarEmailConfirmacao(string cnpj, string endereco);

        [WebInvoke(Method = "POST", UriTemplate = "GravarSenha?codcli={codcli}&senha={senha}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        bool GravarSenha(string codcli, string senha);
    }
}
