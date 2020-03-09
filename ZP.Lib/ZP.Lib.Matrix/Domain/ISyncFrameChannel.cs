using System;
using ZP.Lib;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix.Domain
{
    public interface ISyncFrameChannel
    {
        IObservable<string> OnConnectedObservable { get; }

        IObservable<string> OnDisConnectedObservable { get; }

        IObservable<SyncFrameUpdatePackage> FrameObservable { get; }

        IObservable<ZNull> Start();
        IObservable<ZNull> Stop();
    }
}
