using System.ServiceModel;
using System.Data;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IListaPedidos" in both code and config file together.
    [ServiceContract]
    public interface IListaPedidos
    {
        [OperationContract]
        DataSet ListaPedido(string codCli, string dataInicial, string dataFinal);

        [OperationContract]
        DataSet ListaItensPedido(double indice);
    }
}
