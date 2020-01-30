using System;
using System.Collections.Generic;
using System.Text;
using UniRx.Operators;
using ZP.Lib.Net;
using ZP.Lib.CoreEx.Reactive;

namespace ZP.Lib.CoreEx.Reactive
{
    abstract internal class PackageOperatorObservableBase<T, TErrorEnum> 
        : MultiOperatorObservableBase<SocketPackageHub<T, TErrorEnum>>,
        IRawPackageObservable<T, TErrorEnum>
    {
        public string Url;

        public PackageOperatorObservableBase(bool isRequiredSubscribeOnCurrentThread) : base(isRequiredSubscribeOnCurrentThread)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

#if ZP_SERVER
            TTPServer.Instance.UnSubscribe(Url);
#else
            TTPClient.Instance.UnSubscribe(Url);
#endif

        }
    }

    abstract public class PackageOperatorObservableBase<T>
    : MultiOperatorObservableBase<SocketPackageHub<T>>,
    IRawPackageObservable<T>
    {
        public string Url;

        public PackageOperatorObservableBase(bool isRequiredSubscribeOnCurrentThread) : base(isRequiredSubscribeOnCurrentThread)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

#if ZP_SERVER
            TTPServer.Instance.UnSubscribe(Url);
#else
            TTPClient.Instance.UnSubscribe(Url);
#endif
        }
    }
}
