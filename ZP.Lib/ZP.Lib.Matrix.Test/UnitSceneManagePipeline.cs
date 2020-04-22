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

namespace ZP.Lib.Matrix.Test
{
    public class UnitSceneManagePipeline 
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

                Builder = app.UseMatrix();
                    //.AddRuntimeServer<TestRuntimeChannel>();

                app.UseScene();


                var log = app.GetService<ILogger<StartUp>>();
                log.LogWarning("set up");

                //Init Local
                var clientBuilder = app.UseRoomClient()

                    .AddPipeline<TestSceneMgrPipeline>()
                    ;// Init RoomClient End

                Debug.Log("On Configure");

                ClientBuilder = clientBuilder;
                //TestSoc.CreateClient(clientBuilder);
            }

        }


        [SetUp]
        public void Setup()
        {
            var args = new string[] {
                "{\"WorkerParam\":\"run\",\"Port\":5050,\"UnitType\":\"hall\",\"Count\":2}"
            };

            //not work you need config by appsettings.json
            //ServerPath.WorkPath = "../../..";

            Task.Run(() =>
            {
                var app = SocApp.CreateSocApp(args)
                //.UseNacos<Program>()    //to manage the config
                .UseStartup<StartUp>();

                app.Run(() => {
                });
            });

            Thread.Sleep(2000);
        }


        [Test]
        public void TestSceneLoad()
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

                            var scenePipe = client.GetChannel<TestSceneMgrPipeline>();

                            //auto connected
                            scenePipe.CheckAndWaitStatus(Domain.ChannelStatusEnum.Opened).WaitAFetch();                            

                            //Thread.Sleep(1000);

                            //Two User and One Auto Connected AI Client
                            //.ObserveOn(ZPRxScheduler.CurrentScheduler)
                            scenePipe.WaitConnected(2).WaitAFetch();

                            Assert.IsTrue(scenePipe.ClientCount == 2);

                            //load Scene
                            scenePipe.LoadSceneAsync("Scene4").Wait();

                            //TODO check scene is loaded

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

            Assert.IsTrue(roomServer.ClientCount == 0);
        }
    }
}
