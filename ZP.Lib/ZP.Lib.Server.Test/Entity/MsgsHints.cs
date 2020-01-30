using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Server.Test.Entity
{
    [PropertyUIItemResClass("Hints/", "TestHint2")]
    public class ZTestHint : ZHint
    {

    }

    public class MsgsHints
    {
        public ZMsgList MsgList = new ZMsgList();

        public ZHintList HintList = new ZHintList();
    }
}
