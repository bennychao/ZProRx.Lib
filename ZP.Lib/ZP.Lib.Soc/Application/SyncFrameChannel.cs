using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UniRx;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Soc.Entity;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Soc
{

    [ChannelBoot(ChannelBootFlagEnum.ManualCreate | ChannelBootFlagEnum.Normal)]
    public class SyncFrameChannel<TAction> :
        ServerChannel,
        IActionAgentContainer<TAction>,
        ISyncFrameChannel
    {
        SyncFrameConfig syncFrameConfig = null;

        readonly IConfigurationRoot configuration;

        protected int curFrame = 0;

        ZListUpdater curListUpdater;

        Dictionary<string, IActionAgent<TAction>> agents = new Dictionary<string, IActionAgent<TAction>>();

        Subject<SyncFrameUpdatePackage> frameSubject = new Subject<SyncFrameUpdatePackage>();

        Subject<SyncFrameActionPackage<TAction>> cmdSubject = new Subject<SyncFrameActionPackage<TAction>>();

        public IObservable<SyncFrameUpdatePackage> FrameObservable => frameSubject;

        SyncFrameActionPackages<TAction> curFrameActions = new SyncFrameActionPackages<TAction>();

        public IObservable<SyncFrameActionPackage<TAction>> ActionObservable => cmdSubject;

        //Dictionary<string, Subject<SyncFrameActionPackage<TAction>>>


        public SyncFrameChannel(IRoomServer roomServer, string clientName, IConfigurationRoot configuration)
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
            channelListener = ChannelListener.GetInstance(zRoom.RoomId.ToString() + "/RoundCastServer" + "/" + clientChannelName);
            channelListener.BaseUrl = GetBaseUrlPrefix() + clientChannelName + "/";
            //var ch = room

            //read from config
            var section = configuration.GetSection(clientChannelName + "RoundConfig");
            section.Bind(syncFrameConfig);

            //find channel
            zChannel = zRoom.FindChannel(clientChannelName);

            //broadcast to clients
            OnConnectedObservable.Subscribe(clientId
                => roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientconnected", clientId).Subscribe());

            OnDisConnectedObservable.Subscribe(clientId
                 => roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientdisconnected", clientId).Subscribe());

        }

        public void BindUpdateObject(ZListUpdater u)
        {
            curListUpdater = u;
        }

        [SyncResult]
        [Action("syncframestart")]
        public ZNull StartSync()
        {
            Run();
            return ZNull.Default;
        }

        [SyncResult]
        [Action("syncframestop")]
        public ZNull StopSync()
        {

            return ZNull.Default;
        }

        public IObservable<ZNull> Start()
        {
            return null;
            //return Observable.;
        }

        public IObservable<ZNull> Stop()
        {
            return null;
        }


        protected override void OnOpened()
        {
            base.OnOpened();

            //register sync cast message
            var observerCast = channelListener.SyncFrameObservable;
            IDisposable lisCast = null;
            lisCast = observerCast.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
              .Subscribe(pack =>
              {
                  var topic = pack.Topic;

                  //reget action delete broad cast
                  var newTopic = removeSub(topic, "syncframe");

                  //send to all Clients the raw msg
                  roomServer.BroadCastRawData(newTopic, pack.Value);

              });

            defaultDisposables.Add(lisCast);


            observerCast = channelListener.SyncFrameEnumObservable;
            lisCast = null;
            lisCast = observerCast.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
              .Subscribe(pack =>
              {
                  var topic = pack.Topic;

                  //reget action delete broad cast
                  var newTopic = removeSub(topic, "syncframeenum");

                  //roomServer.Send(newTopic, pack.Value);

                  var frame = SyncFrameActionPackage<TAction>.CreateFrameStr(pack.Value);

                  curFrameActions.Actions.Add(frame);

              });

            defaultDisposables.Add(lisCast);

            //var updateObservable = channelListener.SyncFrameUpdateObservable;
            //lisCast = null;
            //lisCast = updateObservable.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
            //  .Subscribe(pack =>
            //  {

            //      //wait to frame
            //      //send to all Clients the raw msg
            //      roomServer.Send(newTopic, pack.Value);

            //  });

            //defaultDisposables.Add(lisCast);
        }

        void Run()
        {
            Task.Run(async () =>
            {
                while (curFrame < syncFrameConfig.MaxFrame)
                {
                    if (await Tick())
                        break;

                    curFrame++;
                }
            });
        }//end run

        async Task<bool> Tick()
        {
            var timeoutObservable = new TimeoutObservable(syncFrameConfig.FrameInterval);

            await timeoutObservable;

            //await Task.WhenAny(handSubject.ToTask(), timeoutObservable.ToTask());

            //if (handSubject.IsHand)
            //    await new TimeoutObservable(roundConfig.HandInterval);

            //await new TimeoutObservable(roundConfig.RoundInterval);


            curFrameActions.CurFrame.Value = curFrame;

            await roomServer.SendPackage(GetBaseUrl() + "syncframeenum", curFrameActions).Fetch();

            DispatchActions();

            //get current clientId
            var pack = SyncFrameUpdatePackage.Create(curListUpdater);
            pack.CurFrame.Value = curFrame;

            await roomServer.SendPackage(GetBaseUrl() + "onframe", pack).Fetch();

            curFrameActions.Clear();

            frameSubject.OnNext(pack);

            return false;
        }

        public void RegisterAgent(string clientId, IActionAgent<TAction> agent)
        {
            agents[clientId] = agent;
        }

        public void UnRegisterAgent(string clientId)
        {
            agents[clientId] = null;
        }

        void DispatchActions()
        {
            IActionAgent<TAction> agent;
            foreach (var a in curFrameActions.Actions)
            {
                if (agents.TryGetValue(a.ClientId, out agent))
                    agent.OnAction(a.Action, a.RawData);
            }
        }
    }
}
