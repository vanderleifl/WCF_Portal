using System;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Retorno" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Retorno.svc or Retorno.svc.cs at the Solution Explorer and start debugging.
    public class Retorno : IRetorno
    {
        Conexao conexao = new Conexao();

        public DataSet ListaPedido(string codCli, string data)
        {
            DataSet listaPedidos = null;

            string resposta;

            Func func = new Func();

            try
            {
                conexao.AbrirConexao();
                string sql = "select * from MOB_PED01 where P1CODCLI = " + codCli + " and P1DATA >= '" + data + "' order by P1DATA desc" ;
                OracleCommand comando = new OracleCommand(sql, conexao.ConOra);
                OracleDataAdapter da = new OracleDataAdapter(comando);
                listaPedidos = new DataSet();
                da.Fill(listaPedidos);
            }
            catch(Exception ex)
            {
                resposta = "O servidor retornou o seguinte erro para pedido: " + ex.Message;

                StreamWriter sw = new StreamWriter(func.ObterArquivoUnico("_svc_retorno_erro", ".log"));
                sw.WriteLine(resposta);
                sw.Close();
                sw.Dispose();
            }
            return listaPedidos;
        }
    }
}
