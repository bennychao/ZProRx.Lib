using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib;

namespace ZP.Lib.Web.Entity
{
    public class AppInfo
    {
        private ZProperty<string> app = new ZProperty<string>();

        private ZProperty<string> url = new ZProperty<string>();

        private ZProperty<int> id = new ZProperty<int>();
        private ZProperty<int> port = new ZProperty<int>();

        private Process process = null;

        public int Id => id.Value;

        public bool IsStopApiSupport { get; set; }

        protected string Url => url.Value;


        static public AppInfo CreateInfo(string appName, Process info)
        {
            var ret = ZPropertyMesh.CreateObject<AppInfo>();
            ret.app.Value = appName;
            ret.id.Value = info.Id;
            ret.process = info;

            return ret;
        }

        static public AppInfo CreateInfo(string appName, string url, Process info)
        {
            var ret = ZPropertyMesh.CreateObject<AppInfo>();
            ret.app.Value = appName;
            ret.id.Value = info.Id;
            ret.process = info;
            ret.url.Value = url;

            return ret;
        }

        //static public AppInfo CreateInfo(string appName, Process info)
        //{
        //    var ret = ZPropertyMesh.CreateObject<AppInfo>();
        //    ret.app.Value = appName;
        //    ret.id.Value = info.Id;
        //    ret.process = info;

        //    return ret;
        //}

        public void Kill()
        {
            if (IsStopApiSupport)
            {
                ZPropertyNet.DeleteRaw(Url + "/api/v1/com/system/shutdown", null).Subscribe();

                return;
            }

            try
            {
                if (process?.HasExited == false)
                    process?.Kill();
            }
            catch
            {

            }
            
        }

        public void PostStopApi()
        {
            //TODO get current Ip


        }
    }
}
