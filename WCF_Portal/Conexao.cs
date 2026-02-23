using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace WCF_Portal
{
    public class Conexao
    {
        string connStr;
        string oraUsr;
        string oraSvr;
        string log = string.Empty;

        OracleConnection conexao = new OracleConnection();
        OracleCommand comando = new OracleCommand();

        public OracleConnection ConOra { get { return conexao; } }
        public string strConOra { get { return connStr; } }
        public int codEmp { get; set; }

        public Conexao()
        {
            AbrirConexao();
        }
                
        public void InicioConexao()
        {
            oraUsr = "REC1";
            oraSvr = "REC1";            
            try
            {
                //string ini = "C:\\SGDAT\\webservices.ini";
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string ini = Path.Combine(appPath, "Oracle.ini");
                log = ini;
                if (File.Exists(ini))
                {
                    connStr = GetConnectionStringFromIni(ini);
                    /*
                    StreamReader sr = new StreamReader(ini);
                    string maux;
                    maux = sr.ReadLine();
                    //maux = sr.ReadToEnd();
                    sr.Close();
                    log += "\n\r Conteudo: " + maux;
                                        
                    if (maux.Trim().Equals("TS"))
                    {
                        ini = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                        ini = Path.Combine(ini, "webservices.ini");
                        log += "\n\r " + ini;
                        if (File.Exists(ini))
                        {
                            sr = new StreamReader(ini);
                            maux = sr.ReadLine();
                            sr.Close();
                            log += "\n\r Conteudo: " + maux;
                        }
                    }

                    if (maux.Length > 10)
                    {
                        oraUsr = maux.Substring(0, 10).Trim();
                        oraSvr = maux.Substring(10).Trim();
                    }*/
                }
                else
                {
                    connStr = "Data Source=" + oraSvr + ";Persist Security Info=True;User ID=" + oraUsr + ";Password=a";
                }
            }
            catch
            {
                oraUsr = "REC1";
                oraSvr = "REC1";
                connStr = "Data Source=" + oraSvr + ";Persist Security Info=True;User ID=" + oraUsr + ";Password=a";
            }
            // connStr = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=SERVORA)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User ID=REC1_TESTE;Password=a";
        }

        private string GetConnectionStringFromIni(string iniFilePath)
        {
            // Dicionário para armazenar os valores do arquivo .ini
            var iniData = new Dictionary<string, string>();
            string section = null;

            int codEmpresa = 1;

            // Lê todas as linhas do arquivo
            foreach (var line in File.ReadAllLines(iniFilePath))
            {
                var trimmedLine = line.Trim();
                // Identifica a seção (ex.: [OracleConnection])
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    section = trimmedLine.Substring(1, trimmedLine.Length - 2);
                }
                // Lê os pares chave=valor dentro da seção
                else if (!string.IsNullOrEmpty(trimmedLine) && section != null)
                {
                    var parts = trimmedLine.Split('=');
                    if (parts.Length == 2)
                    {
                        iniData[$"{section}.{parts[0].Trim()}"] = parts[1].Trim();
                    }
                }
            }

            // Extrai os valores do dicionário
            string host = iniData["OracleConnection.HOST"];
            string sid = iniData["OracleConnection.SID"];
            string port = iniData["OracleConnection.PORT"];
            string user = iniData["OracleConnection.USER"];
            int.TryParse(iniData["OracleConnection.CODEMP"], out codEmpresa);

            codEmp = codEmpresa;

            // Monta a string de conexão
            return $"User Id={user};Password=a;Data Source={host}:{port}/{sid}";   // $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SID={sid})));User Id={user};Password=a";
        }

        public bool ConexaoAberta
        {
            get
            {
                return (conexao.State == ConnectionState.Open);
            }
        }

        public void AbrirConexao()
        {
            InicioConexao();
            try
            {
                if (conexao.State == ConnectionState.Open)
                {
                    conexao.Close();
                }
                else
                {
                    conexao.ConnectionString = connStr;
                    conexao.Open();
                }
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("C:\\SGDAT\\Log\\Conexao_PS.log");
                sw.WriteLine(log);
                sw.WriteLine($"connectionString: {connStr}");
                sw.WriteLine(ex.Message);
                sw.Close();
                sw.Dispose();
                conexao.Close();
            }
        }

        public void FecharConexao()
        {
            if (conexao.State == ConnectionState.Open)
            {
                conexao.Close();
            }
        }

        public OracleDataReader FazerLeitura(string select)
        {
            if (conexao.State != ConnectionState.Open)
            {
                AbrirConexao();
            }
            if (comando != null)
            {
                comando.Dispose();
                comando = conexao.CreateCommand();
            }
            comando.Connection = conexao;
            comando.CommandType = CommandType.Text;
            comando.CommandText = select;
            OracleDataReader resultado = comando.ExecuteReader();
            return resultado;
        }

        public object Resultado(string select, OracleParameter[] par = null)
        {
            if (conexao.State != ConnectionState.Open)
            {
                AbrirConexao();
            }
            if (comando != null)
            {
                comando.Dispose();
                comando = conexao.CreateCommand();
            }
            comando.Connection = conexao;
            comando.CommandType = CommandType.Text;
            comando.CommandText = select;

            if (par != null)
            {
                foreach (OracleParameter parametro in par)
                {
                    OracleParameter p = new OracleParameter(parametro.ParameterName, parametro.Value);
                    comando.Parameters.Add(p);
                    //comando.Parameters.Add(parametro);  // Com esta condicao vai dar erro - "Outro OracleParameterCollection já contém OracleParameter"
                }
            }

            return comando.ExecuteScalar();
        }

        public int ResultadoInteiro(string sql, OracleParameter[] par = null)
        {
            int r = 0;
            try
            {
                r = Convert.ToInt32(Resultado(sql, par));
            }
            catch
            {
                r = 0;
            }
            return r;
        }

        public long ResultadoLongo(string sql, OracleParameter[] par = null)
        {
            long r = 0;
            try
            {
                r = Convert.ToInt64(Resultado(sql, par));
            }
            catch
            {
                r = 0;
            }
            return r;
        }

        public string ResultadoTexto(string sql, OracleParameter[] par = null)
        {
            string r = "";
            try
            {
                r = Resultado(sql, par).ToString();
            }
            catch
            {
                r = "";
            }
            return r;
        }

        public int Executar(string sql, OracleParameter[] par = null)
        {
            if (conexao.State != ConnectionState.Open)
            {
                AbrirConexao();
            }
            if (comando != null)
            {
                comando.Dispose();
                comando = conexao.CreateCommand();
            }
            comando.Connection = conexao;
            comando.CommandType = CommandType.Text;
            comando.CommandText = sql;

            if (par != null)
            {
                foreach (OracleParameter parametro in par)
                {
                    OracleParameter p = new OracleParameter(parametro.ParameterName, parametro.Value);
                    comando.Parameters.Add(p);
                    //comando.Parameters.Add(parametro);  // Com esta condicao vai dar erro - "Outro OracleParameterCollection já contém OracleParameter"
                }
            }

            return comando.ExecuteNonQuery();
        }

        public string DadosPadrao(string mForm, string mCampo, string mDefault)
        {
            object rs;
            string msql, r;

            msql = "select PP_VALOR from PARPADRAO"
                  + " where PP_FORM = '" + mForm + "'"
                  + "   and PP_CAMPO = '" + mCampo + "'";
            try
            {
                rs = ResultadoTexto(msql);
                if (rs == null)
                {
                    r = mDefault;
                }
                else
                {
                    r = Convert.ToString(rs);
                }
            }
            catch
            {
                r = mDefault;
            }
            return r;
        }

        public DataSet FazerLeituraDataSet(string select)
        {
            if (conexao.State != ConnectionState.Open)
            {
                AbrirConexao();
            }
            if (comando != null)
            {
                comando.Dispose();
                comando = conexao.CreateCommand();
            }
            comando.Connection = conexao;
            comando.CommandType = CommandType.Text;
            comando.CommandText = select;
            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter(comando);
            da.Fill(ds);
            return ds;
        }
    }
}