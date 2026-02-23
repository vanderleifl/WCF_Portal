using System;
using System.Linq;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Net;
using System.Net.Http;

namespace WCF_Portal
{
    public class Func
    {
        public bool Autorizado(long cnpj, string servico)
        {
            bool r = false;
            string log = "";
            string[] url = {
                "portalseller.com.br/Clientes.txt",
                "sgdat.com/Clientes.txt",
                "portalseller.com/Clientes.txt",
                "www.portalseller.com.br/Clientes.txt",
                "www.sgdat.com/Clientes.txt",
                "https://portalseller.com.br/Clientes.txt",
                "http://portalseller.com.br/Clientes.txt",
                "http://portalseller.com/Clientes.txt",
                "http://sgdat.com/Clientes.txt",
                "https://sgdat.com/Clientes.txt",
                "http://portalseller.com/Clientes.txt" 
            };
            string result = "";

            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
                        
            /*
            WebClient client = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
            client.Headers.Add("Cache-Control", "no-cache");
            client.Encoding = Encoding.UTF8;
            */

            for (int i = 0; i < url.Count(); i++)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Faz a solicitação GET para a URL
                        HttpResponseMessage response = client.GetAsync(new Uri(url[i], UriKind.Absolute)).Result;
                        log += Environment.NewLine + $"Código de status: {response.StatusCode}";

                        // Verifica se a solicitação foi bem-sucedida (código de status 200)
                        if (response.IsSuccessStatusCode)
                        {
                            // Lê o conteúdo da resposta como uma string
                            result = response.Content.ReadAsStringAsync().Result;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                catch (WebException ex)
                {
                    log += Environment.NewLine + $"URL {i} ({url[i]}): {ex.Message} - result: {result}";
                    continue;
                }
                catch (Exception ex)
                {
                    log += Environment.NewLine + $"URL {i} ({url[i]}): {ex.Message} - result: {result}";
                    continue;
                }
                break;
            }

            if (string.IsNullOrEmpty(result))
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\_func_Autorizado_WCF_Portal_1.log");
                sw.WriteLine(log);
                sw.Close();
                sw.Dispose();

                return false;
            }

            try
            {
                string servicos = "";
                int pos = result.IndexOf(cnpj.ToString("00000000000000"));
                if (pos >= 0)
                {
                    int tam = 14;
                    if (result.Length >= 23)
                    {
                        tam = 9;
                    }
                    else if (result.Length >= 20)
                    {
                        tam = 6;
                    }
                    else if (result.Length >= 17)
                    {
                        tam = 3;
                    }
                    pos += 14;
                    servicos = result.Substring(pos, tam).Trim();
                    r = servico.Contains(servico);
                }
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(ObterArquivoUnico("Autorizacao", ".log"));
                sw.WriteLine(ex.Message);
                sw.Close();
                sw.Dispose();
            }

            return r;
        }

        public string ObterArquivoUnico(string nome, string ext)
        {
            string pasta = "C:\\IIS\\ServicePortal\\logs";
            string s = "";
            string _nome = nome;
            double n = 0;
            double n2 = 0;

            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }

