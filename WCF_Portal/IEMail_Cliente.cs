using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEMail_Cliente" in both code and config file together.
    [ServiceContract]
    public interface IEMail_Cliente
    {
        [OperationContract]
        string ObterEMail(double cnpj);

        [OperationContract]
        string ObterSenha(double cnpj);
    }
}
