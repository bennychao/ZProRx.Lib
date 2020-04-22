using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using UniRx;
using UnityEngine;
using ZP.Lib.Card;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;
using ZP.Lib.SocClient;
using ZP.Lib.CoreEx;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Matrix.Domain;
using System.Threading;
using System.Threading.Tasks;
using ZP.Lib.Main.CommonTools;
using ZP.Lib.Net;
namespace ZP.Lib.Matrix.Test
{
    public class UnitPrivateClub
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


                Builder = app.UseMatrix() 
                    .AddChannelType<TestChannel>()
                    ;//End UseMatrix

                app.UseScene();


                var log = app.GetService<ILogger<StartUp>>();
                log.LogWarning("set up");

                //Init Local
                var clientBuilder = app.UseRoomClient()
                    //! init local client [For Test Chapter] 
                    .AddChannelType<TestClientChannel>()
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
                "{\"WorkerParam\":\"run\",\"Port\":5050,\"UnitType\":\"hall\",\"Count\":2, \"IsPrivateClub\":true}"
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
                    });
                });

                Thread.Sleep(2000);
            }

        }

        [Test]
        public void Test1Client()
        {
            CreateClient(StartUp.ClientBuilder);
        }

        static public void CreateClient(ISocClientBuilder clientBuilder)
        {
            IReactiveProperty<bool> testEnd = new ReactiveProperty<bool>(false);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => {

                //create test client
                var client = clientBuilder.CreateClient("1001", 3);

                client.OnRunObservable.Subscribe(_ =>
                {
                    client.RunInClient(() =>
                    {
                        var roomServer = StartUp.Builder.FindRoom(3);
                        Assert.IsTrue(roomServer.ClientCount == 1);

                        TestClientAsync(client).Wait();

                        TestClientWithConnect2(client);

                        TestServerChannelToTestClient(client);

                        IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);
                        var disposeLeave = roomServer.OnLeaveObservable.Subscribe(client =>
                        {
                            Assert.IsTrue(string.Compare(client, "1001") == 0);
                            taskEnd.Value = true;
                        });

                        //destroy the Client
                        client.Disconnect();

                        taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                        taskEnd.Value = false;

                        disposeLeave.Dispose();

                        Assert.IsTrue(roomServer.ClientCount == 0);

                        //[TODO] Check the Architect Server current room's Status
                        testEnd.Value = true;
                    });
                });// end OnRunObservable

            });

            testEnd.Where(cur => cur == true).Fetch().ToTask().Wait();

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.OnStopObservable.WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            ZPropertySocket.CheckRecvListenerCount(4, 4);

        }

        // 1 client to Server
        static async Task TestClientAsync(IRoomClient client)
        {


            //get test channel
            var channelClient = client.GetChannelClient("Test");

            var cInfo = await channelClient.GetInfo().Fetch();

            Assert.IsTrue(cInfo.Actions.Count == 10, "Check action Count");
            Assert.IsTrue(cInfo.ChannelRef.Value != null, "Check ChannelRef Value");

            var retObj = ZPropertyMesh.CreateObject<TestRet>();
            retObj.zProperty.Value = 100;

            Debug.Log("TestClientAsync channelClient.Status = " + channelClient.Status.ToString());

            //will be closed while connect two times
            Assert.IsTrue(channelClient.Status != Domain.ChannelStatusEnum.Opened // || channelClient.Status == Domain.ChannelStatusEnum.Disabled
                , $"Check channelClient.Status  {channelClient.Status}");

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var connectDisp = channelClient.Connect().Subscribe(_ =>
            {
                //it not changed the status
                Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Opened);
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //call testfunc_with_body
            channelClient.Send<TestRet, TestResponseRet>("testfunc_with_body", retObj).Subscribe(ret =>
            {
                Assert.IsTrue(ret.zPropertyRet.Value == 200);
                Debug.Log("TestChannel : On Send return");

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //can't get result from to Task
            //var testRet = channelClient.Send<TestRet, TestResponseRet>("testfunc_with_body", retObj).ToTask().Result;

            //call testfunc_with_body_and_rawpack
            channelClient.Send<TestRet, TestResponseRet>("testfunc_with_body_and_rawpack", retObj).Subscribe(ret =>
            {
                Assert.IsTrue(ret.zPropertyRet.Value == 200);

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //call testfunc_with_valuetype_return_znull
            channelClient.Send<int, ZNull>("testfunc_with_valuetype_return_znull", 100).Subscribe(ret =>
            {
                Assert.IsTrue(ZNull.IsNull(ret));

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //call testfunc_return_znull
            channelClient.Send<ZNull, ZNull>("testfunc_return_znull", ZNull.Default).Subscribe(ret =>
            {
                Assert.IsTrue(ZNull.IsNull(ret));

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //call testfunc_with_rawdata_return_znull
            channelClient.Send<TestRet, ZNull>("testfunc_with_rawdata_return_znull", retObj).Subscribe(ret =>
            {
                Assert.IsTrue(ZNull.IsNull(ret));

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;


            //call testfunc_with_rawdata_no_return
            //delay for WaitAFetch is Ready
            channelClient.Post<TestRet>("testfunc_with_rawdata_no_return", retObj).Delay(TimeSpan.FromSeconds(0.1f)).Subscribe(ret =>
            {
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //call testfunc_with_no_return
            channelClient.Post<TestRet>("testfunc_with_no_return", retObj).Delay(TimeSpan.FromSeconds(0.1f)).Subscribe(ret =>
            {
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //call testfunc_void
            channelClient.Post<ZNull>("testfunc_void", ZNull.Default).Delay(TimeSpan.FromSeconds(0.1f)).Subscribe(ret =>
            {
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            //[TODO] Test Cancel but socclient Can't be canceled.
            var callToCancelDisp = channelClient.Post<ZNull>("testfunc_void", ZNull.Default).Subscribe(ret =>
            {
                //taskEnd.Value = true;
                //Assert.IsTrue(false);
            });

            callToCancelDisp.Dispose();

            //taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
            //taskEnd.Value = false;

            //[TODO] Test Retry
            channelClient.Post<ZNull>("testfunc_void", ZNull.Default).Retry(3).Subscribe(ret =>
            {
                //taskEnd.Value = true;
                Assert.IsTrue(true);
            });

            //[TODO]test sync with pipeline

            Thread.Sleep(300);



            channelClient.Disconnect();
            //connectDisp.Dispose();

            channelClient = client.GetChannelClient("Test");
            Assert.IsTrue(channelClient != null);

            channelClient.CheckClientAndWaitStatus(ChannelStatusEnum.Closed).WaitAFetch(2);

            Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Closed, "Check channelClient.Status When End");

        }

        //test Connect 2 with ServerChannel
        static void TestClientWithConnect2(IRoomClient client)
        {

            var roomServer = StartUp.Builder.FindRoom(3);

            var serverChannel = roomServer.GetChannel<TestChannel>();

            Assert.IsTrue(serverChannel != null);

            var runId = MatrixRuntimeTools.GetRunId();

            //get test channel
            var channelClient = client.GetChannelClient("Test");

            var channelInfo = channelClient.GetInfo();

            var retObj = ZPropertyMesh.CreateObject<TestRet>();
            retObj.zProperty.Value = 100;

            Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Listen
                || channelClient.Status == Domain.ChannelStatusEnum.Disabled
                || channelClient.Status == Domain.ChannelStatusEnum.Closed);

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var connectDisp = channelClient.Connect2().Subscribe(_ =>
            {
                //it not changed the status
                Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Opened);

                //Connect2 will wait for the server status to update
                Assert.IsTrue(serverChannel.Status == ChannelStatusEnum.Opened);
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            channelClient.Send<TestRet, TestResponseRet>("testfunc_with_body", retObj).Subscribe(ret =>
            {
                Assert.IsTrue(ret.zPropertyRet.Value == 200);
                Debug.Log("TestChannel : On Send return");

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            serverChannel.OnDisConnectedObservable.Subscribe(clientId =>
            {
                taskEnd.Value = true;
            });

            //disconnect
            channelClient.Disconnect();
            //connectDisp.Dispose();

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            Assert.IsTrue(serverChannel.Status == Domain.ChannelStatusEnum.Closed);

            channelClient = client.GetChannelClient("Test");
            Assert.IsTrue(channelClient != null);

            channelClient.CheckClientAndWaitStatus(ChannelStatusEnum.Closed).WaitAFetch();

            Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Closed);
        }

        static void TestServerChannelToTestClient(IRoomClient client)
        {
            //link to client Channel
            var roomServer = StartUp.Builder.FindRoom(3);
            Assert.IsTrue(roomServer != null);

            //var serverChannel = roomServer.GetChannel<TestChannel>();

            //Assert.IsTrue(serverChannel != null);
            IReactiveProperty<bool> roomTaskEnd = new ReactiveProperty<bool>(false);

            roomServer.RunInRoom(() =>
            {
                var runId = MatrixRuntimeTools.GetRunId();

                var channelClient = roomServer.GetChannelClient("TestClient", "1001");

                var channelInfo = channelClient.GetInfo();

                var retObj = ZPropertyMesh.CreateObject<TestRet>();
                retObj.zProperty.Value = 100;

                Assert.IsTrue(channelClient.Status != Domain.ChannelStatusEnum.Opened);// || channelClient.Status == Domain.ChannelStatusEnum.Disabled);

                IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

                var connectDisp = channelClient.Connect().Subscribe(_ =>
                {
                    //it not changed the status
                    Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Opened);
                    taskEnd.Value = true;
                });

                taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                taskEnd.Value = false;

                //call testfunc_with_body
                channelClient.Send<TestRet, TestResponseRet>("testfunc_with_body", retObj).Subscribe(ret =>
                {
                    Assert.IsTrue(ret.zPropertyRet.Value == 200);
                    Debug.Log("TestChannel : On Send return");

                    taskEnd.Value = true;
                });

                taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                taskEnd.Value = false;

                //call testfunc_with_body_and_rawpack
                channelClient.Send<TestRet, TestResponseRet>("testfunc_with_body_and_rawpack", retObj).Subscribe(ret =>
                {
                    Assert.IsTrue(ret.zPropertyRet.Value == 200);

                    taskEnd.Value = true;
                });

                taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                taskEnd.Value = false;

                channelClient.Disconnect();

                channelClient.CheckClientAndWaitStatus(ChannelStatusEnum.Closed).WaitAFetch();

                Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Closed);


                roomTaskEnd.Value = true;
            });

            roomTaskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
        }

        [Test]
        public void Test2Client()
        {
            var clientBuilder = StartUp.ClientBuilder;

            //IReactiveProperty<bool> testEnd = new ReactiveProperty<bool>(false);
            var callCount = new InterCountReactiveProperty(0);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var clients = new string[] { "1001", "1002" };

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

                            TestClientAsync(client).Wait();

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);
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

                            //Assert.IsTrue(roomServer.Count == 0);

                            //[TODO] Check the Architect Server current room's Status
                            callCount.Increment();
                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 2, 100).ToTask().Wait();

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.WaitStoped().WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }


        [Test]
        public void TestSyncClient()
        {
            var clientBuilder = StartUp.ClientBuilder;

            //IReactiveProperty<bool> testEnd = new ReactiveProperty<bool>(false);
            var callCount = new InterCountReactiveProperty(0);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var clients = new string[] { "1001", "1002" };

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

                            TestClientAsync(client).Wait();

                            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);
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

                            //Assert.IsTrue(roomServer.Count == 0);

                            //[TODO] Check the Architect Server current room's Status
                            callCount.Increment();
                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 2, 100).ToTask().Wait();

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.WaitStoped().WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }


        [Test]
        public void TestClientWithConnectErrorHandler()
        {
            var clientBuilder = StartUp.ClientBuilder;

            //IReactiveProperty<bool> testEnd = new ReactiveProperty<bool>(false);
            var callCount = new InterCountReactiveProperty(0);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var c = "1001";

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

                            var channelClient = client.GetChannelClient("Test");


                            var connectDisp = channelClient.Connect().Subscribe(_ =>
                            {
                                //it not changed the status
                                Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Opened);
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;


                            var channelClient2 = client.GetChannelClient("Test");

                            channelClient2.Connect().Subscribe(_ =>
                            {
                                taskEnd.Value = true;
                            },
                            error =>
                            {
                                //handle the exception
                                Assert.IsTrue(error.IsError(ZNetErrorEnum.ClientAlreadyExists));

                                taskEnd.Value = true;
                            });

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

                            //Assert.IsTrue(roomServer.Count == 0);

                            //[TODO] Check the Architect Server current room's Status
                            callCount.Increment();
                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 1, 100).ToTask().Wait();

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.WaitStoped().WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }

        [Test]
        public void TestClientWithActionErrorHandler()
        {
            var clientBuilder = StartUp.ClientBuilder;

            //IReactiveProperty<bool> testEnd = new ReactiveProperty<bool>(false);
            var callCount = new InterCountReactiveProperty(0);

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {

                var c = "1001";

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

                            var channelClient = client.GetChannelClient("Test");


                            var connectDisp = channelClient.Connect().Subscribe(_ =>
                            {
                                //it not changed the status
                                Assert.IsTrue(channelClient.Status == Domain.ChannelStatusEnum.Opened);
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;


                            //call testfunc_return_error
                            channelClient.Send<ZNull, ZNull>("testfunc_return_custom_error", ZNull.Default).Subscribe(ret =>
                            {
                                Assert.IsTrue(false);

                                taskEnd.Value = true;
                            },
                            error =>
                            {
                                Assert.IsTrue(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                                taskEnd.Value = true;
                            });

                            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
                            taskEnd.Value = false;

                            channelClient.Send<ZNull, ZNull>("testfunc_return_error", ZNull.Default).Subscribe(ret =>
                            {
                                Assert.IsTrue(false);

                                taskEnd.Value = true;
                            },
                            error =>
                            {
                                Assert.IsTrue(error.IsError(ZNetErrorEnum.ActionError));

                                Assert.IsFalse(error.IsError(ZNetErrorEnum.ClientAlreadyExists));
                                taskEnd.Value = true;
                            });

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

                            //Assert.IsTrue(roomServer.Count == 0);

                            //[TODO] Check the Architect Server current room's Status
                            callCount.Increment();
                        });
                    });// end OnRunObservable
                }


            });

            callCount.WaitFor(cur => cur >= 1, 100).ToTask().Wait();

            var roomServer = StartUp.Builder.FindRoom(3);

            //all client disconnect will trigger room stop
            roomServer.WaitStoped().WaitAFetch();

            Assert.IsTrue(roomServer.RoomStatus == Domain.RoomStatusEnum.Unused);

            Assert.IsTrue(roomServer.ClientCount == 0);

            ZPropertySocket.CheckRecvListenerCount(4, 4);
        }
    }
}
