using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Net;

namespace ZP.Lib.Matrix.Test.Entity
{
    public enum TestCmd
    {
        Command1,
        Command2,
        Command3,
        Command4
    }


    public enum TestErrorEnum
    {
        BaseError = ZNetErrorEnum.MaxError + 0x100,
        Error1,
        Error2,
        Error3,
        Error4,
    }

    public class TestRet
    {
        public ZProperty<int> zProperty = new ZProperty<int>();
    }
    
    public class TestPack
    {
        public ZProperty<int> zProperty = new ZProperty<int>();
    }

    public class TestResponseRet
    {
        public ZProperty<int> zPropertyRet = new ZProperty<int>();
    }

    public class TestResponseMsg
    {
        public ZProperty<string> msg = new ZProperty<string>();
    }

    public class TestData
    {
        public ZProperty<string> testData = new ZProperty<string>();
        public ZProperty<int> testNum = new ZProperty<int>();
    }
}
