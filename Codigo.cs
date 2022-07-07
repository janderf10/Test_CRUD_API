using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Trial_Stef2
{
    public static class Codigo
    {
        //Metodo serve para verificar se a entrada tem todos os campos obrigatorios para a operacao
        public static JObject ValidarInput(string inputJSON, string[] obrigatorioKeys, out string erro)
        {
            try
            {
                JObject obj = JObject.Parse(inputJSON);

                //Checando se todos os campos obrigatorios estao presentes no JSON input 
                foreach (string obgKey in obrigatorioKeys)
                {
                    if (!obj.ContainsKey(obgKey))
                    {
                        throw new Exception("Erro: Faltando campo obrigatório");
                    }
                }
                erro = "";
                return obj;
            }
            catch (Exception e)
            {
                erro = e.Message;
                return null;
            }
        }

        //Metodos para criacao de registros
        public static string CriarSQLInsert(JObject inputJSON,string nomeTabela,Dictionary<string,string> relacaoCampoJSON_SQL,out string erro)
        {
            try
            {

                string insertPart1 = "INSERT INTO " + nomeTabela + " (";
                string insertPart2 = "VALUES (";
                bool algumCampo = false;
                foreach (KeyValuePair<string,string> campo in relacaoCampoJSON_SQL)
                {

                    if (inputJSON.ContainsKey(campo.Key))
                    {
                        algumCampo = true;
                        insertPart1 += campo.Value + ",";
                        insertPart2 += inputJSON.Value<string>(campo.Key) + ",";

                    }
                }

                if (!algumCampo)
                {
                    throw new Exception("Nenhum campo foi encontrado");
                }

                insertPart1.Remove(insertPart1.Count() - 1);
                insertPart1 += ")";

                insertPart2.Remove(insertPart2.Count() - 1);
                insertPart2 += ")";

                string insertFinal = insertPart1 + " " + insertPart2;
                erro = "";
                return insertFinal;

            }
            catch (Exception e)
            {
                erro = e.Message;
                return "";
            }
        }

        public static string ExecuteSQLNonQuery(string inputSQL, SQLOperacao operacao, string fonteDados, string database, string usuario, string password, out string erro)
        {
            try
            {
                string resposta;

                SqlCommand comando;
                SqlDataAdapter adaptador = new SqlDataAdapter();

                string connectionString = @"Data Source=" + fonteDados + ";Initial Catalog=" + database + ";User ID=" + usuario + ";Password=" + password + ";";


                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();

                    comando = new SqlCommand(inputSQL, cnn);
                    adaptador.InsertCommand = comando;
                    int rows = adaptador.InsertCommand.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        string preResp = "";
                        switch (operacao)
                        {
                            case SQLOperacao.Insert:
                                preResp = "inserido";
                                break;
                            case SQLOperacao.Update:
                                preResp = "modificado";
                                break;
                            case SQLOperacao.Delete:
                                preResp = "deletado";
                                break;

                        }
                        resposta = "Registro " + preResp + " com sucesso";
                    }

                    else
                        throw new Exception("Erro: Nao foi possivel executar operacao");

                    comando.Dispose();
                }

                erro = "";
                return resposta;
            }
            catch (Exception e)
            {
                erro = e.Message;
                return "Erro: "+ erro;
            }
        }

        //Metodos para consulta de registros
        public static string CriarSQLSelect(JObject inputJSON, string nomeTabela, Dictionary<string, string> relacaoCampoJSON_SQL, out string erro)
        {
            try
            {

                string selectPart1 = "SELECT *";
                string selectPart2 = "FROM "+ nomeTabela;
                string selectPart3 = "WHERE ";
                bool algumCampo = false;
                foreach (KeyValuePair<string, string> campo in relacaoCampoJSON_SQL)
                {

                    if (inputJSON.ContainsKey(campo.Key))
                    {
                        algumCampo = true;
                        JToken data = inputJSON[campo.Key];
                        if (data.Type==JTokenType.String)
                            selectPart3 += campo.Value + " = '" + data + "' AND";
                        else if (data.Type == JTokenType.Float || data.Type == JTokenType.Integer)
                            selectPart3 += campo.Value + " = "+ data + "AND";

                    }
                }

                if (!algumCampo)
                {
                    throw new Exception("Nenhum campo foi encontrado");
                }

                selectPart3.Remove(selectPart3.Count() - 1);


                erro = "";
                return selectPart1 + " " + selectPart2 + " " + selectPart3;

            }
            catch (Exception e)
            {
                erro = e.Message;
                return "";
            }
        }

        public static string ExecuteSQLQuery(string inputSQL,int numCols, string fonteDados,string database,string usuario,string password, out string erro)
        {
            try
            {
                string resposta="";

                SqlCommand comando;

                string connectionString = @"Data Source=" + fonteDados + ";Initial Catalog=" + database + ";User ID=" + usuario + ";Password=" + password + ";";


                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();

                    comando = new SqlCommand(inputSQL, cnn);
                    SqlDataReader reader = comando.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        for (int i=0;i< numCols;i++)
                        {
                            resposta += reader.GetValue(i);
                        }

                    }
                    reader.Close();
                    comando.Dispose();
                }

                erro = "";
                return resposta;
            }
            catch (Exception e)
            {
                erro = e.Message;
                return "Erro: Nao foi possivel consultar os dados";
            }
        }

        //Metodos para modificacao de registros

        public static string CriarSQLUpdate(JObject inputJSON, string nomeTabela, Dictionary<string, string> relacaoCampoJSON_SQL, out string erro)
        {
            try
            {

                string insertPart1 = "UPDATE " + nomeTabela + " (";
                string insertPart2 = "VALUES (";
                bool algumCampo = false;
                foreach (KeyValuePair<string, string> campo in relacaoCampoJSON_SQL)
                {

                    if (inputJSON.ContainsKey(campo.Key))
                    {
                        algumCampo = true;
                        insertPart1 += campo.Value + ",";
                        insertPart2 += inputJSON.Value<string>(campo.Key) + ",";

                    }
                }

                if (!algumCampo)
                {
                    throw new Exception("Nenhum campo foi encontrado");
                }

                insertPart1.Remove(insertPart1.Count() - 1);
                insertPart1 += ")";

                insertPart2.Remove(insertPart2.Count() - 1);
                insertPart2 += ")";

                string insertFinal = insertPart1 + " " + insertPart2;
                erro = "";
                return insertFinal;

            }
            catch (Exception e)
            {
                erro = e.Message;
                return "";
            }
        }

        //Metodo para remocao de registros

        public static string CriarSQLDelete(JObject inputJSON, string nomeTabela, Dictionary<string, string> relacaoCampoJSON_SQL, out string erro)
        {
            try
            {

                string deletePart1 = "DELETE FROM " + nomeTabela;
                string deletePart2 = "WHERE ";
                bool algumCampo = false;
                foreach (KeyValuePair<string, string> campo in relacaoCampoJSON_SQL)
                {

                    if (inputJSON.ContainsKey(campo.Key))
                    {
                        algumCampo = true;
                        deletePart2 += campo.Value + " = "+ inputJSON[campo.Key]+ ",";

                    }
                }

                if (!algumCampo)
                {
                    throw new Exception("Nenhum campo foi encontrado");
                }

                deletePart2.Remove(deletePart2.Count() - 1);

                erro = "";
                return deletePart1 + " "+ deletePart2;

            }
            catch (Exception e)
            {
                erro = e.Message;
                return "";
            }
        }

        //Metodo para preparar a saida em JSON
        public static string PrepareOutputJSON(bool isError,string content)
        {
            try
            {
                JObject output = new JObject();
                if (isError)
                {
                    output.Add("Status", "Error");
                }
                else
                    output.Add("Status", "Success");
                output.Add("Data",content);
                return output.ToString();
            }
            catch
            {
                return "Erro ao gerar JSON de saida";
            }
        }



        


    }

    public enum SQLOperacao
    {
        Insert,
        Select,
        Update,
        Delete
    }
}
