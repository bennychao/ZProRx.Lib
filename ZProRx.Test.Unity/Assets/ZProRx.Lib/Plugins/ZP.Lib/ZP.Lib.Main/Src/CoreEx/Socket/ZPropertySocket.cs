
#if ZP_UNIRX // && ZP_UNITY_CLIENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.Net;
using ZP.Lib.CoreEx;
using ZP.Lib.Core.Domain;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Common;
using System.Diagnostics;
using System.Threading;
//using ClientResponse = System.Collections.Generic.KeyValuePair<string, string>;

namespace ZP.Lib
{
    public static class ZPropertySocket {

        public static string ClientIP = "127.0.0.1";
        public static int ClientPort = 8222;

        public static string ClientID = "1000";

        //TODO not used
        public static string ClientUser = "username001";
        public static string ClientPassword = "psw001";

        static ZPropertySocket()
        {
            //connect
            //var a = TTPClient.Instance;
        }

        //no recevie msg
        static public IObservable<ZNull> Post(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);
            NetRequestObservable sub = null;

#if ZP_SERVER
            sub = new NetRequestObservable(TTPServer.Instance.SendMsg(url, strQuery));
            return sub;
#else
            sub = new NetRequestObservable(TTPClient.Instance.SendMsg(url, strQuery));
            //return Observable.NextFrame();
            return sub;
#endif

        }

        //param:raw obj 
        static public IObservable<ZNull> Post(string url, object obj)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(obj);


#if ZP_SERVER
            NetRequestObservable sub = null;
            sub = new NetRequestObservable(TTPServer.Instance.SendMsg(url, strQuery));
            return sub;
#else
            return TTPClient.Instance.SendMsg(url, strQuery).Select(_=> ZNull.Default);
            //return Observable.NextFrame();
#endif

        }

        static public IObservable<ZNull> PostPackage<T>(string url, T obj)
        {
            var send = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            send.Data = obj;

            string strQuery = ZPropertyPrefs.ConvertToStr(send);

#if ZP_SERVER
            NetRequestObservable sub = null;
            sub = new NetRequestObservable(TTPServer.Instance.SendMsg(url, strQuery));
            return sub;
#else
            return TTPClient.Instance.SendMsg(url, strQuery).Select(_=> ZNull.Default);
            //return Observable.NextFrame();
#endif

            //sub.Subscribe();

        }

        //param:raw obj 
        static public IObservable<TResult> Send<TResult>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);
            //NetRequestObservable<TResult> sub = null;

            return Observable.Create<TResult>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();

                var multiObserver = MultiObserver<TResult>.Create(observer);

                var clientIdUrl = GetClientId;
                var revObservable = ReceivePackage<TResult>(url + $"/{clientIdUrl}Result");
                var recvDisp = revObservable.Subscribe(multiObserver);
                recvDisp.AddTo(disposables);

#if ZP_SERVER
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(() => recvDisp.Dispose()));
                TTPServer.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);
                //revObservable.Subscribe(_ => TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
                // sub = new NetRequestObservable<TResult>(TTPServer.Instance.Subscribe(url));
#else
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(()=> recvDisp.Dispose()));
                TTPClient.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);
                //revObservable.Subscribe(_ => TTPClient.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
                //sub = new NetRequestObservable<TResult>(TTPClient.Instance.Subscribe(url));
#endif
                return disposables;

            }); // end Observable Create
        }

        /// <summary>
        /// Support Retry
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        static public IObservable<TResult> Send<TResult>(string url, object data)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(data);
            //NetRequestObservable<TResult> sub = null;

            return Observable.Create<TResult>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();

                var multiObserver = MultiObserver<TResult>.Create(observer);
                var clientIdUrl = GetClientId;
                var revObservable = ReceivePackage<TResult>(url + $"/{clientIdUrl}Result");
                var recvDisp = revObservable.Subscribe(multiObserver);
                recvDisp.AddTo(disposables);

