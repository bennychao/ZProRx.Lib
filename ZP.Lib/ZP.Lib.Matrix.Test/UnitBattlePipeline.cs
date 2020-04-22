using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.Main.CommonTools;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;
using ZP.Lib.SocClient;
using ZP.Lib.Battle;
namespace ZP.Lib.Matrix.Test
{
    public class UnitBattlePipeline
    {
        internal class StartUp
        {
            public static ISocRoomBuilder Builder = null;

            public static ISocClientBuilder ClientBuilder = null;

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMatrix();
                services.AddRoomClient();
                services.AddZLog();

                //services.AddNacosConfigFile("matrix", "ZP.Matrix.NlogConfig", "sysnlog.config");
            }

            public void Configure(ISocAppBuilder app)
            {
                app.UseZLog("sysnlog.config");

                //app.Use3T2Axes(axes =>
                //{
                //    axes.TRS(Vector3.zero, Vector3.up, Vector3.one);
                //});

                Builder = app.UseMatrix()
                    .AddRoundServer<TestRoundChannel>() //support add multi
                    ;//End UseMatrix

                app.UseScene();


                var log = app.GetService<ILogger<StartUp>>();
                log.LogWarning("set up");

                //Init Local
                var clientBuilder = app.UseRoomClient()
                    .AddPipeline<TestRoundPipeline>()
                    ;// Init RoomClient End


                //ZCardsFactory<Dard>.Instance.Build();
                //.CreateClient("localClient001");

                Debug.Log("On Configure");

                ClientBuilder = clientBuilder;
                //TestSoc.CreateClient(clientBuilder);
            }

        }

        static bool bSetup = false;
        [SetUp]
        public void Setup()
        {
            var args = new string[] {
                "{\"WorkerParam\":\"run\",\"Port\":5050,\"UnitType\":\"hall\",\"Count\":2}"
            };

            if (!bSetup)
            {
                Task.Run(() =>
                {
                    var app = SocApp.CreateSocApp(args)
                    //.UseNacos<Program>()    //to manage the config
                    .UseStartup<StartUp>();

                    bSetup = true;
                    app.Run(() => {
                        //must wait for the build end
                        //var roomServer = StartUp.Builder.FindRoom(3);
                        //Assert.IsTrue(roomServer.Count == 1);

                        //Check Building is running!!

                        //check setting config

                        //check client is connecting!!
                    });
                });

                Thread.Sleep(2000);
            }

        }


