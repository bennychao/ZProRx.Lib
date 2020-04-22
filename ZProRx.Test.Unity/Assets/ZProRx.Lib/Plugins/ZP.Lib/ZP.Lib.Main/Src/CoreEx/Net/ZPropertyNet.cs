#if ZP_UNIRX 

using ZP.Lib.Common;
using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;
using ZP.Lib.Net;
using ZP.Lib.CoreEx;
using ZP.Lib.Core.Values;

namespace ZP.Lib
{

    // Net Client  API for Link to Http Server
     public static partial class ZPropertyNet
    {
        //static public string IP = "";
        static private string token = "";

        static private INetHttpEngine httpEngine = null;

        static ZPropertyNet()
        {
#if ZP_SERVER
            httpEngine = HTTPClient.Instance;
#else
            httpEngine = new UnityHTTPClient();
#endif
        }

        static public void ConfigEngine(INetHttpEngine newEngine)
        {
            httpEngine = newEngine;
        }

        //WWW is support https
        static public IObservable<TResult> Login<TResult>(string url, Dictionary<string, string> query = null)
        {
            var retToken = Post<ZToken>(url, query);

            retToken.Subscribe(t =>
            {
                token = t.AccessToken;
            });
            //check query param

            return retToken.Select( t=> t.GetRedirectData < TResult>());
        }

        static public IObservable<ZNull> LogOff(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ConvertToUrlQueryStr(query);

            IObservable<string> request = null;
            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;   //for refresh token

            //headers["Content-Type"] = "application/json;charset=utf-8";

            //#if ZP_SERVER
            //            request = HTTPClient.Instance.Delete(ip + strQuery);
            //#else
            //            request = UnityWebRequestObservable.Delete(ip + strQuery, headers);
            //#endif

            request = httpEngine.Delete(ip + strQuery);

            NetRequestObservable sub = new NetRequestObservable(request);

            sub.Subscribe(_ => ResetToken());
            return sub;
        }

        //get responce contains status and error
        //Get object from netrequest
        static public IObservable<T> Load<T>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ConvertToUrlQueryStr(query);

            //NetRequestObservable<T> sub = null;
            IObservable<string> request = null;
            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            //headers["Content-Type"] = "application/json;charset=utf-8";

            //#if ZP_SERVER

            //            request = HTTPClient.Instance.Get(ip + strQuery);
            //#else
            //             request = ObservableWWW.Get(ip + strQuery, headers);
            //#endif

            request = httpEngine.Get(ip + strQuery);

            NetRequestObservable<T> sub = new NetRequestObservable<T>(request);
            return sub;
        }

        static public IObservable<ZNull> Load(string url, Dictionary<string, string> query = null)
        {

            string strQuery = ConvertToUrlQueryStr(query);

            IObservable<string> request = null;
            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            //headers["Content-Type"] = "application/json;charset=utf-8";

            //#if ZP_SERVER

            //            request = HTTPClient.Instance.Get(ip + strQuery, headers);
            //#else
            //            request = ObservableWWW.Get(ip + strQuery, headers);
            //#endif
            request =  httpEngine.Get(ip + strQuery, headers);


            NetRequestObservable sub = new NetRequestObservable(request);
            return sub;
        }


        static public IObservable<string> Load(string url, Dictionary<string, string> query, Dictionary<string, string> headers)
        {
            string strQuery = ConvertToUrlQueryStr(query);

            IObservable<string> request = null;
            var ip = HostMap.ConvertURL(url);

            //#if ZP_SERVER

            //            request = HTTPClient.Instance.Get(ip + strQuery, headers);
            //#else
            //            request = ObservableWWW.Get(ip + strQuery, headers);
            //#endif
            request = httpEngine.Get(ip + strQuery, headers);

            return request;
        }

        static public IObservable<T> Get<T, TErrorEnum>(string url, Dictionary<string, string> query = null)
        {
            var request = Load<NetPackage<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>>(url, query);

            NetRequestObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>> sub 
                = new NetRequestObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>(request);
            return sub;
        }

        static public IObservable<T> Get<T>(string url, Dictionary<string, string> query = null)
        {
            var request = Load<NetPackage<T, ZNetErrorEnum>>(url, query);

            NetRequestObservable<T, ZNetErrorEnum> sub = new NetRequestObservable<T, ZNetErrorEnum>(request);
            return sub;
        }

        static public IObservable<T> Get2<T>(string url, Dictionary<string, string> query = null)
        {
            var request = Load<NetPackage<T, string>>(url, query);

            NetRequestObservable<T, string> sub = new NetRequestObservable<T, string>(request);
            return sub;
        }



