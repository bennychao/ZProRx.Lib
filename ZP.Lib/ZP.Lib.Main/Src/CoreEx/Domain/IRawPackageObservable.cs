using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.CoreEx.Reactive
{
    public interface IRawPackageObservable<T> : IObservable<SocketPackageHub<T>>
    {

    }

    public interface IRawPackageObservable<T, TErrorEnum> : IObservable<SocketPackageHub<T, TErrorEnum>>
    {

    }
}
