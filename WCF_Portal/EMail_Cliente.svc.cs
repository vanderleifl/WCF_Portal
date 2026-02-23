using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EMail_Cliente" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EMail_Cliente.svc or EMail_Cliente.svc.cs at the Solution Explorer and start debugging.
    public class EMail_Cliente : IEMail_Cliente
    {
        Conexao conexao = new Conexao();

        public string ObterEMail(double cnpj)
        {
            if (cnpj <= 0)
            {
                return "Informe o CNPJ";
            }

            conexao.AbrirConexao();
            try
            {
                string sql = "select max(CEMAIL) from  CADCLI"
                           + " where CCGC = 0" + cnpj.ToString()
                           + "    or CCPF = 0" + cnpj.ToString();
                
                return conexao.Resultado(sql).ToString();
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        public string ObterSenha(double cnpj)
        {
            if (cnpj <= 0)
            {
                return "Informe o CNPJ";
            }

            conexao.AbrirConexao();
            try
            {
                string sql = "select max(CELETRON) from  CADCLI"
                           + " where CCGC = 0" + cnpj.ToString()
                           + "    or CCPF = 0" + cnpj.ToString();

                return conexao.Resultado(sql).ToString();
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }
    }
}