#if ZP_SERVER

                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(() => recvDisp.Dispose()));
                TTPServer.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);
                //sub = new NetRequestObservable<TResult>(TTPServer.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#else
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(() =>  recvDisp.Dispose()));
                TTPClient.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);
                //revObservable.Subscribe(_ => TTPClient.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
                //sub = new NetRequestObservable<TResult>(TTPClient.Instance.Subscribe(url));
#endif

                return disposables;
            }); //end Observable Create

        }

        //only support ZNetErrorEnum (ZNetException)
        //send: T  (NetPackage<T, ZNetErrorEnum>) //result:ZNetErrorEnum (NetPackage<T, ZNetErrorEnum>)
        static public IObservable<TResult> SendPackage<T, TResult>(string url, T obj)
        {
            var send = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            send.Data = obj;

            string strQuery = ZPropertyPrefs.ConvertToStr(send);

            var runId = MatrixRuntimeTools.GetRunId();

            //NetRequestObservable<TResult> sub = null;
            return Observable.Create<TResult>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();

                var multiObserver = MultiObserver<TResult>.Create(observer);

                var clientIdUrl = GetClientId;

                var revObservable = ReceivePackage<TResult>(url + $"/{clientIdUrl}Result");
                var recvDisp = revObservable.Subscribe(multiObserver);
                recvDisp.AddTo(disposables);

#if ZP_SERVER
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(() =>
                {
                    //TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result");
                    recvDisp.Dispose();
                }));

                TTPServer.Instance.SendMsg(url, strQuery)
                .Subscribe().AddTo(disposables);

                //sub = new NetRequestObservable<TResult>(TTPServer.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#else
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(()  => recvDisp.Dispose()));
                TTPClient.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);

                //sub = new NetRequestObservable<TResult>(TTPClient.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPClient.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#endif

                return disposables; //support Cancellable
            });


            //return ret;
        }

        static public IObservable<TResult> SendPackage2<T, TResult>(string url, T obj)
        {
            var send = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            send.Data = obj;

            string strQuery = ZPropertyPrefs.ConvertToStr(send);

            //NetRequestObservable<TResult> sub = null;
            return Observable.Create<TResult>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();

                var multiObserver = MultiObserver<TResult>.Create(observer);

                var clientIdUrl = GetClientId;
                //Error Type is string
                var revObservable = ReceivePackage2<TResult>(url + $"/{clientIdUrl}Result");
                var recvDisp = revObservable.Subscribe(multiObserver);
                recvDisp.AddTo(disposables);

#if ZP_SERVER
                //TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result")
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(() =>
                {
                    //UnityEngine.Debug.Log($"SendPackage2 CreateOnTerminate {url} clientId is " + clientIdUrl);
                    recvDisp.Dispose();

                }));
                TTPServer.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);

                //sub = new NetRequestObservable<TResult>(TTPServer.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#else
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(()  =>  recvDisp.Dispose()));
                TTPClient.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);

                //sub = new NetRequestObservable<TResult>(TTPClient.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPClient.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#endif

                return disposables; //support Cancellable
            });


            //return ret;
        }


        static public IObservable<TResult> SendPackage<T, TErrorEnum, TResult>(string url, T obj)
        {
            var send = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            send.Data = obj;

            string strQuery = ZPropertyPrefs.ConvertToStr(send);

            //NetRequestObservable<TResult> sub = null;
            return Observable.Create<TResult>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();

                var multiObserver = MultiObserver<TResult>.Create(observer);

                var clientIdUrl = GetClientId;

                var revObservable = ReceivePackage<TResult, TErrorEnum>(url + $"/{clientIdUrl}Result");
                var recvDisp = revObservable.Subscribe(multiObserver);
                recvDisp.AddTo(disposables);

#if ZP_SERVER
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(() => recvDisp.Dispose()));
                TTPServer.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);

                //sub = new NetRequestObservable<TResult>(TTPServer.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#else
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<TResult>(()  => recvDisp.Dispose()));
                TTPClient.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);

                //sub = new NetRequestObservable<TResult>(TTPClient.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPClient.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
