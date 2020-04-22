using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZP.Lib.NetCore.Entity;
using ZP.Lib.Server.SQL;
using ZP.Lib.Web;
using ZProRx.Test.Web.Entity;

namespace ZProRx.Test.Web.Models
{

    //[SQLEngine("SQLite")]
    [SQLEngine(typeof(ZPropertySQLite))]
    public class TestDatasModel : BaseModel<TestData>
    {
        //private static string TableName = "TestDatas";

        ILogger<TestDatasModel> logger;
        public TestDatasModel(IConfiguration configuration, ILogger<TestDatasModel> logger) : base(configuration, "matrix")
        {
            this.logger = logger;
        }

        public TestData GetData(int tpid)
        {            
            var ret = sqlCtl.Query<TestData>($"SELECT * FROM {this.FullTableName};").FirstOrDefault();

            return ret;
        }

        public uint AddData(TestData data)
        {
            return sqlCtl.Insert<TestData>(this.FullTableName, data);
        }
    }
}
