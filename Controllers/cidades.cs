using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test_CRUD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cidades : ControllerBase
    {

        // GET api/<cidades>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<cidades>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<cidades>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<cidades>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