#endif

                return disposables; //support Cancellable
            });


            //return ret;
        }


        /// <summary>
        /// send: raw data //result:ZNetErrorEnum (NetPackage<T, ZNetErrorEnum>)
        /// Support Retry
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public IObservable<ZNull> Send(string url, object obj)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(obj);
            //NetRequestObservable<ZNull> sub = null;

            return Observable.Create<ZNull>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();
                var multiObserver = MultiObserver<ZNull>.Create(observer);
                var clientIdUrl = GetClientId;

                var revObservable = ReceivePackage<ZNull>(url + $"/{clientIdUrl}Result");
                var ret = revObservable.Subscribe(multiObserver);
                ret.AddTo(disposables);

#if ZP_SERVER

                //TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result")
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<ZNull>(() => ret.Dispose()));
                TTPServer.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);

                //revObservable.Subscribe(_ => TTPServer.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));
                //sub = new NetRequestObservable<Unit>(TTPServer.Instance.Subscribe(url));

#else
                multiObserver.AddObserver(ObserverEx.CreateOnTerminate<ZNull>(() => ret.Dispose()));
                TTPClient.Instance.SendMsg(url, strQuery).Subscribe().AddTo(disposables);
                //sub = new NetRequestObservable<TResult>(TTPClient.Instance.Subscribe(url));
                //revObservable.Subscribe(_ => TTPClient.Instance.UnSubscribe(url + $"/{clientIdUrl}Result"));

#endif
                return disposables;

            }); //end Observable Create

        }

        // /-------------------------------------------------   Receive Functions ---------------------------------------------/ //

        //result raw string
        static public IObservable<string> ReceiveRaw(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

            SocketRawRequestObservable sub = null;

#if ZP_SERVER

            sub = new SocketRawRequestObservable(TTPServer.Instance.SubscribeWithClientId(url), url);
#else
            sub = new SocketRawRequestObservable(TTPClient.Instance.SubscribeWithClientId(url), url);
#endif
            return sub.ToDisposable(()=> UnSubscribe(url));
        }

        static public IObservable<T> ReceiveRaw<T>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

            NetRequestObservable<T> sub = null;

#if ZP_SERVER

            sub = new NetRequestObservable<T>(TTPServer.Instance.Subscribe(url));
#else
            sub = new NetRequestObservable<T>(TTPClient.Instance.Subscribe(url));
#endif
            return sub.ToDisposable(() => UnSubscribe(url));
        }

        static public IObservable<T> ReceiveRaw<T, TResult>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER
            //SocketRawRequestObservable<T, TResult> sub = null;
            return new SocketRawRequestObservable<T, TResult>(
                TTPServer.Instance.SubscribeWithClientId(url)
                .ToDisposable(() => UnSubscribe(url)), 
                url);//.ToDisposable(() => UnSubscribe(url));
#else
            return new SocketClientRequestObservable<T, TResult> (
                TTPClient.Instance.Subscribe(url)
                .ToDisposable(() => UnSubscribe(url)), 
                url);//.ToDisposable(() => UnSubscribe(url));
#endif
            //return sub;
        }

        //        static public IObservable<T> ReceiveRaw<T, TError, TResult>(string url, Dictionary<string, string> query = null)
        //        {
        //            string strQuery = ZPropertyPrefs.ConvertToStr(query);

        //#if ZP_SERVER
        //            //SocketRawRequestObservable<T, TResult> sub = null;
        //            return new SocketRawRequestObservable<T, TError, TResult>(TTPServer.Instance.SubscribeWithClientId(url), url);
        //#else
        //            return new SocketClientRequestObservable<T, TError, TResult> (TTPClient.Instance.Subscribe(url), url);
        //#endif
        //            //return sub;
        //        }

        static public IObservable<object> ReceiveRaw(string url, Type type, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

            NetRequestObservable<object> sub = null;

#if ZP_SERVER

            sub = new NetRequestObservable<object>(TTPServer.Instance.Subscribe(url));
#else
            sub = new NetRequestObservable<object>(TTPClient.Instance.Subscribe(url));
#endif
            return sub.ToDisposable(() => UnSubscribe(url));
        }

        //param: SocketResponseHub<T, TErrorEnum> 
        //return : TResult
        static public IRawPackageObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>> ReceiveRawPackageAndResponse<T, TErrorEnum, TResponse>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER

            return new SocketRawPackageAndResponseObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>, TResponse>(
                TTPServer.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url))
                , url);
            //.ToDisposable(() => UnSubscribe(url));            
