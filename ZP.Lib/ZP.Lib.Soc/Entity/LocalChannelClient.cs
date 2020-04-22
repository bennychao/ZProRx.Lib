using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Soc.Entity
{
    //not used
    [Obsolete("not used")]
    internal class LocalChannelClient : IChannelClient
    {
        ZChannel channel;
        IRoomServer roomServer;
         
        public ChannelStatusEnum Status => throw new NotImplementedException();

        public IObservable<ChannelStatusEnum> StatusChanged => channel.Status.ValueChangeAsObservable<ChannelStatusEnum>();

        public LocalChannelClient(ZChannel channel, IRoomServer roomServer)
        {
            this.roomServer = roomServer;
            this.channel = channel;
        }



        public IObservable<ZNull> Connect()
        {
            //find channel Type
            return null;
        }

        public IObservable<ZNull> Connect2()
        {
            //find channel Type
            return null;
        }

        public void Disconnect()
        {
            //throw new NotImplementedException();
        }

        public List<string> GetActions()
        {
            throw new NotImplementedException();
        }

        public IObservable<ZChannelInfo> GetInfo()
        {
            throw new NotImplementedException();
        }

        public IObservable<TResult> Send<T, TResult>(string action, T data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage<T, TResult>(url, data);
        }

        public IObservable<ZNull> Post<T>(string action, T data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.PostPackage(url, data);
        }

        private string GetBaseUrl()
        {
            return channel.Topic + "/";//.Url + "/Channel/" + channelName + "/";
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
