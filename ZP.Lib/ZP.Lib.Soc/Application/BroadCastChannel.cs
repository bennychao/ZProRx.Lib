using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Soc;
using ZP.Lib.CoreEx;
using UniRx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix;
using UnityEngine;

namespace ZP.Lib.Soc
{
   public  class BroadCastChannel : BaseServerCastChannel
    {

        public BroadCastChannel(IRoomServer roomServer, string clientName)
            : base(roomServer, clientName)
        {
        }

        override public void BindRoom(ZRoom room, IScheduler scheduler = null)
        {
            //base.BindRoom(room, scheduler);
            zRoom = room;
            this.scheduler = scheduler;

            //check attribute
            channelListener = ChannelListener.GetInstance(zRoom.RoomId.ToString() + "/BroadCastServer" + "/" + clientChannelName);
            channelListener.BaseUrl = GetBaseUrlPrefix() + clientChannelName + "/";
            //var ch = room

            //find channel
            zChannel = zRoom.FindChannel(clientChannelName);

            OnConnectedObservable.Subscribe(clientId=>
            {
                //Debug.Log("BroadCastCannel broadCast OnConnectedObservable to clientId =" + clientId);
                roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientconnected", clientId).Subscribe();
            });

            OnDisConnectedObservable.Subscribe(clientId=>
             {
                 roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientdisconnected", clientId).Subscribe();
             });
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            //register broad cast message
            //CheckClient(a => isCurrentClient(a)).
            var observer = channelListener.BroadCastObservable;
            var lis = observer.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
            .Subscribe(pack =>
            {
                var topic = pack.Topic;

                //reget action delete broad cast,
                //reuse the client pipeline name
                var newTopic = removeSub(topic, "_broadcast_");

                //send to all Clients the raw msg
                roomServer.BroadCastRawData(newTopic, pack.Value);
                
            });

            defaultDisposables.Add(lis);
        }

        protected override void OnCloseed()
        {
            base.OnCloseed();
        }
    }
}
