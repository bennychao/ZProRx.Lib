using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZP.Lib.Server.SQL;
using ZP.Lib.Server.Test.Entity;

namespace ZP.Lib.Server.SQL.Test
{
    internal class UnitSQL
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestSQLite()
        {
            var sqlCon = new ZPropertySQLite("Data Source=..\\..\\..\\Assets\\test.db");

            sqlCon.Connect();

            if (!sqlCon.HasTable("TestTable"))
            {
                //sqlCon.CreateDB("");

                sqlCon.CreateTable<TestSQLData>("TestTable");//{DbName}.
            }

            var datas = sqlCon.Query<TestSQLData>("SELECT * FROM TestTable;");

            Assert.IsTrue(datas.Count >= 0);

            var newObj = ZPropertyMesh.CreateObject<TestSQLData>();
            newObj.intData.Value = 100;
            newObj.strData.Value = "newData";
            
            var newIndex = sqlCon.Insert<TestSQLData>("TestTable", newObj);

            var sql = $"SELECT * FROM TestTable WHERE tpid = {newIndex};";

            //var insertNewObj = sqlCon.QueryOne<TestSQLData>("TestTable", newIndex);
            var insertNewObj = sqlCon.Query<TestSQLData>(sql)?.FirstOrDefault();
            Assert.IsTrue(insertNewObj != null
                && string.Compare(insertNewObj.strData.Value, newObj.strData.Value) == 0);


        }
    }
}