#else
            return new SocketRawPackageAndResponseObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>, TResponse> (
                TTPClient.Instance.SubscribeWithClientId(url)
                .ToDisposable(() => UnSubscribe(url)), url);
                //.ToDisposable(()=>UnSubscribe(url));
#endif
            // return sub;
        }

        //param: SocketResponseHub<T, ZNetErrorEnum> 
        //return : TResult
        static public IRawPackageObservable<T> ReceiveRawPackageAndResponse<T, TResponse>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER

            return new SocketRawPackageAndResponseObservable<T, TResponse>(
                TTPServer.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url))
                , url);
            // .ToDisposable(() => UnSubscribe(url));
#else
            return new SocketRawPackageAndResponseObservable<T, TResponse>(
                TTPClient.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url)), 
                url);
                //.ToDisposable(() => UnSubscribe(url));
#endif
            // return sub;
        }

        //param: SocketResponseHub<T, ZNetErrorEnum> 
        //return :No TResult
        static public IRawPackageObservable<T> ReceiveRawPackageAndResponse<T>(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER

            return new SocketRawPackageAndResponseObservable<T>(
                TTPServer.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url))
                , url);
            // .ToDisposable(() => UnSubscribe(url));
#else
            return new SocketRawPackageAndResponseObservable<T>(
                TTPClient.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url)), 
                url);
                //.ToDisposable(() => UnSubscribe(url));
#endif
            // return sub;
        }

        //param: IObservable<SocketResponse> 
        //return :No TResult
        static public IObservable<ISocketPackage> ReceiveLowRawPackage(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER

            return new SocketReceiveIRawPackageObservable(
                TTPServer.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url)), 
                url);
            // .ToDisposable(() => UnSubscribe(url));
#else
            return new SocketReceiveIRawPackageObservable(
                TTPClient.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url)), 
                url);
               // .ToDisposable(() => UnSubscribe(url));
#endif
            // return sub;
        }

        //param: SocketResponseHub<T, IRawDataPref> 
        //return : TResult
        static public IRawPackageObservable<IRawDataPref> ReceiveRawPackageAndResponse(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER

            return new SocketRawPackageAndResponseObservable(
                TTPServer.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url))
                , url);
            //  .ToDisposable(() => UnSubscribe(url));
#else
            return new SocketRawPackageAndResponseObservable(
                TTPClient.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url)), 
                url);
               // .ToDisposable(() => UnSubscribe(url));
