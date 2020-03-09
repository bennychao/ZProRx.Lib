using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UniRx.Operators;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Matrix
{
    public class HandObservable<T> : OperatorObservableBase<T>, IObserver<T>
    {
        //Subject<T> subject = new Subject<T>();
        Subject<T> subject = new Subject<T>();

        bool bHand = false;
        public bool IsHand => bHand;

        public HandObservable()
            : base(false)
        {
        }

        public void Reset()
        {
            bHand = false;

            //subject.Replay();
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {

            subject = new Subject<T>();
            var nw = new Where(this, observer, cancel);
            return subject.Subscribe(nw);
           // return nw;
        }

        public async Task Check()
        {
            if (bHand)
            {
                await Task.Yield();
            }
            else
            {
                await this.Fetch().ToTask();
            }
        }

        public void OnCompleted()
        {
            subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            subject.OnError(error);
        }

        public void OnNext(T value)
        {
            bHand = true;
            subject.OnNext(value);
        }

        class Where : OperatorObserverBase<T, T>
        {
            readonly HandObservable<T> parent;

            public Where(HandObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                this.parent.bHand = true;
                observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }
}
