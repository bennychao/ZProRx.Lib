using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix;
using ZP.Lib.Soc;


namespace ZP.Lib.Soc
{
    public class SceneManageChannel : BaseServerCastChannel, ISceneMgrPipeline
    {
        readonly private IConfigurationRoot configuration;

        protected HandObservable<string> OnLoaded = new HandObservable<string>();

        public IObservable<string> OnLoadedObservable => OnLoaded;


        public SceneManageChannel(IRoomServer roomServer, string clientName, IConfigurationRoot configuration)
        :  base(roomServer, clientName)
        {
            this.configuration = configuration;
        }


        override public void BindRoom(ZRoom room, IScheduler scheduler = null)
        {
            //base.BindRoom(room, scheduler);
            zRoom = room;
            this.scheduler = scheduler;

            //check attribute
            channelListener = ChannelListener.GetInstance(zRoom.RoomId.ToString() + "/SceneManageServer" + "/" + clientChannelName);
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
                 roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientdisconnected", clientId).Subscribe());
        }

        public IObservable<ZNull> LoadScene(string sceneName)
        {
            Assert.IsTrue(Status == ChannelStatusEnum.Opened, "Status is Not Opened");

            ZServerScene.Instance.LoadAsync(sceneName)
                .ContinueWith(__ => OnLoaded.OnNext(sceneName));

            return Observable.Return(ZNull.Default);
        }

        public IObservable<ZNull> LoadSceneSync(string sceneName)
        {
            Assert.IsTrue(Status == ChannelStatusEnum.Opened, "Status is Not Opened");

            //will wait for all return
            var ret = roomServer.SendPackage<string>(GetClientBaseUrl() + "loadscene", sceneName).ObserveOn(innerScheder);

            //load server's scene, when have receive the loadsceneend msg
            ret.SubscribeAsync(_ =>
                    ZServerScene.Instance.LoadAsync(sceneName).ContinueWith(__ => OnLoaded.OnNext(sceneName))
                ); 

            return ret;
        }

        [SyncResult]
        [Action("loadsceneend")]
        protected ZNull onLoadScene()
        {
            Debug.Log("On Load Scene End");

            //wait for server scene load end
            var timeoutTask = new TimeoutObservable(10000).ToTask();
            Task.WhenAny(OnLoaded.Check(), timeoutTask).Wait();

            return ZNull.Default;
        }

        [SyncResult]
        [Action("loadscene")]
        protected ZNull LoadSceneAction([FromPackage] string sceneName)
        {
            LoadScene(sceneName).Subscribe();
            return ZNull.Default;
        }

        [SyncResult]
        [Action("loadscenesync")]
        protected ZNull LoadSceneSyncAction([FromPackage] string sceneName)
        {
            LoadSceneSync(sceneName);//.Subscribe();
            return ZNull.Default;
        }

    }
}