        //get responce contains status and error
        static public IObservable<RetT> PostRaw<RetT>(string url, Dictionary<string, string> query, object obj = null) {

            string strQuery = ConvertToUrlQueryStr(query);    

            IObservable<string> request = null;

            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            headers["Content-Type"] = "application/json;charset=utf-8";

            var data = ZPropertyPrefs.ConvertToStr(obj);

            //#if ZP_SERVER
            //            
            //            request = HTTPClient.Instance.Post(ip + strQuery, data, headers);
            //#else
            //            //Unity's HTTP Client 
            //            //WWWForm form = new WWWForm();
            //            //form.AddField("data", data);

            //            var data = obj != null ? ZPropertyPrefs.ConvertToStr(obj) : " ";

            //            byte[] byteData = Encoding.UTF8.GetBytes(data);

            //            request = ObservableWWW.Post(ip + strQuery, byteData, headers);
            //#endif

            request = httpEngine.Post(ip + strQuery, data, headers);

            NetRequestObservable<RetT> sub = new NetRequestObservable<RetT>(request);

            return sub;
        }

        //get responce contains status and error
        static public IObservable<ZNull> PostRaw(string url, Dictionary<string, string> query, object obj)
        {

            string strQuery = ConvertToUrlQueryStr(query);

            var data = ZPropertyPrefs.ConvertToStr(obj);

            IObservable<string> request = null;

            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            headers["Content-Type"] = "application/json;charset=utf-8";

            //#if ZP_SERVER


            //            request = HTTPClient.Instance.Post(ip + strQuery, data, headers);
            //#else

            //            //WWWForm form = new WWWForm();
            //            //form.AddField("data", data);
            //            byte[] byteData = Encoding.UTF8.GetBytes(data);

            //            request = ObservableWWW.Post(ip + strQuery, byteData, headers);
            //#endif

            request =  httpEngine.Post(ip + strQuery, data, headers);

            NetRequestObservable sub = new NetRequestObservable(request);

            return sub;
        }

        static public IObservable<string> PostRaw(string url, Dictionary<string, string> query, object obj, Dictionary<string, string> headers)
        {
            string strQuery = ConvertToUrlQueryStr(query);

            var data = ZPropertyPrefs.ConvertToStr(obj);

            IObservable<string> request = null;

            var ip = HostMap.ConvertURL(url);

            //#if ZP_SERVER
            //            request = HTTPClient.Instance.Post(ip + strQuery, data, headers);
            //#else

            //            //WWWForm form = new WWWForm();
            //            //form.AddField("data", data);
            //            byte[] byteData = Encoding.UTF8.GetBytes(data);

            //            request = ObservableWWW.Post(ip + strQuery, byteData, headers);
            //#endif

            request =  httpEngine.Post(ip + strQuery, data, headers);

            return request;
        }

        //get responce contains status and error
        static public IObservable<RetT> PutRaw<RetT>(string url, Dictionary<string, string> query, object obj = null)
        {

            string strQuery = ConvertToUrlQueryStr(query);

            var data = ZPropertyPrefs.ConvertToStr(obj);

            IObservable<string> request = null;

            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            headers["Content-Type"] = "application/json;charset=utf-8";

            //#if ZP_SERVER
            //            request = HTTPClient.Instance.Put(ip + strQuery, data, headers);
            //#else
            //            //Unity's HTTP Client 
            //            //WWWForm form = new WWWForm();
            //            //form.AddField("data", data);
            //            byte[] byteData = Encoding.UTF8.GetBytes(data);

            //            //request = ObservableWWW.Post(ip + strQuery, byteData, headers);

            //            request = UnityWebRequestObservable.Put(ip + strQuery, data, headers);
            //#endif

            request =  httpEngine.Put(ip + strQuery, data, headers);

            NetRequestObservable<RetT> sub = new NetRequestObservable<RetT>(request);

            return sub;
        }

        //get responce contains status and error
        static public IObservable<ZNull> PutRaw(string url, Dictionary<string, string> query, object obj)
        {

            string strQuery = ConvertToUrlQueryStr(query);

            var data = ZPropertyPrefs.ConvertToStr(obj);

            IObservable<string> request = null;

            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            headers["Content-Type"] = "application/json;charset=utf-8";

            //#if ZP_SERVER


            //            request = HTTPClient.Instance.Put(ip + strQuery, data, headers);
            //#else

            //            //WWWForm form = new WWWForm();
            //            //form.AddField("data", data);
            //            //byte[] byteData = Encoding.UTF8.GetBytes(data);

            //            request = UnityWebRequestObservable.Put(ip + strQuery, data, headers);
            //#endif

            request = httpEngine.Put(ip + strQuery, data, headers);

            NetRequestObservable sub = new NetRequestObservable(request);

            return sub;
        }


