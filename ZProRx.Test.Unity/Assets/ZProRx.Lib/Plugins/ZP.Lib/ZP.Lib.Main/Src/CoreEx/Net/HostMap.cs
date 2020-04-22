using System;
using ZP.Lib;

namespace ZP.Lib.Net
{
    
    internal class HostMap : PropObjectSingleton<HostMap>
    {
        private ZPropertyList<ZEndPoint> maps = new ZPropertyList<ZEndPoint>();

        public HostMap()
        {

        }

        public void OnCreate()
        {
#if DEBUG
            maps.Add(ZEndPoint.LocalHost);
#endif
        }

        public  ZEndPoint Find(string hostName)
        {
            return maps.FindValue((arg) => string.Compare(arg.IP, hostName) == 0);
        }

        //for http
        public static string ConvertURL(string url)
        {
            int index = url.IndexOf("//", StringComparison.Ordinal);
   
            var host = url.Substring(0, index);

            if (host.Contains("http"))
                return url;

            var q = url.Substring(index);

            return HostMap.Instance.Find(host)?.URL + q;
        }
    }
}
