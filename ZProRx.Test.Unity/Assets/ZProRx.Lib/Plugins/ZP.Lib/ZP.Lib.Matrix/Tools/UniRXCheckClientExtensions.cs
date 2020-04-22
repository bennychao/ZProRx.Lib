using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Matrix
{
    public static class UniRXCheckClientExtensions
    {
        public static IObservable<T> CheckClient<T>(this IObservable<T> source, Func<string, bool> predicate) 
        {
            return new CheckClientObservable<T>(source, predicate);
        }
    }

    public class CheckClientObservable<T> : OperatorObservableBase<T>
    {
        public IObservable<T> source;
        readonly Func<string, bool> predicate;
        public CheckClientObservable(IObservable<T> source, Func<string, bool> predicate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var nw = new Where(this, observer, cancel) ;
            var vr = source.Subscribe(nw);

            nw.socketResponse = (vr as ISocketPackageGetable)?.SocketPackage;

            return vr;
        }
        class Where : OperatorObserverBase<T, T>
        {
            readonly CheckClientObservable<T> parent;

            public ISocketPackage socketResponse;

            public Where(CheckClientObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                var isPassed = false;
                try
                {
                    //get ID
                    var obserId = socketResponse?.Key;
                        //this.parent.source as IObservableWithId<string>;
                    if (obserId != null)
                        isPassed = parent.predicate(obserId);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }

                if (isPassed)
                {
                    observer.OnNext(value);
                }
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
