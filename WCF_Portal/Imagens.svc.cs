using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Imagens" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Imagens.svc ou Imagens.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Imagens : IImagens
    {
        public IEnumerable<PS_IMAGENS> ObterLista()
        {
            IEnumerable<PS_IMAGENS> lista = null;
            string log = "Inicio";
            try
            {
                Conexao con = new Conexao();
                log += " Passei 1";
                string sql;
                sql = "select * from PS_IMAGENS "
                    + " where PS_STATUS > 0 "
                    +$"   and PS_CODEMP = {con.codEmp}";
                log += " Passei 2 - sql: " + sql;
                lista = con.ConOra.Query<PS_IMAGENS>(sql);
                log += " Passei 3";
                con.FecharConexao();
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\_Imagens_ObterLista.log");
                sw.WriteLine(ex.Message);
                sw.WriteLine(log);
                sw.Close();
                sw.Dispose();
            }
            return lista;
        }
        
    }
}
