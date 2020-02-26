using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Test.Entity
{
    public enum TestCmd
    {
        Command1,
        Command2,
        Command3,
        Command4
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

}
