using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;

namespace ZP.Lib.CoreEx.Reactive
{
    internal class FetchOperator<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;

        public FetchOperator(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new Fetch(this, observer, cancel));
        }

        class Fetch : OperatorObserverBase<T, T>
        {
            readonly FetchOperator<T> parent;
            object lockObj = new object();
            bool isCompleted = false;

            public Fetch(FetchOperator<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);

                lock(lockObj)
                { 
                    isCompleted = true;

                    observer.OnCompleted();
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                try { 
                    lock (lockObj)
                    {
                        if (!isCompleted)
                        {
                            observer.OnCompleted();
                        }
                    }

                } finally { Dispose(); };
            }
        }
    }
}