        [Test]
        public void TestRound()
        {
            var clientBuilder = StartUp.ClientBuilder;

            var callCount = new InterCountReactiveProperty(0);


            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var clients = new string[] { "1001", "1002" };//

                foreach (var c in clients)
                {
                    int handCount = 0;
                    int tickCount = 0;

                    //create test client
                    var client = clientBuilder.CreateClient(c, 3);

                    client.OnRunObservable.Subscribe(_ =>
                    {
                        client.RunInClient(() =>
                        {
                            var roomServer = StartUp.Builder.FindRoom(3);

                            Assert.IsTrue(roomServer.ClientCount >= 1);

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

                            var roundCh = client.GetChannel<TestRoundPipeline>();

                            roundCh.CheckAndWaitStatus(Domain.ChannelStatusEnum.Listen).WaitAFetch();

                            //connect
                            roundCh.Connect2().Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            //Thread.Sleep(1000);

                            //Two User and One Auto Connected AI Client
                            //.ObserveOn(ZPRxScheduler.CurrentScheduler)
                            roundCh.WaitConnected(2).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            Assert.IsTrue(roundCh.ClientCount == 2);

                            roundCh.OnHandObservable.Subscribe(_ => {
                                Interlocked.Increment(ref handCount);
                            });

                            roundCh.OnTickObservable.Subscribe(_ => Interlocked.Increment(ref tickCount));


                            //sync to start round
                            roundCh.Start().WaitAFetch();
                            //round 0 will sync the start

                            //Thread.Sleep(1000);
                            //round 1
                            roundCh.OnRoundObservable.WaitAFetch();

                            Assert.IsTrue(roundCh.CurRound == 1);

                            //round 2
                            roundCh.OnRoundObservable.WaitAFetch();

                            Assert.IsTrue(roundCh.CurRound == 2);

                            roundCh.OnStopObservable.WaitAFetch();

                            //check max round
                            Assert.IsTrue(roundCh.CurRound == 3);
                            Assert.IsTrue(handCount == 2);
                            Assert.IsTrue(tickCount == 4);

                            Thread.Sleep(1000);

                            roomServer.OnLeaveObservable.Subscribe(client =>
                            {
                                //Assert.IsTrue(string.Compare(client, c) == 0);

                                if (string.Compare(client, c) == 0)
                                    taskEnd.Value = true;
                            });

                            //destroy the Client
                            client.Disconnect();

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;


                            callCount.Increment();

                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 2, 100).ToTask().Wait();

            Debug.Log("End of the Test");

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.OnStopObservable.WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);
        }

        [Test]
        public void TestRoundAgent()
        {
            var clientBuilder = StartUp.ClientBuilder;

            var callCount = new InterCountReactiveProperty(0);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var clients = new string[] { "1001", "1002" };//

                foreach (var c in clients)
                {
                    //create test client
                    var client = clientBuilder.CreateClient(c, 3);

                    client.OnRunObservable.Subscribe(_ =>
                    {
                        client.RunInClient(() =>
                        {
                            var roomServer = StartUp.Builder.FindRoom(3);

                            Assert.IsTrue(roomServer.ClientCount >= 1);

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

                            var roundCh = client.GetChannel<TestRoundPipeline>();

                            roundCh.CheckAndWaitStatus(Domain.ChannelStatusEnum.Listen).WaitAFetch();

                            //connect
                            roundCh.Connect2().Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            //Thread.Sleep(1000);

                            //Two User and One Auto Connected AI Client
                            //.ObserveOn(ZPRxScheduler.CurrentScheduler)
                            roundCh.WaitConnected(2).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            Assert.IsTrue(roundCh.ClientCount == 2);

                            //sync to start 
                            //round 0
                            roundCh.Start().WaitAFetch();

                            roundCh.OnHandObservable.Subscribe(ro => {

                                if (ro == 1)
                                {
                                    var retObj = ZPropertyMesh.CreateObject<TestRet>();
                                    retObj.zProperty.Value = 100;
                                    roundCh.PostAction<TestRet>(TestCmd.Command1, retObj).Subscribe();

                                    roundCh.Hand().Subscribe();
                                }
                            });

                            //round 1
                            roundCh.OnRoundObservable.WaitAFetch();

                            Assert.IsTrue(roundCh.CurRound == 1);

                            //round 2
                            roundCh.OnRoundObservable.WaitAFetch();

                            Assert.IsTrue(roundCh.CurRound == 2);

                            roundCh.OnStopObservable.Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            roundCh.Stop().Subscribe();

                            taskEnd.Where(cur => cur == true).WaitAFetch(2);
                            taskEnd.Value = false;

                            //check max round
                            Assert.IsTrue(roundCh.CurRound == 2);

                            Thread.Sleep(1000);
                            taskEnd.Value = false;

                            roomServer.OnLeaveObservable.Subscribe(client =>
                            {
                                //Assert.IsTrue(string.Compare(client, c) == 0);

                                if (string.Compare(client, c) == 0)
                                    taskEnd.Value = true;
                            });

                            //destroy the Client
                            client.Disconnect();

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;


                            callCount.Increment();

                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 2, 100).ToTask().Wait();

            Assert.IsTrue(TestCmdClientAgent.ActionCount == 2);

            Debug.Log("End of the Test");

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.OnStopObservable.WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);
        }


        [Test]
        public void TestRoundStopByClientSync()
        {
            var clientBuilder = StartUp.ClientBuilder;

            var callCount = new InterCountReactiveProperty(0);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var clients = new string[] { "1001", "1002" };//

                foreach (var c in clients)
                {
                    //create test client
                    var client = clientBuilder.CreateClient(c, 3);

                    client.OnRunObservable.Subscribe(_ =>
                    {
                        client.RunInClient(() =>
                        {
                            var roomServer = StartUp.Builder.FindRoom(3);

                            Assert.IsTrue(roomServer.ClientCount >= 1);

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

                            var roundCh = client.GetChannel<TestRoundPipeline>();

                            roundCh.CheckAndWaitStatus(Domain.ChannelStatusEnum.Listen).WaitAFetch();

                            //connect
                            roundCh.Connect2().Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            //Thread.Sleep(1000);

                            //Two User and One Auto Connected AI Client
                            //.ObserveOn(ZPRxScheduler.CurrentScheduler)
                            roundCh.WaitConnected(2).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            Assert.IsTrue(roundCh.ClientCount == 2);

                            //sync to start round
                            roundCh.Start().WaitAFetch();

                            //Thread.Sleep(1000);
                            //round 1
                            roundCh.OnRoundObservable.WaitAFetch();

                            Assert.IsTrue(roundCh.CurRound == 1);

                            //round 2
                            roundCh.OnRoundObservable.WaitAFetch();

                            Assert.IsTrue(roundCh.CurRound == 2);
                            
                            roundCh.OnStopObservable.Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });
                            
                            roundCh.Stop().Subscribe();

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            //check max round
                            Assert.IsTrue(roundCh.CurRound == 2);

                            Thread.Sleep(1000);
                            taskEnd.Value = false;

                            roomServer.OnLeaveObservable.Subscribe(client =>
                            {
                                //Assert.IsTrue(string.Compare(client, c) == 0);

                                if (string.Compare(client, c) == 0)
                                    taskEnd.Value = true;
                            });

                            //destroy the Client
                            client.Disconnect();

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;


                            callCount.Increment();

                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 2, 100).ToTask().Wait();

            Debug.Log("End of the Test");

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.OnStopObservable.WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);
        }
    }
}
