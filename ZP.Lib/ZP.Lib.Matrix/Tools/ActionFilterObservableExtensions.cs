using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UniRx;
using UniRx.Operators;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix
{
    //for check the channel's Function, ex. check token
    static public class ActionFilterObservableExtensions
    {
        public static IRawPackageObservable<T> ResponceFilter<T>(this IRawPackageObservable<T> source, MethodInfo methodInfo, BaseChannel baseChannel)
        {
            return new ActionFilterObservable<T>(source, methodInfo, baseChannel);
        }
    }


    public class ActionFilterObservable<T> : PackageOperatorObservableBase<T>
        //where T : SocketResponseHub<T>
    {
        public IObservable<SocketPackageHub<T>> source;

        readonly MethodInfo methodInfo;
        readonly BaseChannel baseChannel;
        public ActionFilterObservable(IObservable<SocketPackageHub<T>> source, MethodInfo methodInfo, BaseChannel baseChannel)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            
            this.methodInfo = methodInfo;

            this.baseChannel = baseChannel;
        }

        protected override IDisposable SubscribeCore(IObserver<SocketPackageHub<T>> observer, IDisposable cancel)
        {
            var nw = new Where(this, observer, cancel);
            var vr = source.Subscribe(nw);

            nw.socketResponse = (vr as ISocketPackageGetable)?.SocketPackage;

            return vr;
        }
        class Where : OperatorObserverBase<SocketPackageHub<T>, SocketPackageHub<T>>
        {
            readonly ActionFilterObservable<T> parent;

            public ISocketPackage socketResponse;

            public Where(ActionFilterObservable<T> parent, IObserver<SocketPackageHub<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(SocketPackageHub<T> value)
            {
                var isPassed = false;
                try
                {
                    //get ID
                    var obserId = socketResponse?.Key;
                    //this.parent.source as IObservableWithId<string>;
                    //if (obserId != null)
                    //    isPassed = parent.predicate(obserId);
                    var t = parent.methodInfo.GetCustomAttribute<ActionFilterAttribute>()?.FilterType;

                    var filter = (t != null ? Activator.CreateInstance(t, new object[] { parent.baseChannel}) as IActionFillter<T> : null) ;

                   var valueF = filter?.OnFilter(value) ?? value;

                    observer.OnNext(valueF);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
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
