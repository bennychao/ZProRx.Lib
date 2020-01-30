using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZP.Lib.CoreEx.Domain;

namespace ZP.Lib.CoreEx.Reactive
{
    public  static class ZObservableExtensions
    {
        public static IObservable<T> Fetch<T>(this IObservable<T> source)
        {
            return new FetchOperator<T>(source);
        }

        public static IObservable<T> LinkedSubscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return new LinkedSubscribeOperator<T>(source, onNext, null, null);
        }

        public static ICancellableObserver<T> ToCancellable<T>(this IObserver<T> source, CancellationTokenSource cancellationTokenSource)
        {
            return new CancellableOperator<T>(source , cancellationTokenSource);
        }

        public static ICancellableObserver<T> ToCancellable<T>(this IObserver<T> source)
        {
            return new CancellableOperator<T>(source);
        }
    }
}
