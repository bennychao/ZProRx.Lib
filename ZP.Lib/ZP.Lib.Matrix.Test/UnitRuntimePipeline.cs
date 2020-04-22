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
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.Main.CommonTools;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;
using ZP.Lib.SocClient;
using ZP.Lib.Battle;
using ZP.Lib.Net;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc.Entity;

namespace ZP.Lib.Matrix.Test
{
    public class UnitRuntimePipeline
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
            }

            public void Configure(ISocAppBuilder app)
            {
                app.UseZLog("sysnlog.config");

                Builder = app.UseMatrix()
                    //.AddChannelType<TestChannel>()
                    .AddRuntimeServer<TestRuntimeChannel>()
                    //.AddRoundServer<TestRoundChannel>() //support add multi
                    ;//End UseMatrix

                app.UseScene();


                var log = app.GetService<ILogger<StartUp>>();
                log.LogWarning("set up");

                //Init Local
                var clientBuilder = app.UseRoomClient()
                    //.AddChannelType<TestClientChannel>()
                    .AddPipeline<TestRuntimePipeline>()
                    ;// Init RoomClient End

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
                "{\"WorkerParam\":\"run\",\"Port\":5050,\"UnitType\":\"hall\",\"Count\":3}"
            };

            if (!bSetup)
            {
                Task.Run(() =>
                {

                    var app = SocApp.CreateSocApp(args)
                    //.UseNacos<Program>()    //to manage the config
                    .UseStartup<StartUp>();
                    bSetup = true;
                    app.Run();
                });

                Thread.Sleep(2000);
            }

        }


        [Test]
        public void TestSystemRuntime()
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

                            var runtime = client.GetChannel<TestRuntimePipeline>();

                            //runtime pipeline support auto connect
                            runtime.CheckAndWaitStatus(Domain.ChannelStatusEnum.Opened).WaitAFetch();

                            //call sync
                            runtime.Sync().WaitAFetch();

                            //call sync check timeout    
                           if (string.Compare("1001", c) == 0)
                            {
                                runtime.SyncWithTimeOut().Catch((Exception ex) =>
                                {
                                    Assert.IsTrue(ex.IsError(ZNetErrorEnum.Timeout));

                                    return Observable.Return(ZNull.Default);
                                }).WaitAFetch();
                            }

                            //Two User and One Auto Connected AI Client
                            //.ObserveOn(ZPRxScheduler.CurrentScheduler)
                            runtime.WaitConnected(3).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });
                            
                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            Assert.IsTrue(runtime.ClientCount == 3);

                            Thread.Sleep(1000);

                            runtime.CheckAndWaitStatus(Domain.ChannelStatusEnum.Closed).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            runtime.OnDisConnectedObservable.Subscribe(clinetId =>
                            {
                                Debug.Log("");
                            });

                            runtime.Disconnect();

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
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


            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.OnStopObservable.WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            //pipeServers
            var pipeServers = roomServer.GetChannels<ServerChannel>();

            foreach(var c in pipeServers)
            {
                Assert.IsTrue(c.ClientCount == 0);
            }

            Debug.Log("End of the Test");

            Thread.Sleep(1000);
            //ZPropertySocket.CheckRecvListenerCount(5, 4);
        }

        [Test]
        public void TestWExitSystem()
        {
            Debug.Log("start TestWExitSystem");
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

                            //Assert.IsTrue(roomServer.ClientCount >= 1);

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

                            var runtime = client.GetChannel<TestRuntimePipeline>();

                            //runtime pipeline support auto connect
                            runtime.CheckAndWaitStatus(Domain.ChannelStatusEnum.Opened).WaitAFetch();

                            //call sync
                            runtime.Sync().WaitAFetch();

                            //Two User and One Auto Connected AI Client
                            //.ObserveOn(ZPRxScheduler.CurrentScheduler)
                            runtime.WaitConnected(3).Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            Assert.IsTrue(runtime.ClientCount == 3);

                            Thread.Sleep(1000);

                            //roomServer.OnLeaveObservable.Subscribe(client =>
                            //{
                            //    //Assert.IsTrue(string.Compare(client, c) == 0);

                            //    if (string.Compare(client, c) == 0)
                            //        taskEnd.Value = true;
                            //});

                            roomServer.WaitClientCounted(0).Subscribe(client =>
                            {
                                    taskEnd.Value = true;
                            });

                            runtime.ExitSystem().Subscribe();

                            //destroy the Client
                            // client.Disconnect();

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

            roomServer.OnStopObservable.WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            //ZPropertySocket.CheckRecvListenerCount(5, 4);
        }
    }
}
