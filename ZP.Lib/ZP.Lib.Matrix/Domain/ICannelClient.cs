using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix.Domain
{
    public interface IChannelClient : IDisposable
    {
        IObservable<ZNull> Connect();

        IObservable<ZNull> Connect2();

        void Disconnect();

        IObservable<ZChannelInfo> GetInfo();

        ChannelStatusEnum Status { get; }

        IObservable<ChannelStatusEnum> StatusChanged { get; }

        List<string> GetActions();

        IObservable<TResult> Send<T, TResult>(string action, T data);

        IObservable<Unit> Post<T>(string action, T data);

    }
}
