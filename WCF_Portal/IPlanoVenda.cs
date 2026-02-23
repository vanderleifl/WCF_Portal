using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPlanoVenda" in both code and config file together.
    [ServiceContract]
    public interface IPlanoVenda
    {
        [OperationContract]
        DataSet PlanosDeVendaDoCliente(string codCli);

        /*
        [OperationContract]
        DataSet PvSite(string pvTip, string pvTab);
        */ 
    }
}
