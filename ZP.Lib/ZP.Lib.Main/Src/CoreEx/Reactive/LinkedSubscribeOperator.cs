using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;

namespace ZP.Lib.CoreEx.Reactive
{
    //linked Subscribe, same as Do Operator, but not need Subscribed
    public class LinkedSubscribeOperator<T> : OperatorObservableBase<T>
    {
        readonly protected IObservable<T> source;

        readonly Action<T> onNextAction = null;
        readonly Action<Exception> onErrorAction = null;

        readonly Action onCompletedAction = null;

        public LinkedSubscribeOperator(IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
             : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onNextAction = onNext;
            this.onErrorAction = onError;
            this.onCompletedAction = onCompleted;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new InnerObserver(this, observer, cancel));
        }

        class InnerObserver : OperatorObserverBase<T, T>
        {
            readonly LinkedSubscribeOperator<T> parent;

            public InnerObserver(LinkedSubscribeOperator<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                if (parent.onNextAction != null)
                    parent.onNextAction(value);

                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try {
                    if (parent.onErrorAction != null)
                        parent.onErrorAction(error);

                    observer.OnError(error); 
                } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                try
                {
                    if (parent.onCompletedAction != null)
                        parent.onCompletedAction();

                    observer.OnCompleted();
                }
                finally { Dispose(); };
            }
        }
    }
}
