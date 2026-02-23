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
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "ConsultaDebitos" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione ConsultaDebitos.svc ou ConsultaDebitos.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConsultaDebitos : IConsultaDebitos
    {
        public IEnumerable<DEBITO> ListaTitulos(long codcli)
        {
            string log = "Início";
            IEnumerable<DEBITO> lista = null;
            
            try
            {
                Conexao con = new Conexao();
                log += " Passei 1";
                string sql = "";

                int codEmp = con.codEmp;

                for (int x = 1; x <= 2; x++)
                {
                    if (x == 2)
                    {
                        sql += " union ";
                    }
                    sql += "select CRNUMERO, CRDESD, CRDUP, CRTIPO, CRSTATUS, CRDTEMIS, CRDTVCTO, CRVALOR, CRPAGO, CRNPED"
                        + (x == 1 ? " from CADDAR" : " from CADDAR50")
                        + " where CRCODCLI = 0" + codcli.ToString()
                        + "   and CRSTATUS < 2" 
                        +$"   and CRCODEMP = {codEmp}";
                }
                sql += " order by CRDTVCTO, CRNUMERO";
                log += " Passei 2 - sql: " + sql;
                lista = con.ConOra.Query<DEBITO>(sql);
                log += " Passei 3";

                con.FecharConexao();
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\_ConsultaDebitos.log");
                sw.WriteLine(ex.Message);
                sw.WriteLine(log);
                sw.Close();
                sw.Dispose();
            }
            return lista;
        }
    }
}
