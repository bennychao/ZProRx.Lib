using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;
using ZP.Lib.Core.Values;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Entity
{
    public class ChannelClient : IChannelClient
    {
        private ZChannel channel;
        //private string channelName;

        private IScheduler scheduler;

        //string clientId;
        public ChannelStatusEnum Status => this.channel?.Status;

        public IObservable<ChannelStatusEnum> StatusChanged => channel.Status.ValueChangeAsObservable<ChannelStatusEnum>();

        protected IScheduler innerScheder => scheduler ?? Scheduler.CurrentThread;

        public ChannelClient(ZChannel channel, IScheduler scheduler = null)
        {
            this.channel = channel;
            this.scheduler = scheduler ?? Scheduler.CurrentThread;
        }

        public IObservable<ZNull> Connect()
        {
            var ret = SendWithResult("connect")
                .Do(_ => this.channel.Status.Value = ChannelStatusEnum.Opened);   //sync the remote channel's status;

            //ret.Subscribe();

            return ret;
        }

        //for normal Channel not to Support OnConnected Event
        public IObservable<ZNull> Connect2()
        {
            var ret = Send<ZNull, ZPropertyListHub<string>>("connect2", ZNull.Default);
            //ret.Subscribe();

            return ret
                .Do(_=> this.channel.Status.Value = ChannelStatusEnum.Opened)   //sync the remote channel's status
                .Select(_ => ZNull.Default);
        }

        public void Disconnect()
        {
            ///Send("disconnect", "client");
            //sync the remote channel's status            

            SendWithResult("disconnect")
                .Do(_=> this.channel.Status.Value = ChannelStatusEnum.Closed)
                .Subscribe();
        }

        public List<string> GetActions()
        {
            throw new NotImplementedException();
        }

        public IObservable<ZChannelInfo> GetInfo()
        {
            return Send<ZNull, ZChannelInfo>("getinfo", ZNull.Default).Select(info => {
                info.ChannelRef.Value = channel;
                return info;
            });
        }

        private string GetBaseUrl()
        {
            return channel.Topic + "/";//.Url + "/Channel/" + channelName + "/";
        }

        //implement the IChannelClient interface
        public IObservable<TResult> Send<T, TResult>(string action, T data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage2<T, TResult>(url, data).ObserveOn(innerScheder);
        }

        //implement the IChannelClient interface
        public IObservable<Unit> Post<T>(string action, T data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.PostPackage(url, data).ObserveOn(innerScheder);
        }

        //raw Post
        //public IObservable<Unit> Post(string action, string data)
        //{
        //    var url = GetBaseUrl() + action;

        //    return ZPropertySocket.Post(url, data).ObserveOn(innerScheder);
        //}

        protected IObservable<ZNull> SendWithResult(string action)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage<ZNull, ZNull>(url, ZNull.Default).ObserveOn(innerScheder);
        }


        public void Dispose()
        {
            Disconnect();
        }
    }
}
