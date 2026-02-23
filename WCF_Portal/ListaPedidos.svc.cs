using System;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ListaPedidos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ListaPedidos.svc or ListaPedidos.svc.cs at the Solution Explorer and start debugging.
    public class ListaPedidos : IListaPedidos
    {
        Conexao conexao = new Conexao();

        public DataSet ListaPedido(string codCli, string dataInicial, string dataFinal)
        {
            DataSet listaPedidos = null;

            string resposta;

            Func func = new Func();

            int codEmp = conexao.codEmp;

            try
            {
                conexao.AbrirConexao();
                string sql = "select *                         " 
                           + "from MOB_PED01                   "
                           +$"where P1CODCLI = {codCli}        "
                           +$"  and P1DATA  >= '{dataInicial}' "
                           +$"  and P1DATA  <= '{dataFinal}'   "
                           +$"  and P1CODEMP = {codEmp}        "
                           + "order by P1DATA desc             ";

                OracleCommand comando = new OracleCommand(sql, conexao.ConOra);
                OracleDataAdapter da = new OracleDataAdapter(comando);
                listaPedidos = new DataSet();
                da.Fill(listaPedidos);
            }
            catch (Exception ex)
            {
                resposta = "O servidor retornou o seguinte erro para pedido: " + ex.Message;

                StreamWriter sw = new StreamWriter(func.ObterArquivoUnico("_svc_retorno_erro", ".log"));
                sw.WriteLine(resposta);
                sw.Close();
                sw.Dispose();
            }
            return listaPedidos;
        }

        public DataSet ListaItensPedido(double indice)
        {
            DataSet listaItens = null;

            string resposta;

            Func func = new Func();

            int codEmp = conexao.codEmp;

            try
            {
                conexao.AbrirConexao();
                string sql = "select P2INDICE, P2CODPRO, P2PRECO, P2DES1, P2DES2, P2DES3, P2DESF, P2TOTLIQ,"
                           + " P2KIT, P2PROMOCAO, P2LOTE, P2BON, P2QTD, P2QTDFAT, P2STATUS, P2CODEMP,"
                           + " PDNOME, PDNOME2, PDMARCA, PDUND"
                           + " from MOB_PED02, CADPRO"
                           + " where P2CODPRO = PDCODPRO"
                           + "   and P2INDICE = " + indice.ToString() 
                           +$"   and P2CODEMP = {codEmp}";
                OracleCommand comando = new OracleCommand(sql, conexao.ConOra);
                OracleDataAdapter da = new OracleDataAdapter(comando);
                listaItens = new DataSet();
                da.Fill(listaItens);
            }
            catch (Exception ex)
            {
                resposta = "O servidor retornou o seguinte erro para pedido: " + ex.Message;

                StreamWriter sw = new StreamWriter(func.ObterArquivoUnico("_svc_Lista_Itens_erro", ".log"));
                sw.WriteLine(resposta);
                sw.Close();
                sw.Dispose();
            }
            return listaItens;
        }
    }
}
