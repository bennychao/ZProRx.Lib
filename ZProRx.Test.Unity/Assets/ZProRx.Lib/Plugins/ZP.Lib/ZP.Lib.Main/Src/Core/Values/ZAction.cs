using System;
using ZP.Lib;

namespace ZP.Lib.Core.Values
{
    public class ZAction
    {
        private ZProperty<string> actionUrl = new ZProperty<string>();

        private ZProperty<string> actionParam = new ZProperty<string>();

        public string ActionUrl => actionUrl.Value;

        public string ActionParam => actionParam.Value;


        public ZAction()
        {
        }
    }
}
