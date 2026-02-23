using System.ServiceModel;
using System.Data;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRetorno" in both code and config file together.
    [ServiceContract]
    public interface IRetorno
    {
        [OperationContract]
        DataSet ListaPedido(string codCli, string data);
    }
}