            while (n2 < 99999999)
            {
                n++;
                if (n >= 99999999)
                {
                    n2++;
                    _nome += "_bkp" + n2.ToString();
                    n = 1;
                }
                s = pasta + "\\" + _nome + n.ToString("0000000000") + ext;
                if (File.Exists(s))
                {
                    continue;
                }
                break;
            }
            return s;
        }

        public string Substituir(string texto, string de, string e, string para, string epara)
        {
            string r = "";
            for (int i = 0; i < texto.Length; i++)
            {
                if (texto.Substring(i, 1) == de)
                {
                    r += para;
                }
                else if (texto.Substring(i, 1) == e)
                {
                    r += epara;
                }
                else
                {
                    r += texto.Substring(i, 1);
                }
            }
            return r;
        }

        // Função para Converter para Inteiro
        public int ParaInteiro(object valor)
        {
            int r;
            try
            {
                r = Convert.ToInt32(valor);
            }
            catch
            {
                r = 0;
            }
            return r;
        }

        // Função para Converter para Inteiro Longo
        public long ParaLongo(object valor)
        {
            long r;
            try
            {
                r = Convert.ToInt64(valor);
            }
            catch
            {
                r = 0;
            }
            return r;
        }

        // Função para Converter para Double
        public double ParaDouble(object valor)
        {
            double r;
            try
            {
                r = Convert.ToDouble(valor);
            }
            catch
            {
                r = 0;
            }
            return r;
        }

        // Função para Converter para string
        public string ParaString(object valor)
        {
            string r;
            try
            {
                r = valor.ToString();
            }
            catch
            {
                r = "";
            }

            if (r == "(null)")
            {
                r = "";
            }

            return r;
        }

        // Função para Converter para Data
        public DateTime ParaData(object valor)
        {
            DateTime r;
            try
            {
                string s = ParaString(valor);
                int dia, mes, ano;
                ano = 0;
                if (s.Length > 10)
                {
                    s = s.Substring(0, 10);
                }
                else if (s.Length < 8)
                {
                    return DateTime.MinValue;
                }
                else if (s.Length < 10)
                {
                    ano = ParaInteiro(s.Substring(0, 2));
                    s = s.Substring(3);
                }
                else
                {
                    ano = ParaInteiro(s.Substring(0, 4));
                    s = s.Substring(5);
                }
                mes = ParaInteiro(s.Substring(0, 2));
                dia = ParaInteiro(s.Substring(3, 2));
                r = new DateTime(ano, mes, dia);
            }
            catch
            {
                r = DateTime.MinValue;
            }
            return r;
        }

        // Função que retorna ponto no lugar de vírgula
        public string MudaVP(string wTexto)
        {
            string t = "0";
            int x, y;
            if (String.IsNullOrEmpty(wTexto))
            {
                return t;
            }
            y = wTexto.Length - 1;
            for (x = 0; x <= y; x++)
            {
                if (wTexto.Substring(x, 1) == ",")
                {
                    t += ".";
                }
                else
                {
                    t += wTexto.Substring(x, 1);
                }
            }
            return t;
        }

        public long Sequencia(OracleConnection oc, string NomeSeq)
        {
            long mseq;
            try
            {
                mseq = Convert.ToInt64(ResultadoSQL(oc, "select " + NomeSeq + ".nextval from dual"));
            }
            catch
            {
                mseq = 1;
                try
                {
                    ExecutarSQL(oc, "create sequence " + NomeSeq + " increment by 1 start with 1 nocache");
                    mseq = Convert.ToInt64(ResultadoSQL(oc, "select " + NomeSeq + ".nextval from dual"));
                }
                catch (Exception ex)
                {
                    StreamWriter sw = new StreamWriter("C:\\IIS\\ServicePortal\\logs\\_func_sequencia.log");
                    sw.WriteLine(ex.Message);
                    sw.Close();
                    sw.Dispose();
                }
            }

            return mseq;
        }

        public object ResultadoSQL(OracleConnection oc, string sql)
        {
            object resultado;
            OracleCommand cmd = oc.CreateCommand();
            cmd.CommandText = sql;
            try
            {
                resultado = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\IIS\\ServicePortal\\logs\\_func_ResultadoSQL.log");
                sw.WriteLine(ex.Message);
                sw.WriteLine(sql);
                sw.Close();
                sw.Dispose();
                resultado = null;
            }
            return resultado;
        }

        public void ExecutarSQL(OracleConnection oc, string sql)
        {
            // Conexão Oracle
            OracleCommand cmd = oc.CreateCommand();
            cmd = oc.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(ObterArquivoUnico("_func_ExecutarSQL", ".log"));
                sw.WriteLine(ex.Message);
                sw.WriteLine(sql);
                sw.Close();
                sw.Dispose();
            }
        }
    }
}