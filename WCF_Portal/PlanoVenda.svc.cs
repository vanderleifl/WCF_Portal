using System;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PlanoVenda" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PlanoVenda.svc or PlanoVenda.svc.cs at the Solution Explorer and start debugging.
    public class PlanoVenda : IPlanoVenda
    {

        public DataSet PlanosDeVendaDoCliente(string codCli )
        {
            Conexao conexao = new Conexao();

            DataSet dsPlanoVenda;
            try
            {
                string sql = "select PVTIP, PVTAB, PVNOM as NOME"
                           + " from CADPV"
                           + " where PVTIP = "
                           + "   (select CPVTIP from CADCLI where CCODCLI = " + codCli  + " )"
                           + "   and PVTAB = (select CPVTAB from CADCLI where CCODCLI = " + codCli  + " ) "
                           + " union "
                           + " select PCTPC as PVTIP, PCTAB as PVTAB, PCREF as NOME"
                           + " from CADPVC"
                           + " where PCCODCLI = " + codCli  
                           + "   and PCDTINI < '" + DateTime.Now.ToString("dd/MM/yyyy") + "' and PCDTVENC > '" + DateTime.Now.ToString("dd/MM/yyyy") + "' "
                           + " union "
                           + " select PSPVTIP as PVTIP, PSPVTAB as PVTAB, PVNOM as NOME"
                           + " from CADPV, CADPVS"
                           + " where PSCODCLI = " + codCli  
                           + "   and PSPVTIP=PVTIP and PSPVTAB=PVTAB";
                OracleCommand comando = new OracleCommand(sql, conexao.ConOra);
                OracleDataAdapter da = new OracleDataAdapter(comando);
                dsPlanoVenda = new DataSet();
                da.Fill(dsPlanoVenda, "PVS");
                comando.Dispose();
                da.Dispose();
                /*
                sql = "select sum(CRVALOR-Decode(CRPAGO,NULL,0,CRPAGO)) as Total "
                    + "from CADDAR "
                    + "where CRCODCLI = " + codCli + " "
                    + "and CRSTATUS<=1";
                comando = new OracleCommand(sql, conexao.conexao);
                da = new OracleDataAdapter(comando);
                da.Fill(dsPlanoVenda, "CADDAR");
                comando.Dispose();
                da.Dispose();
                */
                return dsPlanoVenda;
            }                
            catch(Exception ex) 
            {
                StreamWriter escritor = new StreamWriter("C:\\SGDAT\\Log\\PlanoVenda_log.log");
                escritor.WriteLine(ex.Message);
                escritor.Close();
                escritor.Dispose();
                dsPlanoVenda = null;
                return null;
            }
        }

        /*
        public DataSet PvSite(string pvTip, string pvTab)
        {
            try
            {
                if (String.IsNullOrEmpty(pvTip))
                {
                    pvTip = "AP";
                }
                if (String.IsNullOrEmpty(pvTab))
                {
                    pvTab = "99";
                }
                string sql = "select *"
                           + " from CADPV"
                           + " where PVTIP = '" + pvTip + "' and PVTAB = '" + pvTab + "'";
                DataSet dsPv = new DataSet();
                OracleCommand comando = new OracleCommand(sql, conexao.conexao);
                OracleDataAdapter da = new OracleDataAdapter(comando);
                da.Fill(dsPv);
                return dsPv;
            }
            catch
            {
                return null;
            }
            
        }
        */ 
    }
}
