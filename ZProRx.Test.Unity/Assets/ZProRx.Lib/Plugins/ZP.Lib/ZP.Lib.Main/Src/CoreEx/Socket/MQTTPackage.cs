using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.CoreEx
{
    //not used
    [Obsolete("not used")]
    internal class MQTTResponse : ISocketPackage
    {
        public string Topic => throw new NotImplementedException();
        public string Value { get; set; }
        public string Key => throw new NotImplementedException();
    }
}
