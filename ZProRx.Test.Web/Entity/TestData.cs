using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZP.Lib;
using ZP.Lib.Net;

namespace ZProRx.Test.Web.Entity
{
    public enum TestErrorEnum
    {
        //must start form ZNetErrorEnum.MaxError
        BaseError = ZNetErrorEnum.MaxError + 0x100,
        Error1,
        Error2,
        Error3,
        Error4,
    }

    public class TestData
    {
        public ZProperty<string> testData = new ZProperty<string>();
        public ZProperty<int> testNum = new ZProperty<int>();
    }
}
