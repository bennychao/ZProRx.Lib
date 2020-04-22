using System;
using ZP.Lib;

namespace ZP.Lib.Net
{
    public class ZEndPoint
    {
        private ZProperty<string> host = new ZProperty<string>();

        private ZProperty<int> port = new ZProperty<int>();
        private ZProperty<string> ip = new ZProperty<string>();

        public int Port
        {
            get => port.Value;
            set => port.Value = value;
        }

        public string IP
        {
            get => ip.Value;
            set => ip.Value = value;
        }


        public string URL => "http://" + IP + ":" + Port;

        public string URLS => "https://" + IP + ":" + Port;

        public string Host => host.Value;

        public static ZEndPoint LocalHost 
        {
            get {
                var ret = ZPropertyMesh.CreateObject<ZEndPoint>();
                ret.host.Value = "localhost";
                ret.ip.Value = "localhost";
                ret.port.Value = 5000;
                return ret;
            }
        }

    }
}
