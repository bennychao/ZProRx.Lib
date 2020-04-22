using System;
using ZP.Lib;

namespace ZP.Lib.Soc.Entity
{
    public class SocRawPackage
    {
        private ZProperty<string> clientId = new ZProperty<string>();

        private ZProperty<string> template = new ZProperty<string>();

        private ZProperty<string> msg = new ZProperty<string>();

        public SocRawPackage(string template, string msg, string clientId)
        {
            this.template.Value = template;
            this.msg.Value = msg;
            this.clientId.Value = clientId;
        }
    }
}
