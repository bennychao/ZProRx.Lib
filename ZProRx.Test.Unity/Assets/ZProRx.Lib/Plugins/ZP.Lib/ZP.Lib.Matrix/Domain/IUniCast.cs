using System;
using UniRx;

namespace ZP.Lib.Matrix.Domain
{
    public interface IUniCast
    {
        IObservable<TResult> UniCast<T, TResult>(string curClientId, string action, T data);

        IObservable<ZNull> UniCast<T>(string curClientId, string action, T data);

        IObservable<TResult> UniCastMsg<T, TResult>(string curClientId, T data);

        IObservable<ZNull> UniCastMsg<T>(string curClientId, T data);
    }
}
