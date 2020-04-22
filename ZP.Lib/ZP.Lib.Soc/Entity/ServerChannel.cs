using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc;

namespace ZP.Lib.Soc.Entity
{
    public class ServerChannel  : BaseChannel
    {
        protected string clientChannelName;
        protected readonly IRoomServer roomServer = null;

        public int ClientCount => clientIds.Count();

        public ServerChannel(IRoomServer roomServer, string clientName)
        {
            this.clientChannelName = clientName;
            this.roomServer = roomServer;
        }

        public string removeSub(string strdata, string removetag)
        {
            //var inde = strdata.IndexOf(removetag);
            return strdata.Replace(removetag + "/", "");
        }
            
        public string replaseSub(string strdata, string removetag, string targettag)
        {
            //var inde = strdata.IndexOf(removetag);
            return strdata.Replace(removetag, targettag);
        }

        protected string GetClientBaseUrl()
        {
            return GetBaseUrlPrefix() + clientChannelName + "/";
        }
    }
}
