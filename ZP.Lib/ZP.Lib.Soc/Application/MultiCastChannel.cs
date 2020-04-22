using System;
using ZP.Lib.CoreEx;
using UniRx;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix;
using ZP.Lib.Net;

namespace ZP.Lib.Soc
{
    public class MultiCastChannel : BaseServerCastChannel
    {

        public class GroupConfig
        {
            public List<string> Groups { get; set; }
        }

        readonly IConfigurationRoot configuration;

        protected List<MultiChannelGroup> channelGroups = new List<MultiChannelGroup>();

        public MultiCastChannel(IRoomServer roomServer, string clientName, IConfigurationRoot configuration)
             : base(roomServer, clientName)
        {

            this.configuration = configuration;
        }

        override public void BindRoom(ZRoom room, IScheduler scheduler = null)
        {
            //base.BindRoom(room, scheduler);
            zRoom = room;
            this.scheduler = scheduler;

            //check attribute
            channelListener = ChannelListener.GetInstance(zRoom.RoomId.ToString() + "/MultiCastServer" + "/" + clientChannelName);
            channelListener.BaseUrl = GetBaseUrlPrefix() + clientChannelName + "/";
            //var ch = room

            //read from config
            var groudC = new GroupConfig();
            var section = configuration.GetSection(clientChannelName + "GroupConfig");
            section.Bind(groudC);

            foreach (var g in groudC.Groups)
            {
                channelGroups.Add(new MultiChannelGroup(g));
            }

            //find channel
            zChannel = zRoom.FindChannel(clientChannelName);

            OnConnectedObservable.Subscribe(clientId =>
            {
                //Debug.Log("BroadCastCannel broadCast OnConnectedObservable to clientId =" + clientId);
                roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientconnected", clientId).Subscribe();
            });

            OnDisConnectedObservable.Subscribe(clientId =>
            {
                roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientdisconnected", clientId).Subscribe();
            });
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            //register broad cast message
            var observer = channelListener.MultiCastObservable;
            var lis = observer.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
                .Subscribe(pack =>
                {
                    var topic = pack.Topic;

                    var group = ExtractGroupId(topic);

                    //reget action delete broad cast
                    //reuse the client pipeline name
                    var newTopic = removeSub(topic, "_multicast_/" + group);

                    var g = channelGroups.Find(a => string.Compare(a.Group, group, true) == 0);
                    if (g == null)
                    {
                        //return ZPropertyNet.ErrorResult(Net.ZNetErrorEnum.UnDefineGroup);
                        throw new ZNetException(ZNetErrorEnum.UnDefineGroup);
                    }

                    //send to all Clients
                    foreach (var c in g.Clients)
                        roomServer.BroadCastRawDataTo(c, newTopic, pack.Value);

                });

            defaultDisposables.Add(lis);

            var joinObserver = channelListener.JoinGroupObservable;
            //listen join channel
            lis = joinObserver.ResponseOn(innerScheder)
                .Subscribe(pack =>
                {
                    var g = channelGroups.Find(a => string.Compare(a.Group, pack.data.Data, true) == 0);
                    if (g == null)
                    {
                        //return ZPropertyNet.ErrorResult(Net.ZNetErrorEnum.UnDefineGroup);
                        throw new ZNetException(ZNetErrorEnum.UnDefineGroup);
                    }

                    //var re = joinObserver as IGetSocketResponse;
                    //if (re != null)
                    //{
                    //    var id = re.SocketResponse.Key;
                    //    g.Clients.Add(id);
                    //}

                    g.Clients.Add(pack.socketPackage.Key);

                    //return ZPropertyNet.OkResult();
                    return ZNull.Default;
                });

            defaultDisposables.Add(lis);

            var unjoinObserver = channelListener.UnJoinGroupObservable;
            //listen unjoin channel
            lis = unjoinObserver.ResponseOn(innerScheder)
                .Subscribe(pack =>
                {
                    //
                    var g = channelGroups.Find(a => string.Compare(a.Group, pack.data.Data, true) == 0);
                    if (g == null)
                    {
                        //return ZPropertyNet.ErrorResult(Net.ZNetErrorEnum.UnDefineGroup);
                        throw new ZNetException(ZNetErrorEnum.UnDefineGroup);
                    }
                    g.Clients.Remove(pack.socketPackage.Key);
                    //var re = joinObserver as IGetSocketResponse;
                    //if (re != null)
                    //{
                    //    var id = re.SocketResponse.Key;
                    //    g.Clients.Remove(id);
                    //}

                    //return ZPropertyNet.OkResult();
                    return ZNull.Default;
                });

            defaultDisposables.Add(lis);

        }

        private string ExtractGroupId(string topic)
        {
            var startIndex = topic.IndexOf("_multicast_/") + "_multicast_/".Length;
            var endIndex = topic.IndexOf("/", startIndex);

            return topic.Substring(startIndex, endIndex - startIndex);
        }
        protected override void OnCloseed()
        {
            base.OnCloseed();
        }


    }
}
