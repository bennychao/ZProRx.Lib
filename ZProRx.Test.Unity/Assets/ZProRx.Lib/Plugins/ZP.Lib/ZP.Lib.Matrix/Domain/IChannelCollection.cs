using System;
using System.Collections.Generic;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix.Domain
{
    public interface IChannelCollection
    {
        bool CheckChannel(string channelName);

        void AddChannel(BaseChannel channel);

        IChannelClient GetChannelClient(string channelName);

        IChannelClient GetChannelClient(string channelName, string clientId);

        //TChannel GetChannel<TChannel>(Predicate<TChannel> predicate) where TChannel : BaseChannel;

        //return defined channel in runtime by type
        TChannel GetChannel<TChannel>() where TChannel : BaseChannel;

        BaseChannel GetChannel(string channelName);

        IEnumerable<TChannel> GetChannels<TChannel>() where TChannel : BaseChannel;
    }
}
