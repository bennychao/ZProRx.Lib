using System;
using ZP.Lib.CoreEx;
using UniRx;
using ZP.Lib.Net;
using System.Threading;
using UnityEngine;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.CoreEx.Tools;

namespace ZP.Lib
{

    //IObservable<T>  From: IObservable<string>
    //INetResponse<TResult>
    //from client to server
    internal class SocketClientRequestObservable<T, TResult> : 
        MultiOperatorObservableBase<T>,
        IObservableWithScheduler,
        //INetResponse<NetPackage<TResult, TErrorEnum>>,
        INetResponsable<TResult>,
        IDisposable
    {
        public IScheduler scheduler { get; set; }

        public IObservable<string> request;
        public string Url;
        public SocketClientRequestObservable(IObservable<string> request, string url) : base(true)
        {
            this.request = request;
            this.Url = url;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

// #if ZP_SERVER
//             TTPServer.Instance.UnSubscribe(Url);
// #else
//             TTPClient.Instance.UnSubscribe(Url);
// #endif
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }

        public void SetResult(TResult result)
        {
            //var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, ZNetErrorEnum>>();
            //ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

           // (request as INetResponse<NetPackage<TResult, ZNetErrorEnum>>)?.SetResult(ret);
            ZPropertySocket.Post(this.Url + "/Result", result).Subscribe();
        }

        public void SetRawResult(string result)
        {
            ZPropertySocket.Post(this.Url + "/Result", result).Subscribe();
        }

        public void SetError(ZNetErrorEnum error)
        {
            ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
        }

        public void SetError(Exception error)
        {
            //ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            throw error;
        }

        class NetResponse : IDisposable//, INetResponse<TResult>
        {
            SocketClientRequestObservable<T, TResult> parent;
            public IDisposable disposable;
            IObserver<T> observer;

            private string clientId;

            public NetResponse(SocketClientRequestObservable<T, TResult> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();

                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {

                    var obj = ZPropertyMesh.CreateObject<T>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s);

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketClientRequestObservable<T, TResult>: response json read error"));
                    }


                    observer.OnNext(obj);
                    //observer.OnCompleted();
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //IObservable<string> From: IObservable<SocketResponse>
    //IObservableWithId<string>
    internal class SocketRawRequestObservable:
        MultiOperatorObservableBase<string>, 
        IObservableWithId<string>,
        IObservableWithScheduler,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
        public string Url;
        private string clientId = "";
        public string Id => clientId;//socketResponse.Key;
        public IScheduler scheduler { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public SocketRawRequestObservable(IObservable<ISocketPackage> request, string url) : base(true)
        {
            this.Url = url;
            this.request = request;
        }

        protected override IDisposable SubscribeCore(IObserver<string> observer, IDisposable cancel)
        {
            return base.Register( new NetResponse(this, observer));
        }

        class NetResponse : IDisposable, ISocketPackageGetable
        {

            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            SocketRawRequestObservable parent;
            public IDisposable disposable;
            IObserver<string> observer;

            public NetResponse(SocketRawRequestObservable parent, IObserver<string> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();

                //will check Dispose
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    socketResponse = s;
                    observer.OnNext(s.Value);
                    //observer.OnCompleted();
                },
                (e) =>
                {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }


    //IObservable<T>  From: IObservable<SocketResponse>
    //INetResponse<TResult>, 
    //IObservableWithId<string>
    internal class SocketRawRequestObservable<T, TResult> : 
        MultiOperatorObservableBase<T>,
        INetResponsable<TResult>, 
        IObservableWithId<string>,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
        public string Url;

        private SemaphoreSlim lockMu = new SemaphoreSlim(1);
        private string clientId;
        public IScheduler scheduler { get; set; }


        public string Id => clientId;

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketRawRequestObservable(IObservable<ISocketPackage> request, string url) : base(true)
        {
            this.request = request;
            this.Url = url;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
// #if ZP_SERVER
//             TTPServer.Instance.UnSubscribe(Url);
// #else
//              TTPClient.Instance.UnSubscribe(Url);
// #endif
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return Register(  new NetResponse(this, observer));
        }

        public void SetResult(TResult result)
        {
            //var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, ZNetErrorEnum>>();
            //ret.Data = (TResult)ZPropertyMesh.CloneObject(result);
            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", result).Subscribe();
            lockMu.Release();

            //Debug.Log("SetResult " + this.Url);
        }

        public void SetRawResult(string result)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId) ? "" : clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", result).Subscribe();
            lockMu.Release();
        }

        public void SetError(ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(Exception error)
        {
            //ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            throw error;
        }



        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketRawRequestObservable<T, TResult> parent;
            public IDisposable disposable;
            IObserver<T> observer;


            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRawRequestObservable<T, TResult> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    if (!parent.lockMu.Wait(30000))
                    {
                        //time out
                        Debug.LogWarning("wait mutex time out " + parent.Url);
                        observer.OnError(new Exception(" time out"));
                        return;
                    }

                    //Debug.Log("Get a mutex " + parent.Url);

                    socketResponse = s;
                    parent.clientId = s.Key;

                    var obj = ZPropertyMesh.CreateObject<T>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s.Value);

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketRawRequestObservable<T, TResult>:response json read error"));
                    }


                    observer.OnNext(obj);
                    //observer.OnCompleted();
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //internal class SocketRawRequestObservable<T, TErrorEnum, TResult> : SocketRawRequestObservable<T, TResult>
    //{
    //    public SocketRawRequestObservable(IObservable<ISocketPackage> request, string url) : base(request, url)
    //    {
    //    }

    //    public new void SetError(Exception error)
    //    {
    //        ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
    //        //throw error;
    //    }
    //}

    //IObservable<SocketResponseHub<T, TErrorEnum> >  From: IObservable<SocketResponse>
    //INetResponse<TResult>,
    //  IObservableWithId<string>
    internal class SocketRawPackageAndResponseObservable<T, TErrorEnum, TResult> :
        PackageOperatorObservableBase<T, TErrorEnum>,
        INetResponsable<TResult, TErrorEnum>,
        INetResponsableWithClientId<TResult>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
      
    {
        public IObservable<ISocketPackage> request;
        
        public IScheduler scheduler { get; set; }

        private string clientId;

        private SemaphoreSlim lockMu = new SemaphoreSlim(1);

        public string Id => clientId;

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketRawPackageAndResponseObservable(IObservable<ISocketPackage> request, string url) : base(true)
        {
            this.request = request;
            this.Url = url;
        }

        protected override IDisposable SubscribeCore(IObserver<SocketPackageHub<T, TErrorEnum>> observer, IDisposable cancel)
        {
            return base.Register( new NetResponse(this, observer));
        }

        //serial return
        public void SetResult(TResult result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, TErrorEnum>>();
            ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ret).Subscribe();
             lockMu.Release();

            //Debug.Log("SetResult " + this.Url);
        }

        public void SetRawResult(string result)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId) ? "" : clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", result).Subscribe();
            lockMu.Release();
        }

        public void SetError(ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
             lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(TErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
             lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(Exception error)
        {
            var zex = (error as ZNetException<TErrorEnum>);
            if (zex != null)
            {
                SetError(zex.Error);
            }
            else
            {
                //common error;
                var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, string>>();
                ret.Error = error.ToString();

                var retStr = ZPropertyPrefs.ConvertToStr(ret);

                (request as INetResponsable)?.SetRawResult(retStr);
            }
        }

        //parallel return
        public void SetResult(string cId, TResult result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, TErrorEnum>>();
            ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

            var rex = MatrixRuntimeTools.IsServerId(cId)?"": cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ret).Subscribe();
            lockMu.Release();

            //Debug.Log("SetResult " + this.Url);
        }

        public void SetError(string cId, ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(cId)?"": cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(string cId, TErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(cId) ? "" : cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(string cId, Exception error)
        {
            var zex = (error as ZNetException<TErrorEnum>);
            if (zex != null)
            {
                SetError(cId, zex.Error);
            }
            else
                throw error;
        }

        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketRawPackageAndResponseObservable<T, TErrorEnum, TResult> parent;
            public IDisposable disposable;
            IObserver<SocketPackageHub<T, TErrorEnum>> observer;


            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRawPackageAndResponseObservable<T, TErrorEnum, TResult> parent, IObserver<SocketPackageHub<T, TErrorEnum>> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();

                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
               disposable = obser.Subscribe(s =>
                {
                    if (!parent.lockMu.Wait(30000))
                    {
                        //time out
                        Debug.LogWarning("wait mutex time out " + parent.Url);
                        observer.OnError(new Exception(" time out"));
                        return;
                    }
                    //Debug.Log("Get a mutex " + parent.Url);

                    socketResponse = s;
                    parent.clientId = s.Key;
                    var spon = new  SocketPackageHub<T, TErrorEnum>();
                    spon.socketPackage = s;
                   
                    var obj = ZPropertyMesh.CreateObject<NetPackage<T, TErrorEnum>>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s.Value);
                        spon.data = obj;

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketRawPackageAndResponseObservable<T, TErrorEnum, TResult> :response json read error"));
                    }


                    observer.OnNext(spon);
                    //observer.OnCompleted();
                },
                (e) => {
                    observer.OnError(e);
                }
                ,
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //IObservable<SocketResponseHub<T> >  From: IObservable<SocketResponse>
    //INetResponse<TResult>,
    //  IObservableWithId<string>
    internal class SocketRawPackageAndResponseObservable<T, TResult> :
        PackageOperatorObservableBase<T>,
        INetResponsable<TResult>,
        INetResponsableWithClientId<TResult>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
        
        public IScheduler scheduler { get; set; }

        private string clientId;
        private SemaphoreSlim lockMu = new SemaphoreSlim(1);

        public string Id => clientId;

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketRawPackageAndResponseObservable(IObservable<ISocketPackage> request, string url) : base(false)
        {
            this.request = request;
            this.Url = url;
        }

        protected override IDisposable SubscribeCore(IObserver<SocketPackageHub<T>> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }

        public void SetResult(TResult result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, ZNetErrorEnum>>();
            ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ret).Subscribe();
            lockMu.Release();

            //Debug.Log("SetResult " + this.Url);
        }

        public void SetRawResult(string result)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId) ? "" : clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", result).Subscribe();
            lockMu.Release();
        }

        public void SetError(ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(Exception error)
        {
            //ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            throw error;
        }

        //parallel return
        public void SetResult(string cId, TResult result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, ZNetErrorEnum>>();
            ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

            var rex = MatrixRuntimeTools.IsServerId(cId)?"": cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ret).Subscribe();
            lockMu.Release();

            //Debug.Log("SetResult " + this.Url);
        }

        public void SetError(string cId, ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(cId)?"": cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(string cId, Exception error)
        {
            //ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            throw error;
        }
        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketRawPackageAndResponseObservable<T, TResult> parent;
            public IDisposable disposable;
            IObserver<SocketPackageHub<T>> observer;


            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRawPackageAndResponseObservable<T, TResult> parent, IObserver<SocketPackageHub<T>> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    if (!parent.lockMu.Wait(30000))
                    {
                        //time out
                        Debug.LogWarning("wait mutex time out " + parent.Url);
                        observer.OnError(new Exception(" time out"));
                        return;
                    }
                    //Debug.Log("Get a mutex " + parent.Url);

                    socketResponse = s;
                    parent.clientId = s.Key;
                    var spon = new SocketPackageHub<T>();
                    spon.socketPackage = s;

                    var obj = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s.Value);
                        spon.data = obj;

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketRawPackageAndResponseObservable<T, TResult>:response json read error"));
                    }


                    observer.OnNext(spon);
                    //observer.OnCompleted();
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //IObservable<SocketResponse>  From: IObservable<SocketResponse>
    internal class SocketReceiveIRawPackageObservable :
        MultiOperatorObservableBase<ISocketPackage>,
        IObservableWithScheduler,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
        public string Url;
        public IScheduler scheduler { get; set; }

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketReceiveIRawPackageObservable(IObservable<ISocketPackage> request, string url) : base(false)
        {
            this.request = request;
            this.Url = url;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
// #if ZP_SERVER
//             TTPServer.Instance.UnSubscribe(Url);
// #else
//             TTPClient.Instance.UnSubscribe(Url);
// #endif
        }

        protected override IDisposable SubscribeCore(IObserver<ISocketPackage> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }


        class NetResponse : IDisposable
        {
            SocketReceiveIRawPackageObservable parent;
            public IDisposable disposable;
            IObserver<ISocketPackage> observer;


            //private SocketResponse socketResponse;
            //public ISocketPackage SocketResponse => socketResponse;

            public NetResponse(SocketReceiveIRawPackageObservable parent, IObserver<ISocketPackage> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    observer.OnNext(s);
                    //observer.OnCompleted();
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //IObservable<SocketResponseHub<T> >  From: IObservable<SocketResponse>
    // no result return
    //  IObservableWithId<string>
    internal class SocketRawPackageAndResponseObservable<T> :
        PackageOperatorObservableBase<T>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
        
        public IScheduler scheduler { get; set; }

        private string clientId;


        public string Id => clientId;

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketRawPackageAndResponseObservable(IObservable<ISocketPackage> request, string url) : base(false)
        {
            this.request = request;
            this.Url = url;
        }

        protected override IDisposable SubscribeCore(IObserver<SocketPackageHub<T>> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }

        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketRawPackageAndResponseObservable<T> parent;
            public IDisposable disposable;
            IObserver<SocketPackageHub<T>> observer;


            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRawPackageAndResponseObservable<T> parent, IObserver<SocketPackageHub<T>> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    socketResponse = s;
                    parent.clientId = s.Key;
                    var spon = new SocketPackageHub<T>();
                    spon.socketPackage = s;

                    var obj = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s.Value);
                        spon.data = obj;

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketRawPackageAndResponseObservable<T>:response json read error"));
                    }


                    observer.OnNext(spon);
                    //observer.OnCompleted();
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }


    //IObservable<SocketResponseHub<IRawDataRef> >  From: IObservable<SocketResponse>
    //INetResponse<TResult>,
    //  IObservableWithId<string>
    internal class SocketRawPackageAndResponseObservable :
        PackageOperatorObservableBase<IRawDataPref>,
        INetResponsable<object>,
        INetResponsableWithClientId<object>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
        
        public IScheduler scheduler { get; set; }

        private string clientId;
       // private SemaphoreSlim lockMu = new SemaphoreSlim(1);

        public string Id => clientId;

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketRawPackageAndResponseObservable(IObservable<ISocketPackage> request, string url) : base(true)
        {
            this.request = request;
            this.Url = url;
        }

        protected override IDisposable SubscribeCore(IObserver<SocketPackageHub<IRawDataPref>> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }

        public void SetResult(object result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<IRawDataPref, ZNetErrorEnum>>();
            ret.Data = ZPropertyPrefs.ConvertToRawData(result); // (IRawDataPref)ZPropertyMesh.CloneObject(result);

            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ret).Subscribe();
           // lockMu.Release();

            //Debug.Log("SetResult " + this.Url);
        }

        public void SetRawResult(string result)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId) ? "" : clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", result).Subscribe();
        }

        public void SetError(ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(clientId)?"": clientId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
           // lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(Exception error)
        {
            //ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            throw error;
        }


        public void SetResult(string cId, object result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<IRawDataPref, ZNetErrorEnum>>();
            ret.Data = ZPropertyPrefs.ConvertToRawData(result); // (IRawDataPref)ZPropertyMesh.CloneObject(result);
            var rex = MatrixRuntimeTools.IsServerId(cId)?"": cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ret).Subscribe();
            //lockMu.Release();

            //Debug.Log("SetResult " + this.Url + " Client Id is " + cid);
        }

        public void SetError(string cId, ZNetErrorEnum error)
        {
            var rex = MatrixRuntimeTools.IsServerId(cId)?"": cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            //lockMu.Release();

            Debug.Log("SetResult Error " + this.Url);
        }

        public void SetError(string cId, Exception error)
        {
            var rex = MatrixRuntimeTools.IsServerId(cId) ? "" : cId + "/";
            ZPropertySocket.Post(this.Url + $"/{rex}Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            throw error;
        }

        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketRawPackageAndResponseObservable parent;
            public IDisposable disposable;
            IObserver<SocketPackageHub<IRawDataPref>> observer;


            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRawPackageAndResponseObservable parent, IObserver<SocketPackageHub<IRawDataPref>> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    //if (!parent.lockMu.Wait(30000))
                    //{
                    //    //time out
                    //    Debug.LogWarning("wait mutex time out " + parent.Url);
                    //    observer.OnError(new Exception(" time out"));
                    //    return;
                    //}

                    //Debug.Log("Get a mutex " + parent.Url);

                    socketResponse = s;
                    parent.clientId = s.Key;
                    var spon = new SocketPackageHub<IRawDataPref>();
                    spon.socketPackage = s;

                    var obj = ZPropertyMesh.CreateObject<NetPackage<IRawDataPref, ZNetErrorEnum>>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s.Value);
                        spon.data = obj;

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketRawPackageAndResponseObservable:response json read error"));
                    }


                    observer.OnNext(spon);
                    //observer.OnCompleted();

                    //will call SetResult
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }


    //IObservable<SocketResponseHub<IRawDataRef> >  From: IObservable<SocketResponse>
    //INetResponse<TResult>,
    //  IObservableWithId<string>
    // TODO Select<> ??
    internal class SocketRawPackageObservable :
        PackageOperatorObservableBase<IRawDataPref>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
    {
        public IObservable<ISocketPackage> request;
 
        public IScheduler scheduler { get; set; }

        private string clientId;
        public string Id => clientId;

        //public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketRawPackageObservable(IObservable<ISocketPackage> request, string url) : base(true)
        {
            this.request = request;
            this.Url = url;
        }

        protected override IDisposable SubscribeCore(IObserver<SocketPackageHub<IRawDataPref>> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }

        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketRawPackageObservable parent;
            public IDisposable disposable;
            IObserver<SocketPackageHub<IRawDataPref>> observer;


            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRawPackageObservable parent, IObserver<SocketPackageHub<IRawDataPref>> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    socketResponse = s;
                    parent.clientId = s.Key;
                    var spon = new SocketPackageHub<IRawDataPref>();
                    spon.socketPackage = s;

                    var obj = ZPropertyMesh.CreateObject<NetPackage<IRawDataPref, ZNetErrorEnum>>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s.Value);
                        spon.data = obj;

                    }
                    catch
                    {
                        observer.OnError(new Exception("SocketRawPackageObservable:response json read error"));
                    }


                    observer.OnNext(spon);
                    //observer.OnCompleted();

                    //will call SetResult
                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //do with error type
    //IObservable<T>  From: IObservable<NetPackage<T, TErrorEnum>>
    //INetResponse<NetPackage<TResult, TErrorEnum>>, 
    //    INetResponse<TResult>,
    //    IObservableWithId<string>
    public class SocketPackageObservable<T, TErrorEnum, TResult> : 
        MultiOperatorObservableBase<T>,
        INetResponsable<NetPackage<TResult, TErrorEnum>>, 
        INetResponsable<TResult, TErrorEnum>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
    {
        public IObservable<NetPackage<T, TErrorEnum>> request;

        public IScheduler scheduler { get; set; }

        public string Id => (request as IObservableWithId<string>)?.Id;

        public SocketPackageObservable(IObservable<NetPackage<T, TErrorEnum>> request) : base(true)
        {
            this.request = request;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            (request as IDisposable)?.Dispose();
//#if ZP_SERVER
//            TTPServer.Instance.UnSubscribe(Url);
//#else
//            TTPClient.Instance.UnSubscribe(Url);
//#endif
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return Register( new NetResponse(this, observer));
        }

        public void SetResult(NetPackage<TResult, TErrorEnum> result)
        {
            //var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, TErrorEnum>>();
            //ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

            (request as INetResponsable<NetPackage<TResult, TErrorEnum>>)?.SetResult(result);
        }
        public void SetResult(TResult result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, TErrorEnum>>();
            ret.Data = (TResult)ZPropertyMesh.CloneObject(result);

            (request as INetResponsable<NetPackage<TResult, TErrorEnum>>)?.SetResult(ret);
        }

        public void SetRawResult(string result)
        {
            (request as INetResponsable)?.SetRawResult(result);
        }

        public void SetError(ZNetErrorEnum error)
        {
            (request as INetResponsable<NetPackage<TResult, TErrorEnum>>)?.SetError(error);
        }

        public void SetError(TErrorEnum error)
        {
            (request as INetResponsable<TResult, TErrorEnum>)?.SetError(error);
        }

        public void SetError(Exception error)
        {
            //ZPropertySocket.Post(this.Url + "/Result", ZPropertyNet.ErrorResult(error)).Subscribe();
            //throw error;
            var zex = (error as ZNetException<TErrorEnum>);
            if (zex != null)
            {
                var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, TErrorEnum>>();
                ret.Error = zex.Error;

                (request as INetResponsable<NetPackage<TResult, TErrorEnum>>)?.SetResult(ret);
            }
            else
            {
                //common error;
                var ret = ZPropertyMesh.CreateObject<NetPackage<TResult, string>>();
                ret.Error = error.ToString();

                var retStr = ZPropertyPrefs.ConvertToStr(ret);

                (request as INetResponsable)?.SetRawResult(retStr);
            }               
        }

        class NetResponse : IDisposable, ISocketPackageGetable
        {
            SocketPackageObservable<T, TErrorEnum, TResult> parent;
            public IDisposable disposable;
            IObserver<T> observer;

            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketPackageObservable<T, TErrorEnum, TResult> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    var vr = disposable as ISocketPackageGetable;

                    socketResponse = vr?.SocketPackage as ISocketPackage;

                    if (s.IsResponceOk())
                    {
                        observer.OnNext(s.Data);
                        //observer.OnCompleted();
                    }
                    else
                    {
                        observer.OnError(new ZNetException<TErrorEnum>(s.Error));
                    }

                },
                (e) => {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }

    //IObservable<object>  From: IObservable<NetPackage<IRawDataPref, ZNetErrorEnum>> 
    // object is support ZNull IRawDataPref or Object (load form IRawDataPref)
    //    INetResponse<IRawDataPref>,
    //    IObservableWithId<string>
    internal class SocketRequestObjectObservable : 
        MultiOperatorObservableBase<object>, 
        INetResponsable<IRawDataPref>,
        IObservableWithScheduler,
        IObservableWithId<string>,
        IDisposable
    {
        Type type;
        public IObservable<NetPackage<IRawDataPref, ZNetErrorEnum>> request;
        public IScheduler scheduler { get; set; }

        public string Id => (request as IObservableWithId<string>)?.Id;

        public SocketRequestObjectObservable(IObservable<NetPackage<IRawDataPref, ZNetErrorEnum>> request, Type type) : base(true)
        {
            this.request = request;
            this.type = type;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            (request as IDisposable)?.Dispose();
        }

        protected override IDisposable SubscribeCore(IObserver<object> observer, IDisposable cancel)
        {
            return Register(new NetResponse(this, observer));
        }

        public void SetResult(IRawDataPref result)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<IRawDataPref, ZNetErrorEnum>>();
            ret.Data = ZPropertyPrefs.ConvertToRawData(result); // (IRawDataPref)ZPropertyMesh.CloneObject(result);


            (request as INetResponsable<NetPackage<IRawDataPref, ZNetErrorEnum>>)?.SetResult(ret);
        }

        public void SetRawResult(string result)
        {
            (request as INetResponsable)?.SetRawResult(result);
        }

        public void SetError(ZNetErrorEnum error)
        {
            (request as INetResponsable<NetPackage<IRawDataPref, ZNetErrorEnum>>)?.SetError(error);
        }

        public void SetError(Exception error)
        {
            throw error;
        }

        class NetResponse : IDisposable, ISocketPackageGetable//,  INetResponse<IRawDataPref>
        {
            SocketRequestObjectObservable parent;
            public IDisposable disposable;
            IObserver<object> observer;

            private ISocketPackage socketResponse;
            public ISocketPackage SocketPackage => socketResponse;

            public NetResponse(SocketRequestObjectObservable parent, IObserver<object> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();

                parent.UnRegister(this);
            }

            public void Register()
            {
                var obser = parent.scheduler == null ? parent.request : parent.request.ObserveOn(parent.scheduler);
                disposable = obser.Subscribe(s =>
                {
                    var vr = disposable as ISocketPackageGetable;

                    socketResponse = vr?.SocketPackage as ISocketPackage;

                    if (s.IsResponceOk())
                    {
                        if (ZNull.IsNull(parent.type))
                        {
                            observer.OnNext(ZNull.Default);
                            //observer.OnCompleted();
                        }
                        else if (ZPropertyMesh.IsRawDataRef(parent.type))
                        {
                            observer.OnNext(s.Data);
                            //observer.OnCompleted();
                        }
                        else
                        {
                            var obj = ZPropertyMesh.CreateObject(parent.type);
                            try
                            {
                                ZPropertyPrefs.LoadFromRawData(obj, s.Data);

                                observer.OnNext(obj);
                                //observer.OnCompleted();
                            }
                            catch
                            {
                                observer.OnError(new Exception("SocketRequestObjectObservable:response json read error"));
                            }
                        }
                    }
                    else
                    {
                        observer.OnError(new ZNetException<ZNetErrorEnum>(s.Error));
                    }
                },
                (e) =>
                {
                    observer.OnError(e);
                },
                ()=>{
                    observer.OnCompleted();
                }

                );
            }
        }
    }
}

