using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZP.Lib;
using ZP.Lib.Web;
using ZProRx.Test.Web.Entity;

namespace ZProRx.Test.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TestErrorController : ControllerBase
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
            return ZPropertyNetCore.ZsonErrorResult(TestErrorEnum.Error1);
        }

        // POST api/<controller>
        [HttpPost("{id}")]
        public IActionResult Post(int id, [FromBody]TestData value)
        {
            return ZPropertyNetCore.ZsonErrorResult(TestErrorEnum.Error1);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TestData value)
        {
            return ZPropertyNetCore.ZsonErrorResult(TestErrorEnum.Error1);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return ZPropertyNetCore.ZsonErrorResult(TestErrorEnum.Error1);
        }
    }
}