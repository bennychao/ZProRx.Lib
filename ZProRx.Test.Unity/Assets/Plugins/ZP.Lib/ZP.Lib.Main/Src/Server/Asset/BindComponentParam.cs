using System;
namespace ZP.Lib
{
    internal class BindComponentParam
    {
        public ZProperty<string> Name = new ZProperty<string>();

        //now only support string
        public ZProperty<string> Data = new ZProperty<string>();

        //for com type
        public ZProperty<IRawDataPref> RawData = new ZProperty<IRawDataPref>();

        public BindComponentParam()
        {
        }
    }
}
