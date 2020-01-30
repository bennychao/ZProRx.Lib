using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.CoreEx;
using ZP.Lib;
using UniRx;
using System.Threading.Tasks;
using ZP.Lib.Net;
using UnityEngine;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.Core.Main;
using ZP.Lib.Core.Domain;
using UniRx.Operators;
using System.Threading;
using ZP.Lib.Server.CommonTools;

namespace ZP.Lib.CoreEx.Reactive
{
    //for Receive package Observable with not response
    public static class PackageObservableExtensions
    {        
        public static IObservable<T> ResponseOn<T>(this IObservable<T> observable, IScheduler scheduler)
        {
            var resultDo = observable as IObservableWithScheduler;
            if (resultDo != null)
                resultDo.scheduler = scheduler;

            return observable;
        }

        public static IRawPackageObservable<T> ResponseOn<T>(this IRawPackageObservable<T> observable, IScheduler scheduler)
        {
            var resultDo = observable as IObservableWithScheduler;
            if (resultDo != null)
                resultDo.scheduler = scheduler;
            else
            {
                throw new Exception("ResponseOn Error ");
            }
            return observable;
        }

        //no return Action<T, ISocketPackage>
        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable, Action<T, ISocketPackage> func)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable, 
            Action<T, ISocketPackage> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable, 
            Action<T, ISocketPackage> func,  Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable,
            Action<T, ISocketPackage> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, onCompleted);
        }


        //Action<T, ISocketPackage>
        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, Action<T, ISocketPackage> func)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, 
            Action<T, ISocketPackage> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, 
            Action<T, ISocketPackage> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, 
            Action<T, ISocketPackage> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, onCompleted);
        }

        static private IDisposable SubscribeRecvPackage<T>(
            IObservable<SocketPackageHub<T>> observable, 
            Action<T, ISocketPackage> func, Action<Exception> onError, Action onCompleted)
        {
            var ret = observable.Subscribe(a =>
            {
                //var resultDo = observable as INetResponse<TResult>;
                func(a.data.Data, a.socketPackage);
                //resultDo.SetResult(r);
            }, onError, onCompleted);

            return ret;
        }

        static internal IDisposable SubscribeRecvPackage<T>(IRawPackageObservable<T> observable,
            Action<T, ISocketPackage> func, Action<Exception> onError, Action onCompleted)
        {
            var ret = observable.Subscribe(a =>
            {
                //var resultDo = observable as INetResponse<TResult>;

                func(a.data.Data, a.socketPackage);

                //resultDo.SetResult(r);

            }, onError, onCompleted);

            return ret;
        }

        //class ReceivePackageObserver<T, TResult> : IObserver<T>
        //{
        //    readonly Func<T, ISocketPackage, TResult> onNext;
        //    readonly Action<Exception> onError;
        //    readonly Action onCompleted;

        //    int isStopped = 0;

        //    public ReceivePackageObserver(
        //        INetResponsable<TResult> resultDo, 
        //        INetResponsableWithClientId<TResult> resultDoById,
        //        Func<T, ISocketPackage, TResult> onNext, Action<Exception> onError, Action onCompleted)
        //    {
        //        this.onNext = onNext;
        //        this.onError = onError;
        //        this.onCompleted = onCompleted;
        //    }

        //    public void OnNext(T a)
        //    {
        //        if (isStopped == 0)
        //        {
        //            try
        //            {
        //                var r = onNext(a.data.Data, a.socketPackage);

        //                if (resultDoById != null)
        //                    resultDoById.SetResult(a.socketPackage.Key, r);
        //                else
        //                    resultDo.SetResult(r);
        //            }
        //            catch (ZNetException e)
        //            {
        //                if (resultDoById != null)
        //                    resultDoById.SetError(a.socketPackage.Key, e.Error);
        //                else
        //                    resultDo.SetError(e.Error);
        //            }
        //            catch (Exception e)
        //            {
        //                if (resultDoById != null)
        //                    resultDoById.SetError(a.socketPackage.Key, e);
        //                else
        //                    resultDo.SetError(e);
        //            }
        //        }
        //    }

        //    public void OnError(Exception error)
        //    {
        //        if (Interlocked.Increment(ref isStopped) == 1)
        //        {
        //            onError(error);
        //        }
        //    }


        //    public void OnCompleted()
        //    {
        //        if (Interlocked.Increment(ref isStopped) == 1)
        //        {
        //            onCompleted();
        //        }
        //    }
        //}
    }
}