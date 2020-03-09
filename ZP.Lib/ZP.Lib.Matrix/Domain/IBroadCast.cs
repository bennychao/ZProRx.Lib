using System;
using UniRx;

namespace ZP.Lib.Matrix.Domain
{
    public interface IBroadCast
    {
        IObservable<Unit> BroadCast<T>(string action, T data = default(T));

        IObservable<Unit> BroadCastMsg<T>(T data);
    }
}
