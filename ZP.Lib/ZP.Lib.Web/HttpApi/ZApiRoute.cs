using System;
using ZP.Lib.Web.Domain;
using UniRx;
using UniRx.Operators;

namespace ZP.Lib.Web
{
    public class ZApiRoute<T> : OperatorObservableBase<T>, IApiRoute//T is return value type
    {
        public string Template { get; set; }
        public MethodTypeEnum Method { get; set; }

        public Type ReturnType => typeof(T);

        internal Subject<T> request = new Subject<T>();


        public ZApiRoute(string template, MethodTypeEnum method = MethodTypeEnum.POST) : base(false)
        {
            Template = template;
            Method = method;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new NetResponse(this.request, observer);
        }

        public void OnNext(object data)
        {
            request.OnNext((T)data);
            //request.OnError()
        }

        public void OnError(Exception error)
        {
            request.OnError(error);
        }

        class NetResponse : IDisposable
        {
            Subject<T> request;
            public IDisposable disposable = null;
            IObserver<T> observer;

            public NetResponse(Subject<T> request, IObserver<T> observer)
            {
                this.request = request;
                this.observer = observer;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();
            }

            public void Register()
            {
                request.Subscribe(s =>
                {
                    observer.OnNext(default(T));
                    observer.OnCompleted();
                },
                (e) =>
                {
                    observer.OnError(e);
                }
                );
            }
        }

    }
}
