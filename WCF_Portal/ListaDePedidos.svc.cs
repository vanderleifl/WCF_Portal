using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using Dapper;
using System.IO;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "ListaDePedidos" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione ListaDePedidos.svc ou ListaDePedidos.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ListaDePedidos : IListaDePedidos
    {
        public IEnumerable<M_PED01> ObterPedidos(long codcli, DateTime dtIni, DateTime dtFin)
        {
            string log = "Inicio";
            try
            {
                Conexao con = new Conexao();
                log += " Passei 1";

                int codEmp = con.codEmp;

                string sql;
                sql = "select P1INDICE, P1NUMERO, P1DATA, P1DESC, P1VALBRU, P1VALDES,"
                    + "       P1VALFIN, P1VALIMP, P1VALLIQ, P1PEDGER, P1ORCGER, P1STATUS,"
                    + "       P1CASAS, P1ORIGEM, P1LSEP, P1HORREC, P1OBS"
                    + " from MOB_PED01"
                    + " where P1CODCLI = 0" + codcli.ToString()
                    + "   and P1DATA >= '" + dtIni.ToString("dd/MM/yyyy") + "'"
                    + "   and P1DATA <= '" + dtFin.ToString("dd/MM/yyyy") + "' " 
                    +$"   and P1CODEMP = {codEmp} "
                    + " order by P1INDICE desc";
                log += " Passei 2 - sql: " + sql;
                var lista = con.ConOra.Query<M_PED01>(sql);
                log += " Passei 3";
                con.FecharConexao();
                return lista;
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\_ListaDePedidos_ObterPedidos.log");
                sw.WriteLine(ex.Message);
                sw.WriteLine(log);
                sw.Close();
                sw.Dispose();
                return null;
            }
        }

        public IEnumerable<M_PED02> ObterItens(int indice)
        {
            string log = "Inicio";
            try
            {
                Conexao con = new Conexao();
                log += " Passei 1";

                int codEmp = con.codEmp;

                string sql;
                sql = "select P2INDICE, P2NUMERO, P2ITEM, P2CODPRO, P2QTD, P2QTDFAT, P2PRECO,"
                    + "       P2DES1, P2DES2, P2DES3, P2DESF, P2TOTLIQ, P2PROMOCAO, P2KIT, P2BON,"
                    + "       PDNOME, PDMARCA, PDUND"
                    + " from MOB_PED02, CADPRO"
                    + " where P2CODPRO=PDCODPRO" 
                    + "   and P2INDICE = 0" + indice.ToString()
                    +$"   and P2CODEMP = {codEmp}"
                    + " order by P2ITEM";
                log += " Passei 2 - sql: " + sql;
                var lista = con.ConOra.Query<M_PED02>(sql);
                log += " Passei 3";
                con.FecharConexao();
                return lista;
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\_ListaDePedidos_ObterItens.log");
                sw.WriteLine(ex.Message);
                sw.WriteLine(log);
                sw.Close();
                sw.Dispose();
                return null;
            }
        }
    }
}