        static public IObservable<TResult> Post<TResult>(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PostRaw<NetPackage<TResult, ZNetErrorEnum>>(url, query, obj);

            NetRequestObservable<TResult, ZNetErrorEnum> sub = new NetRequestObservable<TResult, ZNetErrorEnum>(request);
            return sub;
        }

        static public IObservable<TResult> Post2<TResult>(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PostRaw<NetPackage<TResult, string>>(url, query, obj);

            NetRequestObservable<TResult, string> sub = new NetRequestObservable<TResult, string>(request);
            return sub;
        }

        static public IObservable<TResult> Post<TResult, TErrorEnum>(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PostRaw<NetPackage<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>>(url, query, obj);

            NetRequestObservable<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>> sub 
                = new NetRequestObservable<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>(request);
            return sub;
        }

        static public IObservable<ZNull> Post(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PostRaw<NetPackage<ZNull, ZNetErrorEnum>>(url, query, obj);

            NetRequestObservable<ZNull, ZNetErrorEnum> sub = new NetRequestObservable<ZNull, ZNetErrorEnum>(request);
            return sub;
        }

        static public IObservable<TResult> Put<TResult>(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PutRaw<NetPackage<TResult, ZNetErrorEnum>>(url, query, obj);

            NetRequestObservable<TResult, ZNetErrorEnum> sub = new NetRequestObservable<TResult, ZNetErrorEnum>(request);
            return sub;
        }

        static public IObservable<TResult> Put2<TResult>(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PutRaw<NetPackage<TResult, string>>(url, query, obj);

            NetRequestObservable<TResult, string> sub = new NetRequestObservable<TResult, string>(request);
            return sub;
        }

        static public IObservable<TResult> Put<TResult, TErrorEnum>(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PutRaw<NetPackage<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>>(url, query, obj);

            NetRequestObservable<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>> sub = new NetRequestObservable<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>(request);
            return sub;
        }

        static public IObservable<ZNull> Put(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = PutRaw<NetPackage<ZNull, ZNetErrorEnum>>(url, query, obj);

            NetRequestObservable<ZNull, ZNetErrorEnum> sub = new NetRequestObservable<ZNull, ZNetErrorEnum>(request);
            return sub;
        }

        //get responce contains status and error
        //Get object from netrequest
        static public IObservable<T> DeleteRaw<T>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ConvertToUrlQueryStr(query);

            //NetRequestObservable<T> sub = null;
            IObservable<string> request = null;
            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            //headers["Content-Type"] = "applicatin/json;charset=utf-8";

            //#if ZP_SERVER

            //            request = HTTPClient.Instance.Delete(ip + strQuery);
            //#else
            //             request = UnityWebRequestObservable.Delete(ip + strQuery, headers);
            //#endif

            request = httpEngine.Delete(ip + strQuery);

