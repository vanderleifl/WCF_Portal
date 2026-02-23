using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace WCF_Portal
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "RecuperaSenha" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione RecuperaSenha.svc ou RecuperaSenha.svc.cs no Gerenciador de Soluções e inicie a depuração.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RecuperaSenha : IRecuperaSenha
    {
        Conexao conexao = new Conexao();

        public int EnviarEmailConfirmacao(string cnpj, string endereco)
        {
            // Retornos possíveis:
            // -3 : CNPJ não encontrado
            // -2 : Não foi encontrado um e-mail válido associado a este CNPJ
            // -1 : Ocorreu um erro ao enviar e-mail. Favor entrar em contato com a empresa
            //  0 : E-mail enviado
            int r = -3;
            string sql, email, empresa;
            sql = String.Format("select CCODCLI from CADCLI where CCGC = {0} or CCPF = {0}", cnpj);
            long codcli = conexao.ResultadoLongo(sql);
            if (codcli > 0)
            {
                sql = String.Format("select CEMAIL from CADCLI where CCGC = {0} or CCPF = {0}", cnpj);
                email = conexao.ResultadoTexto(sql);
                r = -2;
                if (email.Length > 5 && email.Contains("@") && email.Contains("."))
                {
                    try
                    {
                        sql = "select RAZAO from CADFIR";
                        empresa = conexao.ResultadoTexto(sql);

                        // Enviar e-mail com link
                        string remetenteEmail = "recupera@digitalsistemas.com.br";
                        string destinatarioEmail = email;
                        string senhaCred = "Web@cesso2020";
                        // Criando a mensagem de e-mail
                        MailMessage mail = new MailMessage();

                        // Atribuindo os endereços
                        mail.From = new MailAddress(remetenteEmail);
                        mail.To.Add(destinatarioEmail);

                        // Definindo o conteúdo
                        mail.Subject = "Senha PortalSeller";
                        string corpo = "Olá, você solicitou a definição de senha do PortalSeller."
                                     + "\n\n"
                                     + "Clique no link abaixo, ou copie e cole o endereço na barra de endereços do seu navegador web e siga as instruções da tela."
                                     + "\n";

                        string link = endereco;
                        if (!link.StartsWith("http"))
                        {
                            link = "http://" + link;
                        }
                        if (!link.EndsWith("/"))
                        {
                            link += "/";
                        }
                        link += "novaSenha.aspx?codigo=" + codcli.ToString();
                        corpo += link;

                        corpo += "\n\nSe você não solicitou a senha do PortalSeller, por favor entre em contato imediatamente com a empresa " + empresa + ".";

                        mail.Body = corpo;

                        // Enviando a mensagem
                        SmtpClient smtp = new SmtpClient("smtp.digitalsistemas.com.br", 587);
                        smtp.Credentials = new NetworkCredential(remetenteEmail, senhaCred);
                        smtp.Send(mail);

                        r = 0;
                    }
                    catch (Exception ex)
                    {
                        r = -1;

                        if (!Directory.Exists("C:\\SGDAT\\Log"))
                        {
                            Directory.CreateDirectory("C:\\SGDAT\\Log");
                        }
                        StreamWriter escritor = new StreamWriter("C:\\SGDAT\\Log\\RecuperaSenha_EnviarEmailConfirmacao.log");
                        escritor.WriteLine(ex.Message);
                        escritor.WriteLine("string de conexão: " + conexao.ConOra.ConnectionString);
                        escritor.Close();
                    }
                }
                else
                {
                    StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\RecuperaSenha_EnviarEmailConfirmacao2.log");
                    sw.WriteLine("mail:{0}",email);
                    sw.WriteLine("sql:{0}", sql);
                    sw.Close();
                }
            }
            return r;
        }

        public bool GravarSenha(string codcli, string senha)
        {
            string sql;
            try
            {
                sql = String.Format("update CADCLI set CELETRON = '{0}' where CCODCLI = {1}", senha, codcli);
                conexao.Executar(sql);
                return true;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists("C:\\SGDAT\\Log"))
                {
                    Directory.CreateDirectory("C:\\SGDAT\\Log");
                }
                StreamWriter escritor = new StreamWriter("C:\\SGDAT\\Log\\RecuperaSenha_GravarSenha.log");
                escritor.WriteLine(ex.Message);
                escritor.WriteLine("string de conexão: " + conexao.ConOra.ConnectionString);
                escritor.Close();
                escritor.Dispose();

                return false;
            }
        }
    }
}
