using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;
using ZP.Lib.SocClient;
using ZP.Lib.CoreEx;
using ZP.Lib.Net;
using ZP.Lib.Main.CommonTools;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Test
{
    public class UnitPipeline
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
                    //! [For Test Chapter] 
                    .AddChannelType<TestChannel>()
                    //.AddRuntimeServer<TestRuntimeChannel>()
                    //.AddRoundServer<TestRoundChannel>() //support add multi

                    ////! [For Dudu Chapter]
                    //.AddRoundServer<DuduBattleChannel>()

                    ;//End UseMatrix

                app.UseScene();


                var log = app.GetService<ILogger<StartUp>>();
                log.LogWarning("set up");

                //Init Local
                var clientBuilder = app.UseRoomClient()
                    //! init local client [For Test Chapter] 
                    //.AddChannelType<TestClientChannel>()

                    //.AddPipeline<TestRuntimePipeline>()
                    .AddPipeline<TestBroadPipeline>()       //use default broad server channel
                    .AddPipeline<MultiCastPipeline>()       //use default multicast pipeline and server channel
                    .AddPipeline<UniCastPipeline>()

                    ////for Test
                    //.AddPipeline<TestRoundPipeline>()//;
                    //.AddPipeline<TestSceneMgrPipeline>()  //for socClient not need App Scene Pipeline

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
        public void TestBroadCast()
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

                            var cast = client.GetChannel<TestBroadPipeline>();

                            //wait for all clients to connect, broadServerChannel only server can do it
                            //var broadServerChannel = roomServer.GetChannel<BroadCastChannel>();
                            cast.WaitConnected(2).ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            Assert.IsTrue(cast.ClientCount == 2);

                           var castInfo = cast.GetInfo().Fetch().ToTask().Result;

                            //broadcast
                            var retObj = ZPropertyMesh.CreateObject<TestRet>();
                            retObj.zProperty.Value = 100;


                            cast.BroadCast<TestRet>("testfunc_with_pack_notify", retObj).Subscribe(ret =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            //[TODO]Test for Msg

                            cast.RawMsgObservable<TestRet>().Subscribe(data =>
                            {
                                taskEnd.Value = true;
                            });

                            //only send msg one time
                            if (string.Compare(c, "1001") == 0)
                                cast.BroadCastMsg<TestRet>(retObj).Subscribe();

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            cast.BroadCast<TestRet>("testfunc_with_void_notify", retObj).Subscribe();

                            //get with result not support
                            //cast.Send<TestRet, TestResponseRet>("testfunc_with_package", retObj).Subscribe(ret =>
                            //{
                            //    taskEnd.Value = true;
                            //});
                            Thread.Sleep(500);

                            //Msg will receive 4 times
                            taskEnd.Value = false;

                            //wait for all broadcast called
                            //per pipeline call 4 times
                            cast.CallCount.WaitFor(c => c >= 4, 10).ToTask().Wait();

                            cast.OnDisConnectedObservable.Subscribe(cl =>
                            {
                                Debug.Log("OnDisConnectedObservable");
                            });

                            cast.CheckAndWaitStatus(ChannelStatusEnum.Closed).Subscribe(cl =>
                            {
                                Debug.Log("OnDisConnectedObservable");
                            });

                            cast.Disconnect();

                            Thread.Sleep(400);

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
            roomServer.OnStopObservable.WaitAFetch(5);

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }


        [Test]
        public void TestMultiCast()
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
                            var runId = MatrixRuntimeTools.GetRunId();
                            Assert.IsTrue(string.Compare(runId, c) == 0);

                            var roomServer = StartUp.Builder.FindRoom(3);

                            Assert.IsTrue(roomServer.ClientCount >= 1);

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

                            var cast = client.GetChannel<MultiCastPipeline>();

                            //wait for the cast to listen
                            Thread.Sleep(1000);

                            cast.CheckAndWaitStatus(Domain.ChannelStatusEnum.Listen).WaitAFetch();

                            //cast.StatusChanged.Subscribe(s =>
                            //{
                            //    taskEnd.Value = true;
                            //});

                            //taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            //taskEnd.Value = false;

                            //connect
                            cast.Connect2().Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            //join group, group1 is not define in appsetting
                            cast.JoinGroup("group1").Catch((Exception ex) =>
                            {
                                Assert.IsTrue( ex.IsError(ZNetErrorEnum.UnDefineGroup));
                                return Observable.Return(ZNull.Default);
                            })
                            .Fetch().ToTask().Wait();

                            runId = MatrixRuntimeTools.GetRunId();
                            Assert.IsTrue(string.Compare(runId, c) == 0);

                            cast.JoinGroup("Test").Fetch().ToTask().Wait();

                            //wait for all clients to connect, broadServerChannel only server can do it
                            //var broadServerChannel = roomServer.GetChannel<BroadCastChannel>();
                            cast.WaitConnected(2).ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).WaitAFetch();
                            taskEnd.Value = false;

                            Assert.IsTrue(cast.ClientCount == 2);

                            //Thread.Sleep(1000);

                            //broadcast
                            var retObj = ZPropertyMesh.CreateObject<TestRet>();
                            retObj.zProperty.Value = 100;

                            //use for Custom MultiCast Class
                            //cast.MultiCast<TestRet>("group1", "testfunc_with_pack_notify", retObj).Subscribe(ret =>
                            //{
                            //    taskEnd.Value = true;
                            //});

                            //[TODO]Test for Msg

                            var disp = cast.RawMsgObservable<TestRet>().Subscribe(data =>
                            {
                                taskEnd.Value = true;
                            });

                            //only send msg one time
                            if (string.Compare(c, "1001") == 0)
                                cast.MultiCastMsg<TestRet>("Test", retObj).Subscribe();

                            taskEnd.Where(cur => cur == true).WaitAFetch();
                            taskEnd.Value = false;
                            disp.Dispose();

                            //Leave group
                            cast.LeaveGroup("Test").WaitAFetch();

                            //cast.RawMsgObservable<TestRet>().Subscribe(data =>
                            //{
                            //    taskEnd.Value = true;
                            //});

                            ////only send msg one time
                            //if (string.Compare(c, "1001") == 0)
                            //    cast.MultiCastMsg<TestRet>("Test", retObj).Subscribe();

                            //taskEnd.Where(cur => cur == true).WaitAFetch();
                            //taskEnd.Value = false;
                            //disp.Dispose();

                            Thread.Sleep(1000);

                            roomServer.OnLeaveObservable.Subscribe(client =>
                            {
                                //Assert.IsTrue(string.Compare(client, c) == 0);
                                if (string.Compare(client, c) == 0)
                                    taskEnd.Value = true;
                            });
                            //destroy the Client
                            client.Disconnect();

                            taskEnd.Where(cur => cur == true).WaitAFetch();
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

            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }

        [Test]
        public void TestUniCast()
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

                            var cast = client.GetChannel<UniCastPipeline>();

                            //wait for the cast to opened
                            Thread.Sleep(1000);

                            cast.CheckAndWaitStatus(Domain.ChannelStatusEnum.Listen).WaitAFetch();

                            //connect
                            cast.Connect2().Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;


                            var disp = cast.RawMsgObservable<TestRet>().Subscribe(data =>
                            {
                                if (string.Compare(c, "1002") == 0)
                                    taskEnd.Value = true;
                            });


                            //wait for all clients to connect, broadServerChannel only server can do it
                            //var broadServerChannel = roomServer.GetChannel<BroadCastChannel>();
                            cast.WaitConnected(2).ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).WaitAFetch();
                            taskEnd.Value = false;

                            Assert.IsTrue(cast.ClientCount == 2);

                            //Thread.Sleep(1000);

                            //broadcast
                            var retObj = ZPropertyMesh.CreateObject<TestRet>();
                            retObj.zProperty.Value = 100;

                            //use for Custom MultiCast Class
                            //cast.MultiCast<TestRet>("group1", "testfunc_with_pack_notify", retObj).Subscribe(ret =>
                            //{
                            //    taskEnd.Value = true;
                            //});

                            //[TODO]Test for Msg

                            //only send msg one time
                            if (string.Compare(c, "1001") == 0)
                                cast.UniCastMsg<TestRet>("1002", retObj).Subscribe();

                            if (string.Compare(c, "1002") == 0)
                            {
                                taskEnd.Where(cur => cur == true).WaitAFetch();
                                taskEnd.Value = false;
                                disp.Dispose();
                            }

                            Thread.Sleep(1000);

                            roomServer.OnLeaveObservable.Subscribe(client =>
                            {
                                //Assert.IsTrue(string.Compare(client, c) == 0);
                                if (string.Compare(client, c) == 0)
                                    taskEnd.Value = true;
                            });
                            //destroy the Client
                            client.Disconnect();

                            taskEnd.Where(cur => cur == true).WaitAFetch();
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


            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }

        [Test]
        public void TestCustomPipeline()
        {

        }
    }
}
