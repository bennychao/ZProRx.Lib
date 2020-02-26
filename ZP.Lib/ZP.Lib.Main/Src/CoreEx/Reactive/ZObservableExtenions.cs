using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UniRx;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.Main.CommonTools;

namespace ZP.Lib.CoreEx
{
    public  static class ZObservableExtensions
    {
#if ZP_SERVER
        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source)
        {
            return source.ObserveOn(ZPRxScheduler.CurrentScheduler);
        }

#endif

        public static IObservable<T> Fetch<T>(this IObservable<T> source)
        {
            return new FetchOperator<T>(source);
        }

        public static void WaitAFetch<T>(this IObservable<T> source)
        {
            var observ = source.Fetch();
            observ.ToTask().Wait();
        }

        public static void WaitTask<T>(this IObservable<T> source)
        {
            source.ToTask().Wait();
        }

        public static void WaitAFetch<T>(this IObservable<T> source, float timeoutSec)
        {
            source.Fetch().Timeout(TimeSpan.FromSeconds(timeoutSec)).ToTask().Wait();
        }

        public static void WaitTask<T>(this IObservable<T> source, float timeoutSec)
        {
            source.Timeout(TimeSpan.FromSeconds(timeoutSec)).ToTask().Wait();
        }

        public static void WaitAFetch<T>(this IObservable<T> source, float timeoutSec, TimeoutException error)
        {
            source.Fetch().Timeout(TimeSpan.FromSeconds(timeoutSec)).Catch((TimeoutException e) =>
            {
                //throw new Exception
                throw error;
            }).ToTask().Wait();
        }

        public static void WaitTask<T>(this IObservable<T> source, float timeoutSec, TimeoutException error)
        {
            source.Timeout(TimeSpan.FromSeconds(timeoutSec)).Catch((TimeoutException e) =>
            {
                //throw new Exception
                throw error;
            }).ToTask().Wait();
        }

        public static T FetchResult<T>(this IObservable<T> source)
        {
            var observ = source.Fetch();
            return observ.Timeout(TimeSpan.FromSeconds(2)).Catch((TimeoutException e) =>
            {
                //throw new Exception
                throw new Exception("FetchResult Time out");

            }).ToTask().Result;
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

        public static IObservable<T> ToDisposable<T>(this IObservable<T> source, Action dispFunc)
        {
            return Observable.Create<T>(observer =>
            {
                MultiDisposable disposables = new MultiDisposable();
                source.Subscribe(observer).AddTo(disposables);

                disposables.Add(new FunctionDispose(dispFunc));

                return disposables;
            });
        }

        public static IRawPackageObservable<T> ToDisposable<T>(this IRawPackageObservable<T> source, Action dispFunc)
        {
            return (source as IObservable<SocketPackageHub<T>>)?.ToDisposable(dispFunc) as IRawPackageObservable<T>;
        }

        public static IRawPackageObservable<T, TErrorEnum> ToDisposable<T, TErrorEnum>(
            this IRawPackageObservable<T, TErrorEnum> source, Action dispFunc)
        {
            return (source as IObservable<SocketPackageHub<T, TErrorEnum>>)?.ToDisposable(dispFunc) as IRawPackageObservable<T, TErrorEnum>;
        }

        public static IObservable<T> Counted<T>(this IObservable<T> source, InterCountReactiveProperty counter)
        {
            return source.Do(_=> {
                counter.Increment();
            });
        }

    }
}
