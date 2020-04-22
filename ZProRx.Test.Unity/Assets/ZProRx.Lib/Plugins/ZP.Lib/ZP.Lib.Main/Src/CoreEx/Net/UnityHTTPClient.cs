using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Net
{
#if !ZP_SERVER
    internal class UnityHTTPClient : INetHttpEngine
    {
        public IObservable<string> Delete(string url, Dictionary<string, string> headers = null)
        {
            return UnityWebRequestObservable.Delete(url, headers);
        }

        public IObservable<string> Get(string url, Dictionary<string, string> headers = null)
        {
            return ObservableWWW.Get(url, headers);
        }

        public IObservable<string> Post(string url, object obj, Dictionary<string, string> headers = null)
        {
            var data = ZPropertyPrefs.ConvertToStr(obj);
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            return ObservableWWW.Post(url, byteData, headers);
        }

        public IObservable<string> Post(string url, string data, Dictionary<string, string> headers = null)
        {
            //var data = obj != null ? ZPropertyPrefs.ConvertToStr(obj) : " ";

            byte[] byteData = Encoding.UTF8.GetBytes(data);

            return ObservableWWW.Post(url, byteData, headers);
        }

        public IObservable<string> Put(string url, object obj, Dictionary<string, string> headers = null)
        {
            var data = ZPropertyPrefs.ConvertToStr(obj);

            byte[] byteData = Encoding.UTF8.GetBytes(data);

            //request = ObservableWWW.Post(ip + strQuery, byteData, headers);

            return UnityWebRequestObservable.Put(url, data, headers);
        }
    }
#endif

}
