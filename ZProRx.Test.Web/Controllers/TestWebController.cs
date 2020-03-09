using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZP.Lib;
using ZP.Lib.Web;
using ZProRx.Test.Web.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZProRx.Test.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class TestWebController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var ret = ZPropertyMesh.CreateObject<TestData>();
            ret.testNum.Value = 100;
            return ZPropertyNetCore.ZsonOkResult(ret);
        }

        // POST api/<controller>
        [HttpPost("{id}")]
        public void Post(int id, [FromBody]TestData value)
        {

        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TestData value)
        {
            var ret = ZPropertyMesh.CreateObject<TestData>();
            ret.testNum.Value = 100;
            return ZPropertyNetCore.ZsonOkResult(ret);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
