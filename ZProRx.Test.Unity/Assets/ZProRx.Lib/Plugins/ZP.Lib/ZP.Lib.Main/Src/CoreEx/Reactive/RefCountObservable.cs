using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;

namespace ZP.Lib.CoreEx
{
    public class RefCountObservable<T> : OperatorObservableBase<T>
    {
        readonly protected IObservable<T> source;
        readonly protected RefCountDisposable refCountDisposable;// = new RefCountDisposable();

        /// <summary>
        /// Create RefCount Observable
        /// </summary>
        /// <param name="source"></param>
        /// <param name="disposable">Underlying disposable. when Ref count is 0 will Call this Dispose</param>
        public RefCountObservable(IObservable<T> source, IDisposable disposable)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            refCountDisposable = new RefCountDisposable(disposable);
        }

        public RefCountObservable(IObservable<T> source)
             : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;

            var self = this as IDisposable;

            if (self == null)
                throw new Exception("Self is not Disposable, Please set the outer disposable");

            refCountDisposable = new RefCountDisposable(self);
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new CompositeDisposable(new IDisposable[]
            {
                refCountDisposable.GetDisposable(),
                source.Subscribe(new InnerObserver(this, observer, cancel))
            }) ;
        }

        class InnerObserver : OperatorObserverBase<T, T>
        {
            readonly RefCountObservable<T> parent;

            public InnerObserver(RefCountObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                try
                {
                    observer.OnCompleted();
                }
                finally { Dispose(); };
            }
        }
    }
}
