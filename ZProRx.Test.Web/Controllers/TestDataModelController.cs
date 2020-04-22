using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.Web;
using ZProRx.Test.Web.Entity;
using ZProRx.Test.Web.Models;

namespace ZProRx.Test.Web.Controllers
{
    [Route("api/v1/testdata/[controller]")]
    [ApiController]
    public class TestDataModelController : ControllerBase
    {

        TestDatasModel testDatasModel = null;

        ILogger<TestDataModelController> logger = null;

        public TestDataModelController(IModelsProvider modelsProvider, ILogger<TestDataModelController> logger)
        {
            this.logger = logger;
            testDatasModel = modelsProvider.GetModel<TestDatasModel>();

            testDatasModel.CheckOrCreateDB();

            testDatasModel.Connect();
        }

        [HttpGet("{tpid}")]
        public IActionResult Get(int tpid)
        {
            var ret = testDatasModel.GetData(tpid);

            logger.LogWarning("Test");

            Debug.Log("Debug:Test");

            return ZPropertyNetCore.ZsonOkResult(ret);
        }

        [HttpPost()]
        public IActionResult Add()
        {
            var newData = ZPropertyMesh.CreateObject<TestData>();
            newData.testData.Value = "加油";
            var ret = testDatasModel.AddData(newData);
            return ZPropertyNetCore.ZsonOkResult(ret);
        }

        /*
        {
            "TestData.testNum": 4,
            "TestData.testData": "Test111"
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tpid"></param>
        /// <param name="newData"></param>
        /// <returns></returns>
        [HttpPut("{tpid}")]
        public IActionResult Put(uint tpid, [FromBody] TestData newData)
        {
            var ret = testDatasModel.Update(tpid, newData);
            return ZPropertyNetCore.ZsonOkResult(ret);
        }

        [HttpDelete("{tpid}")]
        public IActionResult Delete(uint tpid)
        {
            var ret = testDatasModel.Delete(tpid);
            return ZPropertyNetCore.ZsonOkResult(ret);
        }
    }
}