using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using UniRx;
using ZP.Lib.Soc.Entity;
//using ZP.Lib.CoreEx;
using System.Threading.Tasks;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Entity;
using UnityEngine;
using ZP.Lib.Net;
using ZP.Lib.CoreEx;
using ZP.Lib.CoreEx.Domain;

namespace ZP.Lib.Soc
{

    [ChannelBoot(ChannelBootFlagEnum.ManualCreate | ChannelBootFlagEnum.Normal)]
    public class RoundChannel<TCmd> :
        ServerChannel,
        IActionAgentContainer<TCmd>,
        IRoundPipeline
    {

        readonly private IConfigurationRoot configuration;
        //readonly IRoomServer roomServer = null;
        //protected string clientChannelName;

        protected Dictionary<string, IActionAgent<TCmd>> agents = new Dictionary<string, IActionAgent<TCmd>>();


        private RoundConfig roundConfig = new RoundConfig();

        public RoundConfig RoundConfig => roundConfig;


        //lifecricle subject define
        private Subject<ZNull> startSubject = new Subject<ZNull>();

        private Subject<ZNull> stopSubject = new Subject<ZNull>();//.ToCancellable;

        private ICancellableObserver<ZNull> cancellableStopObserver = null;// stopSubject.ToCancellable();

        private Subject<ZNull> readySubject = new Subject<ZNull>();

        private Subject<RoundPackage> roundSubject = new Subject<RoundPackage>();

        private Subject<RoundPackage> tickSubject = new Subject<RoundPackage>();

        private HandObservable<Unit> handSubject = new HandObservable<Unit>();

        //private Dictionary<string, object> curResult = new Dictionary<string, object>();



        volatile protected int curRound = 0;

        volatile protected int curRoundIndex = 0;


        public IObservable<RoundPackage> OnRoundObservable => roundSubject.ObserveOn(innerScheder);

        public IObservable<RoundPackage> OnTickObservable => tickSubject.ObserveOn(innerScheder);

        public IObservable<ZNull> OnReadyObservable => readySubject.ObserveOn(innerScheder);

        public IObservable<ZNull> OnStartObservable => startSubject.ObserveOn(innerScheder);

        public IObservable<ZNull> OnStopObservable => stopSubject.ObserveOn(innerScheder);

        //public int ClientCount => clientIds.Count;

        public List<string> Clients => clientIds;

        public RoundChannel(IRoomServer roomServer, string clientName, IConfigurationRoot configuration)
                    : base(roomServer, clientName)
        {
            //this.clientChannelName = clientName;
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
            section.Bind(roundConfig);

            //find channel
            zChannel = zRoom.FindChannel(clientChannelName);


            //broadcast to clients
            OnConnectedObservable.Subscribe(clientId
                =>
            {
                roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientconnected", clientId).Subscribe();
            }) ;

            OnDisConnectedObservable.Subscribe(clientId
             => 
                 roomServer.SendPackageEx<string>(clientId, GetClientBaseUrl() + "onclientdisconnected", clientId).Subscribe());
        }


        public IObservable<ZNull> Ready()
        {
            readySubject.OnNext(ZNull.Default);

            roomServer.SendPackage<ZNull>(GetClientBaseUrl() + "oninnerready", ZNull.Default).Subscribe();

            return Observable.Return<ZNull>(ZNull.Default);
        }

        //call in server by server battle
        public IObservable<ZNull> Start()
        {
            return Observable.Return<ZNull>(ZNull.Default);
            //return Observable.;
        }

        //call in server by server battle
        public IObservable<ZNull> Stop()
        {
            cancellableStopObserver.Dispose();
            return Observable.Return<ZNull>(ZNull.Default);
        }

        public void RegisterAgent(string clientId, IActionAgent<TCmd> agent)
        {
            if (agents.ContainsKey(clientId))
                return;

            agents[clientId] = agent;
        }

        public void UnRegisterAgent(string clientId)
        {
            agents[clientId] = null;
        }

        //call by Client
        [SyncResult]
        [Action("roundstart")]
        public ZNull StartSync()
        {
            Debug.Log("round start");

            startSubject.OnNext(ZNull.Default);
            
            ResetRound();

            //round loop
            Run();

            return ZNull.Default;
        }

        //call by Client
        [SyncResult]
        [Action("roundstop")]
        public ZNull StopSync()
        {
            //stopSubject.OnNext(Unit.Default);
            cancellableStopObserver.Dispose();

            InnerStop();

            return ZNull.Default;
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            //register start message, wait all client's msg
            var observer = channelListener.RoundStartObservable;

            //register hand message
            //topic:round/hand
            IDisposable lishand = null;
            observer = channelListener.RoundHandObservable;
            lishand = observer.ResponseOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
            .Subscribe(_ =>
            {
                //var re = (lishand as IGetSocketResponse);
                handSubject.OnNext(Unit.Default);

                return ZNull.Default;
            });

            defaultDisposables.Add(lishand);


            //register broad cast message
            var observerCast = channelListener.RoundCastObservable;
            IDisposable lisCast = null;
            lisCast = observerCast.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
              .Subscribe(pack =>
                {
                    var topic = pack.Topic;

                    //reget action delete broad cast
                    var newTopic = removeSub(topic, "roundcast");

                    //send to all Clients the raw msg
                    roomServer.BroadCastRawData(newTopic, pack.Value);

                });

            defaultDisposables.Add(lisCast);

            //roundcmd
            var observerCmd = channelListener.RoundCmdObservable;
            IDisposable lisCmd = null;
            lisCmd = observerCmd.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
              .Subscribe(pack =>
              {
                  var topic = pack.Topic;

                  //reget action delete broad cast
                  var newTopic = replaseSub(topic, "roundcmd", "roundcmdclient");

                  //var pType = GetBodyParamType(m);

                  //send to all Clients the raw msg
                  roomServer.BroadCastRawDataEx(pack.Key, newTopic, pack.Value);
                  //roomServer.Send(newTopic, pack.Value);

                  //dispatch the cmd
                  var ret = NetPackage<CmdPackage<TCmd>, ZNetErrorEnum>.Parse(pack.Value);
                  if (ret.error == ZNetErrorEnum.NoError)
                  {
                      DispatchActions(ret.data);
                  }

              });

            defaultDisposables.Add(lisCmd);


            //Obsolete use actions
            //lis = observer.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
            //.Subscribe(_ =>
            //{
            //   return  OnStartRoundAsync(lis);
            //});

            //register stop message, wait all client's msg
            //IDisposable lisclose = null;
            //observer = channelListener.RoundStopObservable;
            //lisclose = observer.ObserveOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
            //.Subscribe(_ =>
            //{
            //    return OnStopRoundAsync(lisclose);
            //});
            //Obsolete end
        }

        /// <summary>
        /// main function
        /// </summary>
        private void Run()
        {
            cancellableStopObserver = stopSubject.ToCancellable();

            var task = new Task(async () =>
            {
                while (curRound < roundConfig.MaxRound)
                {
                    if (cancellableStopObserver.Token.IsCancellationRequested)
                    {
                        Debug.Log("Round Channel is Stop or Cancelled");
                        break;
                    }

                    Debug.Log($"Tick Call curRound = {curRound}");
                    if (await Tick())
                    //if (Tick().Result)
                        break;

                    curRound++;

                    //change to next client
                    NextClient();
                }

                //Send Stop Subject
                cancellableStopObserver.OnNext(ZNull.Default);

                //notify to client pipeline
                DispatchStopAction();

                Debug.Log("End Of Run");

            }, cancellableStopObserver.Token);

            task.Start(TaskScheduler.Current);
        }//end run



        //return be break;
        /// <summary>
        /// Tick System
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Tick()
        {
            var tickId = Task.CurrentId.ToString();
            Debug.Log($"On Tick Start {Task.CurrentId} RoundId is {curRound}");

            //get current clientId
            var pack = ZPropertyMesh.CreateObject<RoundPackage>();
            pack.CurRound.Value = curRound;
            pack.CurClientId.Value = clientIds[curRoundIndex];

            //workaround
            //var tickRetTack = new Subject<ZNull>();
            IReactiveProperty<bool> waitFlag = new ReactiveProperty<bool>(false);

            //workaround for [Debug Journal No.1] [TODO]
            _ = Task.Delay(100).ContinueWith(_ => waitFlag.Value = true);

            roomServer.SendPackage(GetBaseUrl() + "ontick", pack).ObserveOn(innerScheder).Subscribe(_ =>
            {
                //await Task.Delay(100);

                Debug.Log(" On Tick Return From Client");

                //tickRetTack.OnNext(ZNull.Default);
                //waitFlag.Value = true;                
            });

            //while (!waitFlag.Value)
            //{
            //    await Task.Delay(100);
            //    //Debug.LogWarning("OnTick is not end");
            //}

            await waitFlag.Where(a => a).Fetch().ObserveOn(innerScheder);

            if (CheckRound())
                roundSubject.OnNext(pack);

            tickSubject.OnNext(pack);

            //wait the cur client's action
           var timeoutTask = new TimeoutObservable(roundConfig.RoundHandTimeout).ToTask();            

            //await timeoutTask;
            ///handSubject.Subscribe(_ => Debug.Log(" On Hand"));
            //await handSubject.Check();
            await Task.WhenAny(handSubject.Check(), timeoutTask);
            //Debug.Log($"On Tick Wait For a Hand or Timeout {tickId}");

            if (handSubject.IsHand)
            {
                Debug.Log("hand is recv");
                handSubject.Reset();
                //await new TimeoutObservable(roundConfig.HandInterval);
                await Task.Delay(roundConfig.HandInterval);
            }

            //await new TimeoutObservable(roundConfig.RoundInterval);
            await Task.Delay(roundConfig.RoundInterval);

            Debug.Log($"On Tick End {Task.CurrentId} From Start{tickId}");
            return false;
        }


        /// <summary>
        /// server channel will dispatch to all agents
        /// </summary>
        /// <param name="data"></param>
        private void DispatchActions(CmdPackage<TCmd> data)
        {
            IActionAgent<TCmd> agent;
            if (agents.TryGetValue(data.Organizer, out agent) 
            )
                agent.OnAction(data.Cmd, data.RawData);
        }

        private void DispatchStopAction()
        {
            InnerStop();
            roomServer.SendPackage<ZNull>(GetClientBaseUrl() + "round/onstop", ZNull.Default)
                .WaitAFetch(10, new TimeoutException("round Stop Timeout while round reach max"));
        }

        

        private bool CheckRound()
        {
            return true;
        }

        private void ResetRound()
        {
            this.curRound = 0;
            this.curRoundIndex = 0;
        }

        private void NextClient()
        {
            curRoundIndex = ++curRoundIndex % clientIds.Count;

            //check current client's attribute or status
        }

        private void InnerStop()
        {
            stopSubject.OnCompleted();
            startSubject.OnCompleted();

            readySubject.OnCompleted();
            tickSubject.OnCompleted();
            roundSubject.OnCompleted();

            handSubject.OnCompleted();
        }

    }
}
