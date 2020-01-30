using System;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.CoreEx;
using ZP.Lib.Net;
using ZP.Lib.Common;
using ZP.Lib.Server.CommonTools;

namespace ZP.Lib.CoreEx.Reactive
{
    public static class ObservableWithResponsableExtension
    {

        // SubscribeAsync Func<T, Task>
        static public IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> func)
        {
            var ret = observable.Subscribe(a =>
            {
                var r = func(a);
            });

            return ret;
        }

        static public IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> func
            ,Action onCompleted)
        {
            var ret = observable.Subscribe(a =>
            {
                var r = func(a);
            }, ZFunctions.ThrowNextAction, onCompleted);

            return ret;
        }

        static public IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> func
            , Action<Exception> onError)
        {
            var ret = observable.Subscribe(a =>
            {
                var r = func(a);
            }, onError, ZFunctions.NullAction);

            return ret;
        }

        static public IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> func
            , Action<Exception> onError, Action onCompleted)
        {
            var ret = observable.Subscribe(a =>
            {
                var r = func(a);
            }, onError, onCompleted);

            return ret;
        }

        //task is will get
        //Subscribe Func<T, TResult>
        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, TResult> func
            , Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribe<T, TResult>(observable, func, onError, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, TResult> func
            , Action<Exception> onError)
        {
            return InnerSubscribe<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, TResult> func, Action onCompleted)
        {
            return InnerSubscribe<T, TResult>(observable, func, ZFunctions.ThrowNextAction , onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, TResult> func)
        {
            return InnerSubscribe<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static internal IDisposable InnerSubscribe<T, TResult>(IObservable<T> observable, Func<T, TResult> func
            , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            if (resultDo == null)
                throw new Exception("Result Type is not match");

            var ret = observable.Subscribe(a =>
            {
                //if (resultDo == null)
                //    throw new Exception("can't support INetResponse");
                try
                {
                    var r = func(a);

                    resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    resultDo.SetError(e);
                }

            }, onError, onCompleted);

            return ret;
        }


        //Subscribe Func<T, Task<TResult>>
        static public IDisposable Subscribe<T, TResult>(this INetResponsable<TResult> observable, Func<T, Task<TResult>> func)
        {
            return InnerSubscribetResponsable<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this INetResponsable<TResult> observable, Func<T, Task<TResult>> func, Action<Exception> onError)
        {
            return InnerSubscribetResponsable<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this INetResponsable<TResult> observable, Func<T, Task<TResult>> func, Action onCompleted)
        {
            return InnerSubscribetResponsable<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this INetResponsable<TResult> observable, Func<T, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribetResponsable<T, TResult>(observable, func, onError, onCompleted);
        }

        static internal IDisposable InnerSubscribetResponsable<T, TResult>(INetResponsable<TResult> observable, Func<T, Task<TResult>> func
            , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            if (resultDo == null)
                throw new Exception("Can't support INetResponse");

            var ret = (observable as IObservable<T>)?.SubscribeAsync(async a =>
            {
                try
                {
                    var r = await func(a);

                    resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    resultDo.SetError(e);
                }


            }, onError, onCompleted);

            return ret;
        }

        // Subscribe Func<T, Task<object>>
        static public IDisposable Subscribe<T>(this INetResponsable<IRawDataPref> observable, Func<T, Task<object>> func)
        {
            return InnerSubscribe<T>(observable, func,  ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this INetResponsable<IRawDataPref> observable, Func<T, Task<object>> func, Action<Exception> onError)
        {
            return InnerSubscribe<T>(observable, func,  onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this INetResponsable<IRawDataPref> observable, Func<T, Task<object>> func, Action onCompleted)
        {
            return InnerSubscribe<T>(observable, func,  ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T>(this INetResponsable<IRawDataPref> observable, Func<T, Task<object>> func
            , Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribe<T>(observable, func, onError, onCompleted);
        }

        static internal IDisposable InnerSubscribe<T>(INetResponsable<IRawDataPref> observable, Func<T, Task<object>> func
            , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<IRawDataPref>;
            //if (resultDo == null)
            //    throw new Exception("Can't support INetResponse");

            var ret = (observable as IObservable<T>)?.SubscribeAsync(async a =>
            {
                try
                {
                    var r = await func(a);

                    resultDo?.SetResult(ZPropertyPrefs.ConvertToRawData(r));
                }
                catch (ZNetException e)
                {
                    resultDo?.SetError(e.Error);
                }
                catch (Exception e)
                {
                    resultDo?.SetError(e);
                }
            }, onError, onCompleted);

            return ret;
        }


        //Subscribe Func<T, TResult>
        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, TResult> func)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, TResult> func, Action<Exception> onError)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, TResult> func, Action onCompleted)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, TResult> func, 
            Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, onError, onCompleted);
        }

        static internal IDisposable InnerSubscribe<T, TResult, TErrorEnum>(IObservable<T> observable, Func<T, TResult> func
             , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<NetPackage<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>>;
            if (resultDo == null)
                throw new Exception("can't support INetResponse");

            var ret = observable.Subscribe(a =>
            {
                try
                {
                    var r = func(a);

                    (resultDo as INetResponsable<TResult>).SetResult(r);
                }
                catch (ZNetException e)
                {
                    resultDo.SetError(e.Error);
                }
                //support custom Error
                catch (ZNetException<TErrorEnum> e)
                {
                    (resultDo as INetResponsable<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>).SetError(e.Error);
                }
                catch (Exception e)
                {
                    resultDo.SetError(e);
                }

            }, onError, onCompleted);

            return ret;
        }


        // Subscribe Func<T, Task<TResult>>
        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, Task<TResult>> func)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, Task<TResult>> func, Action<Exception> onError)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, Task<TResult>> func, Action onCompleted)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult, TErrorEnum>(this IObservable<T> observable, Func<T, Task<TResult>> func
            , Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribe<T, TResult, TErrorEnum>(observable, func, onError, onCompleted);
        }

        static internal IDisposable InnerSubscribe<T, TResult, TErrorEnum>(IObservable<T> observable, Func<T, Task<TResult>> func
            , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<NetPackage<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>>;
            if (resultDo == null)
                throw new Exception("can't support INetResponse");

            var ret = observable.SubscribeAsync(async a =>
            {

                try
                {
                    var r = await func(a);

                    (resultDo as INetResponsable<TResult>).SetResult(r);
                }
                catch (ZNetException e)
                {
                    resultDo.SetError(e.Error);
                }
                catch (ZNetException<TErrorEnum> e)
                {
                    (resultDo as INetResponsable<TResult, MultiEnum<ZNetErrorEnum, TErrorEnum>>).SetError(e.Error);
                }

            }, onError, onCompleted);

            return ret;
        }


        //Subscribe  Func<T, NetPackage<TResult, ZNetErrorEnum>>
        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, NetPackage<TResult, ZNetErrorEnum>> func)
        {
            return InnerSubscribe<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, NetPackage<TResult, ZNetErrorEnum>> func, Action<Exception> onError)
        {
            return InnerSubscribe<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, NetPackage<TResult, ZNetErrorEnum>> func, Action onCompleted)
        {
            return InnerSubscribe<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, NetPackage<TResult, ZNetErrorEnum>> func
            , Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribe<T, TResult>(observable, func, onError, onCompleted);
        }

        static internal IDisposable InnerSubscribe<T, TResult>(IObservable<T> observable, Func<T, NetPackage<TResult, ZNetErrorEnum>> func
            , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<NetPackage<TResult, ZNetErrorEnum>>;
            if (resultDo == null)
                throw new Exception("can't support INetResponse");

            var ret = observable.Subscribe(a =>
            {
                var r = func(a);

                resultDo.SetResult(r);

            }, onError, onCompleted);

            return ret;
        }


        //Subscribe with  Func<T, Task<NetPackage<TResult, ZNetErrorEnum>>>
        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, Task<NetPackage<TResult, ZNetErrorEnum>>> func)
        {
            return InnerSubscribe<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, Task<NetPackage<TResult, ZNetErrorEnum>>> func, Action<Exception> onError)
        {
            return InnerSubscribe<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, Task<NetPackage<TResult, ZNetErrorEnum>>> func, Action onCompleted)
        {
            return InnerSubscribe<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<T> observable, Func<T, Task<NetPackage<TResult, ZNetErrorEnum>>> func
            , Action<Exception> onError, Action onCompleted)
        {
            return InnerSubscribe<T, TResult>(observable, func, onError, onCompleted);
        }

        static internal IDisposable InnerSubscribe<T, TResult>(IObservable<T> observable, Func<T, Task<NetPackage<TResult, ZNetErrorEnum>>> func
            , Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<NetPackage<TResult, ZNetErrorEnum>>;
            if (resultDo == null)
                throw new Exception("can't support INetResponse");

            var ret = observable.SubscribeAsync(async a =>
            {

                var r = await func(a);

                resultDo.SetResult(r);

            }, onError, onCompleted);

            return ret;
        }

    }
}
