using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc;

namespace ZP.Lib.Matrix.Domain
{
    public interface IRoomClient : IChannelCollection
    {
        IObservable<Unit> OnRunObservable { get; }

        IObservable<Unit> OnStopObservable { get; }

        void Connect(int roomId);
        void Disconnect();

        ZRoom ZRoom { get; }

        void SendPackage<T>(string action, T data);

        void PostPackage<T>(string action, T data);

        IChannelClient GetGroupChannelClient(string channelName);

        IChannelClient GetUniChannelClient(string channelName);

        bool IsRunInClient(int schedulerId);

        Task RunInClient(Action action);
        Task RunInClient(Func<Task> function);
    }
}
