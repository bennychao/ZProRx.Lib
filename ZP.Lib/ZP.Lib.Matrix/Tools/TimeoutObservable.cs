using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;

namespace ZP.Lib.Matrix
{
    public class TimeoutObservable : OperatorObservableBase<long>
    {
        IObservable<long> observable;

        bool bTimeout = false;
        public bool IsTimeout => bTimeout;

        /// <summary>
        /// ms timeout
        /// </summary>
        /// <param name="timeout"></param>
        public TimeoutObservable(int timeout)
            : base(false)
        {
            observable = Observable.Timer(TimeSpan.FromMilliseconds(timeout));
        }

        protected override IDisposable SubscribeCore(IObserver<long> observer, IDisposable cancel)
        {
            var nw = new NetResponse(this, observer);

            return nw;
        }

        class NetResponse : IDisposable
        {
            TimeoutObservable parent;
            public IDisposable disposable;
            IObserver<long> observer;

            public NetResponse(TimeoutObservable parent, IObserver<long> observer)
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
                disposable = parent.observable.Subscribe(s =>
                {
                    parent.bTimeout = true;
                    observer.OnNext(default(long));
                },
                (e) =>
                {
                    observer.OnError(e);
                },
                () =>
                {
                    observer.OnCompleted();
                }
                );
            }
        }
    }

}
