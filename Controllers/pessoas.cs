using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Trial_Stef2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class pessoas : ControllerBase
    {

        // GET api/<pessoas>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            try
            {
                string erro1,erro2;
                JObject input = new JObject() { "Id", id.ToString() };
                string query = Codigo.CriarSQLSelect(input, "Pessoa", new Dictionary<string, string>() { { "Id", "Id" } }, out erro1);
                string response = Codigo.ExecuteSQLQuery(query, 5, "", "", "", "",out erro2);

                return Codigo.PrepareOutputJSON(false,response);
            }
            catch (Exception e)
            {
                return Codigo.PrepareOutputJSON(true,"Erro "+ e.Message); ;
            }

        }

        // POST api/<pessoas>
        [HttpPost]
        public string Post([FromBody] string inputJSON)
        {
            try
            {
                string[] fields = { "Nome", "CPF", "Idade", "Cidade", "UF" };
                
                string erro1;
                JObject JSONObj = Codigo.ValidarInput(inputJSON, fields, out erro1);

                if (erro1 == "")
                {
                    Dictionary<string, string> camposCidade = new Dictionary<string, string>() {
                    { "Cidade", "CidadeNome"},
                     { "UF", "CidadeUF"} };
                    JObject novaCidade = new JObject();
                    novaCidade.Add("Cidade", JSONObj["Cidade"]);
                    novaCidade.Add("UF", JSONObj["UF"]);
                    //Procurando cidade mencionada
                    string erroCidade1;
                    string a = Codigo.CriarSQLSelect(novaCidade, "Cidade", camposCidade, out erroCidade1);
                    //Se nao encontrada, criar cidade

                    string erroCidade2;
                    string b = Codigo.CriarSQLInsert(novaCidade, "Cidade", camposCidade, out erroCidade2);


                    Dictionary<string, string> relacaoCampoJSON_SQL = new Dictionary<string, string>() {
                    { "Nome", "PessoaNome"},
                     { "CPF", "PessoaCPF"},
                      { "Idade", "PessoaIdade"},
                    { "Id_Cidade", "Id_Cidade"}
                };

                    string erro2;
                    string SQLscript = Codigo.CriarSQLInsert(JSONObj, "Pessoa", relacaoCampoJSON_SQL, out erro2);

                    string erro3;
                    string resposta = Codigo.ExecuteSQLNonQuery(SQLscript,SQLOperacao.Insert, "","","","", out erro3);

                    return Codigo.PrepareOutputJSON(false,resposta);
                }
                else
                    return Codigo.PrepareOutputJSON(true, "Entrada Invalida");
            }
            catch (Exception e)
            {
                return Codigo.PrepareOutputJSON(true, "Erro. Detalhes: "+e.Message);
            }
        }

        // PUT api/<pessoas>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<pessoas>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
