using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc;
using ZP.Lib.Soc.Domain;

namespace ZP.Lib.Soc.Domain
{
   public  interface ISocRoomBuilder
    {
        //void AddChannel(BaseChannel channel);

        int Port { get; }

        ISocRoomBuilder AddChannelType<T>() where T : BaseChannel;

        IEnumerable<Type> ChannelTypes { get; }

        IRoomServer AddRoom(ZRoom room);

        void Build();

        void Unbuild();


        IRoomServer FindRoom(uint roomId);

        bool IsInBuilding { get; }

        int ClientCount { get; }

        Type FindChannelType(string name);

        IObservable<IRoomServer> OnConnected { get; }

        IObservable<IRoomServer> OnDisConnected { get; }
    }
}
