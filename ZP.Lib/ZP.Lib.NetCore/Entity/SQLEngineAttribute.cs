using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Server.SQL;

namespace ZP.Lib.NetCore.Entity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SQLEngineAttribute : Attribute
    {
        public Type EngineType { get; private set; }
        public SQLEngineAttribute(Type engineType)
        {
            this.EngineType = engineType;
        }

        public SQLEngineAttribute(string strEngineType)
        {
            var str = typeof(ZPropertySQLite);
            this.EngineType = Type.GetType($"ZP.Lib.Server.SQL.ZProperty{strEngineType}");
        }
    }
}
