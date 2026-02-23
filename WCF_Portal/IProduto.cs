using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProduto" in both code and config file together.
    [ServiceContract]
    public interface IProduto
    {
        [OperationContract]
        DataSet Produtos(string codCli, string pvTip, string pvTab, string filtroProdutos, double codpro, bool CalculaST, bool somenteComEstoque);

        [OperationContract]
        DataSet Promocao(string codCli, string pvTip, string pvTab, double codPro, string promocao);

        [OperationContract]
        DataSet SelecionaProduto(string codigoProduto);
    }
}
