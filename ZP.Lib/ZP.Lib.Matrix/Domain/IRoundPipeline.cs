using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix.Domain
{
    public interface IRoundPipeline
    {
        int ClientCount { get; }

        List<string> Clients { get; }

        IObservable<Unit> OnReadyObservable { get; }

        IObservable<Unit> OnStartObservable { get; }

        IObservable<Unit> OnStopObservable { get; }

        IObservable<RoundPackage> OnTickObservable { get; }

        IObservable<RoundPackage> OnRoundObservable { get; }

        //when some client connected 
        IObservable<string> OnConnectedObservable { get; }

        //when some client disconnected
        IObservable<string> OnDisConnectedObservable { get; }

        IObservable<ZNull> Ready();

        IObservable<ZNull> Start();

        IObservable<ZNull> Stop();
    }
}
