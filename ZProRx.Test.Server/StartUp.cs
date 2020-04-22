using Microsoft.Extensions.DependencyInjection;
using System;
using UniRx;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;
using ZP.Lib.SocClient;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZP.Lib.Unity;
using ZP.Lib.Battle;
using ZP.Lib.Matrix;
using ZP.Lib;
using ZP.Lib.Card;
using ZP.Lib.Matrix.Test.Entity;

namespace ZP.Server.Demo
{
    internal class StartUp
    {
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

            app.Use3T2Axes(axes =>
            {
                axes.TRS(Vector3.zero, Vector3.up, Vector3.one);
            });

            app.UseMatrix()
                .AddChannelType<TestChannel>()
                .AddRuntimeServer<TestRuntimeChannel>()
                .AddRoundServer<TestRoundChannel>() //support add multi

                ;//End UseMatrix

            app.UseScene();


            var log = app.GetService<ILogger<StartUp>>();
            log.LogWarning("set up");

           //Init Local for Computer 
            var clientBuilder = app.UseRoomClient()
                //! init local client [For Test Chapter] 
                .AddChannelType<TestClientChannel>()

                .AddPipeline<TestRuntimePipeline>()
                .AddPipeline<TestBroadPipeline>()       //use default broad server channel
                .AddPipeline<MultiCastPipeline>()       //use default multicast pipeline and server channel
                .AddPipeline<UniCastPipeline>()
                .AddPipeline<TestRoundPipeline>()
                .AddPipeline<TestSceneMgrPipeline>()  //for socClient not need App Scene Pipeline
                
                ;// Init RoomClient End
                       

            Debug.Log("On Configure");

            //Create Test local Client
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => clientBuilder.CreateClient("1001", 3));
        }

    }
}
