using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ZP.Lib.Web;
using ZProRx.Test.Web.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZProRx.Test.Web.Controllers
{
    [Route("api/[controller]")]
    public class TestNacosConfigsController : Controller
    {
        // IOptions<TestConfig>
        IConfiguration configuration;
        public TestNacosConfigsController(IConfiguration configuration, IOptions<TestConfig> options)
        {
            this.configuration = configuration;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            var testConfig = new TestConfig();

            var section = configuration.GetSection("TestConfig");
            section.Bind(testConfig);



            return ZPropertyNetCore.ZsonOkResult();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {

            return "value";
        }
    }
}
