using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Entity;
using UniRx;
using System.Threading.Tasks;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix
{
    public static class PipelineObservableExtension
    {

        static public IDisposable SubscribePackage(this IObservable<RoundPackage> observable, Func<int,  Task> func)
        {
            var ret = observable.Subscribe(async a =>

                await func(a.CurRound.Value)

            );

            return ret;
        }

        static public IDisposable SubscribePackage(this IObservable<RoundPackage> observable, Action<int> func)
        {
            var ret = observable.Subscribe(a =>
            {
                func(a.CurRound.Value);
            });

            return ret;
        }

        static public IDisposable SubscribePackage(this IObservable<RoundPackage> observable, Func<string, Task> func)
        {
            var ret = observable.Subscribe(async a =>

                await func(a.CurClientId.Value)

            );

            return ret;
        }

        static public IDisposable SubscribePackage(this IObservable<RoundPackage> observable, Action<string> func)
        {
            var ret = observable.Subscribe(a =>
            {
                func(a.CurClientId.Value);
            });

            return ret;
        }


        static public IDisposable SubscribePackage<TCmd, TData>(this IObservable<CmdPackage<TCmd>> observable, Func<TCmd, TData, Task> func)
        {
            var ret = observable.Subscribe(async a => 
                
                await func(a.Cmd, a.GetData<TData>())
                
            );

            return ret;
        }

        static public IDisposable SubscribePackage<TCmd, TData>(this IObservable<CmdPackage<TCmd>> observable, Action<TCmd, TData> func)
        {
            var ret = observable.Subscribe(a =>
            {
                 func(a.Cmd, a.GetData<TData>());
            });

            return ret;
        }

        static public IDisposable SubscribePackage<TAction, TData>(this IObservable<SyncFrameActionPackage<TAction>> observable, Func<TAction, TData, Task> func)
        {
            var ret = observable.Subscribe(async a =>

                await func(a.Action, a.GetData<TData>())

            );

            return ret;
        }

        static public IDisposable SubscribePackage<TAction, TData>(this IObservable<SyncFrameActionPackage<TAction>> observable, Action<TAction, TData> func)
        {
            var ret = observable.Subscribe( a =>

                func(a.Action, a.GetData<TData>())

            );

            return ret;
        }

    }
}
