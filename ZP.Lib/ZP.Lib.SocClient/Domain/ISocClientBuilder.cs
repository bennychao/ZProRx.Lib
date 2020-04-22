using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc;

namespace ZP.Lib.SocClient.Domain
{
    public interface ISocClientBuilder
    {
        IObservable<IRoomClient> OnConnected { get; }

        IObservable<IRoomClient> OnDisConnected { get; }


        ISocClientBuilder AddChannelType<T>() where T : BaseChannel;

        IEnumerable<Type> ChannelTypes { get; }


        //create a client client to zRoom
        IRoomClient CreateClient(string clientId, int roomId);
    }
}
