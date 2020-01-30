#if ZP_UNIRX
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Operators;
using ZP.Lib.Net;
using ZP.Lib.CoreEx.Reactive;

namespace ZP.Lib
{
    //to observer ZP Class Net request package

    //IObservable<T>  From IObservable<string> where T : ZP Class
    internal class NetRequestObservable<T> : OperatorObservableBase<T>
    {
        public IObservable<string> request;
        public NetRequestObservable(IObservable<string> request) : base(true)
        {
            this.request = request;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new NetResponse(this, observer);
        }

        class NetResponse : IDisposable
        {
            NetRequestObservable<T> parent;
            public IDisposable disposable;
            IObserver<T> observer;

            public NetResponse(NetRequestObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
            }

            public void Register()
            {
                disposable = parent.request.Subscribe(s =>
                {
                    var obj = ZPropertyMesh.CreateObject<T>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s);
                    }
                    catch {
                        observer.OnError(new Exception("response json read error"));
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

    //IObservable<Unit>  From IObservable<string>
    internal class NetRequestObservable : OperatorObservableBase<Unit>
    {
        public IObservable<string> request;
        public NetRequestObservable(IObservable<string> request) : base(true)
        {
            this.request = request;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            return new NetResponse(this, observer);
        }

        class NetResponse : IDisposable
        {
            NetRequestObservable parent;
            public IDisposable disposable;
            IObserver<Unit> observer;

            public NetResponse(NetRequestObservable parent, IObserver<Unit> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
            }

            public void Register()
            {
                disposable = parent.request.Subscribe(s =>
                {
                    observer.OnNext(Unit.Default);
                    //observer.OnCompleted();
                },
                (e) =>
                {
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

 

    //do with error type
    //IObservable<T>  From IObservable<NetPackage<T, TErrorEnum>>
    internal class NetRequestObservable<T, TErrorEnum> : OperatorObservableBase<T>
    {
        public IObservable<NetPackage<T, TErrorEnum> > request;
        public NetRequestObservable(IObservable<NetPackage<T, TErrorEnum>> request) : base(true)
        {
            this.request = request;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new NetResponse(this, observer);
        }

        class NetResponse : IDisposable
        {
            NetRequestObservable<T, TErrorEnum> parent;
            public IDisposable disposable;
            IObserver<T> observer;

            public NetResponse(NetRequestObservable<T, TErrorEnum> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
            }

            public void Register()
            {
                disposable = parent.request.Subscribe(s =>
                {
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


    //IObservable<object> form IObservable<NetPackage<IRawDataPref, ZNetErrorEnum>>
    //convert IRawDataPref to object
    internal class NetRequestObjectObservable : OperatorObservableBase<object>
    {
        //object type
        Type type;
        public IObservable<NetPackage<IRawDataPref, ZNetErrorEnum>> request;
        public NetRequestObjectObservable(IObservable<NetPackage<IRawDataPref, ZNetErrorEnum>> request, Type type) : base(true)
        {
            this.request = request;
            this.type = type;
        }

        protected override IDisposable SubscribeCore(IObserver<object> observer, IDisposable cancel)
        {
            return new NetResponse(this, observer);
        }

        class NetResponse : IDisposable
        {
            NetRequestObjectObservable parent;
            public IDisposable disposable;
            IObserver<object> observer;

            public NetResponse(NetRequestObjectObservable parent, IObserver<object> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
            }

            public void Register()
            {
                disposable = parent.request.Subscribe(s =>
                {
                    if (s.IsResponceOk())
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
                            observer.OnError(new Exception("response json read error"));
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

   //not used
    internal class NetLoginObservable<T> : OperatorObservableBase<T>
    {

        static private string token = "";

        public string Token => token;

        public IObservable<string> request;
        public NetLoginObservable(IObservable<string> request) : base(true)
        {
            this.request = request;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new NetResponse(this, observer);
        }

        class NetResponse : IDisposable
        {
            NetLoginObservable<T> parent;
            public IDisposable disposable;

            private IDisposable refreshDisp;

            IObserver<T> observer;
            public NetResponse(NetLoginObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();

                if (refreshDisp != null)
                    refreshDisp.Dispose();
            }

            public void Register()
            {
                disposable = parent.request.Subscribe(s =>
                {
                    var obj = ZPropertyMesh.CreateObject<NetLoginPackage<T>>();
                    try
                    {
                        ZPropertyPrefs.LoadFromStr(obj, s);
                        token = obj.Token;

                        observer.OnNext(obj.Data);
                        //observer.OnCompleted();

                        refreshDisp = obj.ConfigRefresh();
                    }
                    catch
                    {
                        observer.OnError(new Exception("Login responce json read error"));
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
   
}
#endif