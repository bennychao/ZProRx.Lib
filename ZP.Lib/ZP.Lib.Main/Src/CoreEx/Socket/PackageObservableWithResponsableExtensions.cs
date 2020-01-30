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
    //for Receive package Observable and Support Response
    public static class PackageObservableWithResponsableExtensions
    {


        //Package Observalbe With Responsable Func<T, ISocketPackage, Task>
        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, Task> func)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, Task> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable,
            Func<T, ISocketPackage, Task> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T>(this IObservable<SocketPackageHub<T>> observable,
             Func<T, ISocketPackage, Task> func, Action<Exception> onError,  Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, onCompleted);
        }


        //Package Observalbe With Responsable Func<T, ISocketPackage, TResult>
        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, TResult> func)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }
        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }
        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, TResult> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }
        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, onCompleted);
        }


        //Package Observalbe With Responsable Func<T, ISocketPackage, Task<TResult>>
        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, Task<TResult>> func)
        {           
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable,
            Func<T, ISocketPackage, Task<TResult>> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable,
            Func<T, ISocketPackage, Task<TResult>> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IObservable<SocketPackageHub<T>> observable,
            Func<T, ISocketPackage, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, onCompleted);
        }

        //Package Observalbe With Responsable Func<T, ISocketPackage, TResult>
        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable,
            Func<T, ISocketPackage, TResult> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable,
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, onCompleted);
        }

        //Package Observalbe With Responsable Func<IRawDataPref, ISocketPackage, TResult>
        static public IDisposable Subscribe<TResult>(this IRawPackageObservable<IRawDataPref> observable, 
            Func<IRawDataPref, ISocketPackage, TResult> func)
        {
            return SubscribeRecvPackage<TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<TResult>(this IRawPackageObservable<IRawDataPref> observable,
            Func<IRawDataPref, ISocketPackage, TResult> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<TResult>(this IRawPackageObservable<IRawDataPref> observable,
            Func<IRawDataPref, ISocketPackage, TResult> func, Action onCompleted)
        {
            return SubscribeRecvPackage<TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<TResult>(this IRawPackageObservable<IRawDataPref> observable,
            Func<IRawDataPref, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<TResult>(observable, func, onError, onCompleted);
        }




        //Package Observalbe With Responsable Func<T, ISocketPackage, Task>
        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, Func<T, ISocketPackage, Task> func)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }
        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T>(observable, func, onError, onCompleted);
        }


        //Package Observalbe With Responsable Func<T, TResult>
        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, Func<T, TResult> func)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, TResult> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, TResult> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, onCompleted);
        }


        // Func<T, Task<TResult>> func
        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, Func<T, Task<TResult>> func)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, Task<TResult>> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, Task<TResult>> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, onCompleted);
        }

        //Func<T, ISocketPackage, Task<TResult>>
        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, Func<T, ISocketPackage, Task<TResult>> func)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task<TResult>> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task<TResult>> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TResult>(this IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TResult>(observable, func, onError, onCompleted);
        }

        //Func<T, ISocketPackage, TResult>
        static public IDisposable Subscribe<T, TError, TResult>(this IRawPackageObservable<T, TError> observable, Func<T, ISocketPackage, TResult> func)
        {
            return SubscribeRecvPackage<T, TError, TResult>(observable, func, ZFunctions.ThrowNextAction, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TError, TResult>(this IRawPackageObservable<T, TError> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError)
        {
            return SubscribeRecvPackage<T, TError, TResult>(observable, func, onError, ZFunctions.NullAction);
        }

        static public IDisposable Subscribe<T, TError, TResult>(this IRawPackageObservable<T, TError> observable, 
            Func<T, ISocketPackage, TResult> func, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TError, TResult>(observable, func, ZFunctions.ThrowNextAction, onCompleted);
        }

        static public IDisposable Subscribe<T, TError, TResult>(this IRawPackageObservable<T, TError> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            return SubscribeRecvPackage<T, TError, TResult>(observable, func, onError, onCompleted);
        }


        //inner Functions
        static internal IDisposable SubscribeRecvPackage<T>(IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, Task> func, Action<Exception> onError, Action onCompleted)
        {
            var ret = observable.Subscribe(async a =>
            {
                //var resultDo = observable as INetResponse<TResult>;
                await func(a.data.Data, a.socketPackage);
                //resultDo.SetResult(r);
            }, onError, onCompleted);

            return ret;
        }


        static internal IDisposable SubscribeRecvPackage<T, TResult>(IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(a =>
            {
                try
                {
                    var r = func(a.data.Data, a.socketPackage);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else
                        resultDo.SetError(e);
                }
            }, onError, onCompleted);

            return ret;
        }

        static internal IDisposable SubscribeRecvPackage<T, TResult>(IObservable<SocketPackageHub<T>> observable, 
            Func<T, ISocketPackage, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(async a =>
            {
                try
                {
                    var r = await func(a.data.Data, a.socketPackage);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else
                        resultDo.SetError(e);
                }
            }, onError, onCompleted);

            return ret;
        }

        
        static internal IDisposable SubscribeRecvPackage<T, TResult>(IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(a =>
            {
                try
                {
                    var r = func(a.data.Data, a.socketPackage);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else if (resultDo != null)
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else if (resultDo != null)
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else if (resultDo != null)
                        resultDo.SetError(e);
                }
            }, onError, onCompleted);

            return ret;
        }

        static internal IDisposable SubscribeRecvPackage<TResult>(IRawPackageObservable<IRawDataPref> observable,
            Func<IRawDataPref, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<object>;
            var resultDoById = observable as INetResponsableWithClientId<object>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(a =>
            {
                try
                {
                    var r = func(a.data.Data, a.socketPackage);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else if (resultDo != null)
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else if (resultDo != null)
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else if (resultDo != null)
                        resultDo.SetError(e);
                }
            }, onError, onCompleted);

            return null;
        }

        static internal IDisposable SubscribeRecvPackage<T>(IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task> func, Action<Exception> onError, Action onCompleted)
        {
            var ret = observable.Subscribe(async a =>
            {
                //var resultDo = observable as INetResponse<TResult>;

                await func(a.data.Data, a.socketPackage);
                //resultDo.SetResult(r);

            }, onError, onCompleted);

            return ret;
        }

        static internal IDisposable SubscribeRecvPackage<T, TResult>(IRawPackageObservable<T> observable, 
            Func<T, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(a =>
            {
                try
                {
                    var r = func(a.data.Data);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else if (resultDo != null)
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else if (resultDo != null)
                        resultDo.SetError(e);
                }

            }, onError, onCompleted);

            return ret;
        }

        static internal IDisposable SubscribeRecvPackage<T, TResult>(IRawPackageObservable<T> observable, 
            Func<T, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(async a =>
            {

                //if (a.data.Error != Net.ZNetErrorEnum.NoError)
                //{

                //}
                //else
                //{
                try
                {
                    var r = await func(a.data.Data);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else if (resultDo != null)
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else if (resultDo != null)
                        resultDo.SetError(e);
                }

            }, onError,  onCompleted);

            return ret;
        }


        static internal IDisposable SubscribeRecvPackage<T, TResult>(IRawPackageObservable<T> observable, 
            Func<T, ISocketPackage, Task<TResult>> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(async a =>
            {
                try
                {
                    var r = await func(a.data.Data, a.socketPackage);
                    Debug.Log(" Call Set Result  Subscribe  " + a.socketPackage.Key);
                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else if (resultDo != null)
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else if (resultDo != null)
                        resultDo.SetError(e);
                }
            }, onError, onCompleted);

            return ret;
        }

        static internal IDisposable SubscribeRecvPackage<T, TError, TResult>(IRawPackageObservable<T, TError> observable, 
            Func<T, ISocketPackage, TResult> func, Action<Exception> onError, Action onCompleted)
        {
            var resultDo = observable as INetResponsable<TResult>;
            var resultDoById = observable as INetResponsableWithClientId<TResult>;
            if (resultDo == null && resultDoById == null)
                throw new InvalidCastException();

            var ret = observable.Subscribe(a =>
            {
                //TODO
                //if (a.data.Error != 
                try
                {
                    var r = func(a.data.Data, a.socketPackage);

                    if (resultDoById != null)
                        resultDoById.SetResult(a.socketPackage.Key, r);
                    else if (resultDo != null)
                        resultDo.SetResult(r);
                }
                catch (ZNetException e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e.Error);
                    else if (resultDo != null)
                        resultDo.SetError(e.Error);
                }
                catch (Exception e)
                {
                    if (resultDoById != null)
                        resultDoById.SetError(a.socketPackage.Key, e);
                    else if (resultDo != null)
                        resultDo.SetError(e);
                }
            }, onError, onCompleted
            );

            return ret;
        }        


    }
}
