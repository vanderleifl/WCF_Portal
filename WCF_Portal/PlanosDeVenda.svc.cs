using System;
using System.Collections.Generic;
using System.IO;
using Dapper;
using System.Linq;
using System.ServiceModel.Activation;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "PlanosDeVenda" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione PlanosDeVenda.svc ou PlanosDeVenda.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PlanosDeVenda : IPlanosDeVenda
    {
        public IEnumerable<PV_DISPONIVEL> PlanosDeVendaDisponiveis(long codcli)
        {
            Conexao conexao = new Conexao();
            IEnumerable<PV_DISPONIVEL> planos = null;            
            try
            {
                string sql = "select PVTIP, PVTAB, PVNOM as NOME"
                           + " from CADPV"
                           + " where PVTIP = "
                           + "   (select CPVTIP from CADCLI where CCODCLI = " + codcli.ToString() + " )"
                           + "   and PVTAB = (select CPVTAB from CADCLI where CCODCLI = " + codcli.ToString() + " ) "
                           + "   and (PVUSO_PS<>'N' or PVUSO_PS is null)"
                           + " union "
                           + " select PCTPC as PVTIP, PCTAB as PVTAB, PCREF as NOME"
                           + " from CADPVC"
                           + " where PCCODCLI = " + codcli.ToString()
                           + "   and PCDTINI < '" + DateTime.Now.ToString("dd/MM/yyyy") + "' and PCDTVENC > '" + DateTime.Now.ToString("dd/MM/yyyy") + "' "
                           + " union "
                           + " select PSPVTIP as PVTIP, PSPVTAB as PVTAB, PVNOM as NOME"
                           + " from CADPV, CADPVS"
                           + " where PSCODCLI = " + codcli.ToString()
                           + "   and PSPVTIP=PVTIP and PSPVTAB=PVTAB"
                           + "   and (PVUSO_PS<>'N' or PVUSO_PS is null)";
                planos = conexao.ConOra.Query<PV_DISPONIVEL>(sql);
                conexao.FecharConexao();
                return planos.ToList();
            }
            catch (Exception ex)
            {
                StreamWriter escritor = new StreamWriter("C:\\SGDAT\\Log\\PlanosDeVenda_log.log");
                escritor.WriteLine(ex.Message);
                escritor.Close();
                escritor.Dispose();
                return null;
            }
        }
        
    }
}
