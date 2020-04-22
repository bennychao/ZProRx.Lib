using System;
using UniRx;

namespace ZP.Lib.Matrix.Domain
{
    public interface IBroadCast
    {
        IObservable<ZNull> BroadCast<T>(string action, T data = default(T));

        IObservable<ZNull> BroadCastMsg<T>(T data);
    }
}