#endif
            // return sub;
        }

        //param: SocketResponseHub<T, IRawDataPref> 
        //return : TResult
        static public IRawPackageObservable<IRawDataPref> ReceiveRawPackage(string url, Dictionary<string, string> query = null)
        {
            string strQuery = ZPropertyPrefs.ConvertToStr(query);

#if ZP_SERVER

            return new SocketRawPackageObservable(
                TTPServer.Instance.SubscribeWithClientId(url)
                .ToDisposable(() => UnSubscribe(url)), 
                url);
#else
            return new SocketRawPackageObservable(
                TTPClient.Instance.SubscribeWithClientId(url).ToDisposable(() => UnSubscribe(url)), 
                url);
#endif
            // return sub;
        }

        //param: object return: null;
        static public IObservable<object> ReceiveObject(string url, Type type, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<IRawDataPref, ZNetErrorEnum>>(url, query);

            NetRequestObjectObservable sub = new NetRequestObjectObservable(request, type);
            return sub;
        }

        //can receive


        //param: T return: null;
        static public IObservable<T> ReceivePackage<T>(string url, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<T, ZNetErrorEnum>>(url, query);

            NetRequestObservable<T, ZNetErrorEnum> sub = new NetRequestObservable<T, ZNetErrorEnum>(request);
            return sub;//.ToDisposable(()=> (request as IDisposable)?.Dispose());
        }

        static public IObservable<T> ReceivePackage<T, TErrorEnum>(string url, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>>(url, query);

            NetRequestObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>> sub
                = new NetRequestObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>(request);
            return sub;
        }

        //used by inner to receive string error data
        static internal IObservable<T> ReceivePackage2<T>(string url, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<T, string>>(url, query);

            NetRequestObservable<T, string> sub
                = new NetRequestObservable<T, string>(request);
            return sub;
        }

        //param: T return: TResult (NetPackage<TResult, TErrorEnum>)
        //Subscribe func return TResult or NetPackage<TResult, ZNetErrorEnum>
        static public IObservable<T> ReceivePackageAndResponse<T, TErrorEnum, TResult>(string url, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<T, MultiEnum<ZNetErrorEnum, TErrorEnum>>, NetPackage<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>>(url, query);

            SocketPackageObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>, TResult> sub = new SocketPackageObservable<T, MultiEnum<ZNetErrorEnum, TErrorEnum>, TResult>(request);
            return sub;
        }

        //param: T return: TResult (NetPackage<TResult, ZNetErrorEnum>) 
        //Subscribe func return TResult or NetPackage<TResult, ZNetErrorEnum>
        static public IObservable<T> ReceivePackageAndResponse<T, TResult>(string url, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<T, ZNetErrorEnum>, NetPackage<TResult, ZNetErrorEnum>>(url, query);

            SocketPackageObservable<T, ZNetErrorEnum, TResult> sub = new SocketPackageObservable<T, ZNetErrorEnum, TResult>(request);
            return sub;
        }

        //param: T return: ZNetErrorEnum
        //Subscribe func return NetPackage<ZNull, ZNetErrorEnum>
        static public IObservable<T> ReceivePackageAndResponse<T>(string url, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<T, ZNetErrorEnum>, NetPackage<ZNull, ZNetErrorEnum>>(url, query);

            SocketPackageObservable<T, ZNetErrorEnum, ZNull> sub = new SocketPackageObservable<T, ZNetErrorEnum, ZNull>(request);
            return sub;
        }

        //param: object return: IRawDataPref;
        //Subscribe func return IRawDataPref
        static public IObservable<object> ReceivePackageAndResponse(string url, Type type, Dictionary<string, string> query = null)
        {
            var request = ReceiveRaw<NetPackage<IRawDataPref, ZNetErrorEnum>>(url, query);

            SocketRequestObjectObservable sub = new SocketRequestObjectObservable(request, type);
            return sub;
        }

        //static public IObservable<object> ReceivePackageAndResponseRaw(string url, Dictionary<string, string> query = null)
        //{
        //    var request = ReceiveRaw<NetPackage<IRawDataPref, ZNetErrorEnum>>(url, query);

        //    SocketRequestObjectObservable sub = new SocketRequestObjectObservable(request, typeof(IRawDataPref));
        //    return sub;
        //}


        static public IObservable<Dictionary<string, object>> ReceiveMap(string url)
        {
            NetRequestObservable<Dictionary<string, object>> sub = null;

#if ZP_SERVER

            sub = new NetRequestObservable<Dictionary<string, object>>(TTPServer.Instance.Subscribe(url));
#else
            sub = new NetRequestObservable<Dictionary<string, object>>(TTPClient.Instance.Subscribe(url));
#endif
            return sub.ToDisposable(()=>UnSubscribe(url));
        }

        static public IObservable<bool> RunningObservable  {
            get
            {
#if ZP_SERVER
                return TTPServer.Instance.RunningObservable;
#else
                return Observable.Return<bool>(true);
#endif
            }
        }

        static public IObservable<string> OnConnected() {
            IObservable<string> ret = null;

#if ZP_SERVER
            ret = TTPServer.Instance.OnConnected();

#endif
            return ret;
        }


        static public IObservable<string> OnDisConnected()
        {
            IObservable<string> ret = null;

#if ZP_SERVER
            ret = TTPServer.Instance.OnDisConnected();

#endif
            return ret;
        }



        static public void Start()
        {

#if ZP_SERVER
            TTPServer.Instance.Connect();
#else
            TTPClient.Instance.Connect();
#endif

        }

        static public void Close()
        {
#if ZP_SERVER
            TTPServer.Instance.Disconnect();
#else
            TTPClient.Instance.Disconnect();
#endif

        }

        static public bool IsConnected()
        {
#if ZP_SERVER
            return TTPServer.Instance.IsRunning;
            //return true;
#else
            return TTPClient.Instance.IsConnected();
#endif

        }

        static public NetPackage<T, ZNetErrorEnum> OkResult<T>(T data)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            ret.Data = data;

            return ret;
        }

        static public NetPackage<ZNull, ZNetErrorEnum> OkResult()
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            //ret.Data = data;

            return ret;
        }


        static public TData ParseData<TData>(ISocketPackage socketPackage)
        {
            var ret = ZPropertyMesh.CreateObject<TData>();
            //ret.Data = data;
            ZPropertyPrefs.LoadFromStr(ret, socketPackage.Value);
            return ret;
        }


        //static public NetPackage<ZNetErrorEnum> ErrorResult()
        //{
        //    var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
        //    ret.Data = data;

        //    return ret;
        //}

        static public string AssembleUrl(string baseUrl, string action)
        {
            return action.Contains(baseUrl) ? action : baseUrl + "/" + action;
        }

        static public string AssembleUrl(string baseUrl, string action, string clientId)
        {
            //return action.Contains(baseUrl) ? action : baseUrl + "/" + action + "/" + clientId;
            var ret = action.Contains(baseUrl) ? action : baseUrl + "/" + action;

            ret = ret.Contains("/" + clientId) ? ret : ret + "/" + clientId;
            return ret;
        }

        static private void UnSubscribe(string url)
        {
            //UnityEngine.Debug.Log("UnSubscribe " + url);

#if ZP_SERVER
            TTPServer.Instance.UnSubscribe(url);
#else
            TTPClient.Instance.UnSubscribe(url);
#endif
        }


        [Conditional("DEBUG")]
        static public void FakeConnect(string clientId){
#if ZP_SERVER
            TTPServer.Instance.FakeConnect(clientId);
#endif
        }

        [Conditional("DEBUG")]
        static public void FakeDisConnect(string clientId){
#if ZP_SERVER
            TTPServer.Instance.FakeDisConnect(clientId);
#endif
        }

        [Conditional("DEBUG")]
        static public void CheckRecvListenerCount()
        {
            Thread.Sleep(300);
#if ZP_SERVER
            TTPServer.Instance.CheckRecvListenerCount();
#endif
        }

        [Conditional("DEBUG")]
        static public void CheckRecvListenerCount(int recvCount, int recvWithIdCount)
        {
            Thread.Sleep(300);
#if ZP_SERVER
            TTPServer.Instance.CheckRecvListenerCount(recvCount, recvWithIdCount);
#endif
        }

        static string GetClientId
        {
            get
            {                
                var clientId = (TaskScheduler.Current as IZMatrixRuntime)?.RunId ?? ZPropertySocket.ClientID;
                return MatrixRuntimeTools.IsInServer() ? "" : clientId + "/";
            }
        }

        // static public bool IsServerClient(string clientId){
        //     return clientId.Contains("RoomServer");
        // }

    }

}
#endif