            NetRequestObservable<T> sub = new NetRequestObservable<T>(request);
            return sub;
        }

        static public IObservable<ZNull> DeleteRaw(string url, Dictionary<string, string> query = null)
        {

            string strQuery = ConvertToUrlQueryStr(query);

            IObservable<string> request = null;
            var ip = HostMap.ConvertURL(url);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (IsToken())
                headers["Authorization"] = "Bearer " + token;

            //headers["Content-Type"] = "applicatin/json;charset=utf-8";

            //#if ZP_SERVER

            //            request = HTTPClient.Instance.Delete(ip + strQuery, headers);
            //#else
            //            request = UnityWebRequestObservable.Delete(ip + strQuery, headers);
            //#endif

            request = httpEngine.Delete(ip + strQuery, headers);

            NetRequestObservable sub = new NetRequestObservable(request);
            return sub;
        }

        static public IObservable<T> Delete<T, TErrorEnum>(string url, Dictionary<string, string> query = null)
        {
            var request = DeleteRaw<NetPackage<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>>(url, query);

            NetRequestObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>> sub 
                = new NetRequestObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>(request);
            return sub;
        }

        static public IObservable<T> Delete<T>(string url, Dictionary<string, string> query = null)
        {
            var request = DeleteRaw<NetPackage<T, ZNetErrorEnum>>(url, query);

            NetRequestObservable<T, ZNetErrorEnum> sub = new NetRequestObservable<T, ZNetErrorEnum>(request);
            return sub;
        }

        static public IObservable<T> Delete2<T>(string url, Dictionary<string, string> query = null)
        {
            var request = DeleteRaw<NetPackage<T, string>>(url, query);

            NetRequestObservable<T, string> sub = new NetRequestObservable<T, string>(request);
            return sub;
        }

        static public IObservable<ZNull> Delete(string url, Dictionary<string, string> query, object obj = null)
        {
            var request = DeleteRaw<NetPackage<ZNull, ZNetErrorEnum>>(url, query);

            NetRequestObservable<ZNull, ZNetErrorEnum> sub = new NetRequestObservable<ZNull, ZNetErrorEnum>(request);
            return sub;
        }


        //for result template
        static public string StringErrorResult<TErrorEnum>(TErrorEnum error, string msg)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, MultiEnum<ZNetErrorEnum, TErrorEnum>>>();
            ret.Error = error;
            ret.Msg = msg;

            return ZPropertyPrefs.ConvertToStr(ret);
        }

        static public string StringErrorResult<TErrorEnum>(TErrorEnum error)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, MultiEnum<ZNetErrorEnum, TErrorEnum>>>();
            ret.Error = error;
            //ret.Msg = msg;

            return ZPropertyPrefs.ConvertToStr(ret);
        }

        static public string StringErrorResult(ZNetErrorEnum error, string msg)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            ret.Error = error;
            ret.Msg = msg;

            return ZPropertyPrefs.ConvertToStr(ret);
        }

        static public string StringErrorResult(ZNetErrorEnum error)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            ret.Error = error;
            ret.Msg = EnumStrTools.GetStr(error);

            return ZPropertyPrefs.ConvertToStr(ret);
        }



        static public object ErrorResult<TErrorEnum>(TErrorEnum error)
        {
            if (ZPropertyMesh.IsMultiEnum(error.GetType()))
            {
                var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, TErrorEnum>>();
                ret.Error = error;
                ret.Msg = EnumStrTools.GetStr<TErrorEnum>(error);
                return ret;
            }
            else
            {
                var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, MultiEnum<ZNetErrorEnum, TErrorEnum>>>();
                ret.Error = error;
                ret.Msg = EnumStrTools.GetStr<TErrorEnum>(error);
                return ret;
            }
        }

        static public object ErrorResult<TErrorEnum>(Exception e)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, MultiEnum<ZNetErrorEnum, TErrorEnum>>>();
            ret.Error.Parse( e.ToString());
            //ret.Msg = EnumStrTools.GetStr<TErrorEnum>(error);
            return ret;
        }

        static public object ErrorResult(ZNetErrorEnum error)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();

            ret.Msg = EnumStrTools.GetStr(error);
            ret.Error = error;

            return ret;
        }

        static public object ErrorResult(Exception e)
        {
            //can get by SendPackage2
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, string>>();
            ret.Error = e.ToString();

            return ret;
        }

        static public string StringErrorResult(string error)
        {

            return "";
        }

        static public string StringOkResult<T>(T data)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            //ret.Error = default(T);
            ret.Data = data;
            return ZPropertyPrefs.ConvertToStr(ret);
        }

        static public string StringOkListResult<T>(IZPropertyList<T> data)
        {
            var ret = ZPropertyMesh.CreateObject<NetListResponce<T, ZNetErrorEnum>>();
            ret.AddRange(data);
            return ZPropertyPrefs.ConvertToStr(ret);
        }

        static public string StringOkResult()
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            //ret.Data = data;
            return ZPropertyPrefs.ConvertToStr(ret);
        }


        static public object OkResult<T>(T data)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            ret.Data = data;

            return ret;
        }


        static public object OkResult()
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            //ret.Data = data;
            return ret;
        }

        static public void ResetToken()
        {
            token = "";
        }

        static private bool IsToken()
        {
            return !string.IsNullOrEmpty(token);
        }


        public static string ConverToLaravelQueryStr(Dictionary<string, string> dic)
        {
            string strRet = "";

            if (dic == null)
                return strRet;

            foreach (KeyValuePair<string, string> item in dic)
            {
                strRet += "/" + item.Value;
            }

            return strRet;
        }

        public static string ConvertToUrlQueryStr(Dictionary<string, string> dic)
        {
            string strRet = "";

            if (dic == null)
                return strRet;

            foreach (KeyValuePair<string, string> item in dic)
            {
                if (strRet.Length == 0)
                {
                    strRet += "?";
                }
                else
                {
                    strRet += "&";
                }
                strRet += item.Key + "=" + item.Value;
            }

            return strRet;
        }

        //public static string 


    }

}
#endif