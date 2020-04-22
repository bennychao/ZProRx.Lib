using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.CoreEx
{
    public interface IRawPackageObservable<T> : IObservable<SocketPackageHub<T>>
    {

    }

    public interface IRawPackageObservable<T, TErrorEnum> : IObservable<SocketPackageHub<T, TErrorEnum>>
    {

    }
}
