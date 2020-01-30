using System;
using UniRx.Operators;
using UniRx;
using System.Collections.Generic;

namespace ZP.Lib
{
    public class SubjectObservable<T> : OperatorObservableBase<T> , IDisposable
    {
        public Subject<T> request;

        private object lockOjb = new object();

        private List<OperatorObservable> netResponses = new List<OperatorObservable>();
        public SubjectObservable(Subject<T> request) : base(false)
        {
            this.request = request;
        }

        public void UnSubscribe()
        {
            Dispose();
        }

        /// <summary>
        /// for UnSubscribe the Subject
        /// </summary>
        public void Dispose()
        {
            lock (lockOjb)
            {
                foreach (var n in netResponses)
                {
                    n.Dispose();
                }

                netResponses.Clear();
            }
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var ret = new OperatorObservable(this, observer, cancel);
            lock (lockOjb)
                netResponses.Add(ret);
            return ret;
        }

        class OperatorObservable : IDisposable
        {
            SubjectObservable<T> parent;
            public IDisposable disposable;
            IObserver<T> observer;
            IDisposable cancel;

            public OperatorObservable(SubjectObservable<T> parent, IObserver<T> observer, IDisposable cancel)
            {
                this.parent = parent;
                this.observer = observer;
                this.cancel = cancel;

                Register();
            }

            public void Dispose()
            {
                disposable.Dispose();

                var target = System.Threading.Interlocked.Exchange(ref cancel, null);
                if (target != null)
                {
                    target.Dispose();
                }
            }

            public void Register()
            {
                disposable = parent.request.Subscribe(s =>
                {
                    observer.OnNext(default(T));
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
