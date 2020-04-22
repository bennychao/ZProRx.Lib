using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Server.SQL;

namespace ZP.Lib.Server.Test.Entity
{
    [DBIndex("tid")]
    internal class TestSQLData
    {
        public ZProperty<int> intData = new ZProperty<int>();
        public ZProperty<string> strData = new ZProperty<string>();
    }
}
