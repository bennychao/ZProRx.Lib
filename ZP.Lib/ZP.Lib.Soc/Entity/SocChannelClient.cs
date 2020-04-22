using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;
using ZP.Lib.Core.Values;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Soc.Entity
{
    //use for Soc server to connec to client channel
    internal class SocChannelClient : IChannelClient
    {
        private ZChannel channel;
        private IRoomServer roomServer;
        private string clientId;

        public ChannelStatusEnum Status => this.channel?.Status;

        public IObservable<ChannelStatusEnum> StatusChanged => channel?.Status.ValueChangeAsObservable<ChannelStatusEnum>();

        protected IScheduler innerScheder => roomServer.RoomRxScheduler;

        public SocChannelClient(ZChannel channel, IRoomServer roomServer, string clientId)
        {
            this.roomServer = roomServer;
            this.channel = channel;
            this.clientId = clientId;
        }

        public IObservable<ZNull> Connect()
        {
            var ret = SendWithResult("connect")
                .Do(_ => this.channel.Status.Value = ChannelStatusEnum.Opened);   //sync the remote channel's status;
            //ret.Subscribe();

            return ret;
        }

        public IObservable<ZNull> Connect2()
        {
            var ret = Send<ZNull, ZPropertyListHub<string>>("connect2" + "/" + clientId, ZNull.Default);
            //ret.Subscribe();

            return ret
                .Do(_ => this.channel.Status.Value = ChannelStatusEnum.Opened)   //sync the remote channel's status
                .Select(_ => ZNull.Default);
        }


        public void Disconnect()
        {
            //innerSend("disconnect", "server");
            //sync the remote channel's status

            SendWithResult("disconnect")
                 .Do(_ => this.channel.Status.Value = ChannelStatusEnum.Closed)
                 .Subscribe();
        }

        public List<string> GetActions()
        {
            throw new NotImplementedException();
        }

        public IObservable<ZChannelInfo> GetInfo()
        {
            return Send<ZNull, ZChannelInfo>("getinfo" + "/" + clientId, ZNull.Default);
        }


        public IObservable<TResult> Send<T, TResult>(string action, T data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage2<T, TResult>(url + "/" + clientId, data).ObserveOn(innerScheder);
        }

        public IObservable<ZNull> Post<T>(string action, T data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.PostPackage(url + "/" + clientId, data).ObserveOn(innerScheder);
        }


        protected IObservable<ZNull> SendWithResult(string action)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage<ZNull, ZNull>(url + "/" + clientId, ZNull.Default).ObserveOn(innerScheder);
        }

        public void Dispose()
        {
            Disconnect();
        }


        private string GetBaseUrl()
        {
            return channel.Topic + "/";//.Url + "/Channel/" + channelName + "/";
        }

        private void innerSend(string action, string data)
        {
            var url = GetBaseUrl() + action;

            ZPropertySocket.Post(url + "/" + clientId, data);
        }
    }
}
