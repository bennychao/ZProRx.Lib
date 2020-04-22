using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Soc;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc.Entity;
using UniRx;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;
using ZP.Lib.Core.Values;
using System.Reflection;
using System.Linq;
using UnityEngine;
using ZP.Lib.Matrix.Entity;
using Microsoft.Extensions.Logging;

namespace ZP.Lib.Soc
{
    public static class MatrixAppBuilderExtensions
    {
        static internal bool bPrivateClub = false;

        public static bool IsPrivateClue(this ISocRoomBuilder app)
        {
            return bPrivateClub;
        }

        public static IServiceCollection AddMatrix(this IServiceCollection services)
        {
            //load Channels
            var builder = ZPropertyMesh.CreateObject<SocBuilding>();

            services.Add(new ServiceDescriptor(typeof(ISocRoomBuilder), builder));
            return services;
        }

        public static ISocRoomBuilder UseMatrix(this ISocAppBuilder app)
        {
            Debug.Log("Use Soc Matrix");

            //config asset path
            app.UseAssetPath(app.Configuration);


            var log = app.GetService<ILogger<SocBuilding>>();

            //log.LogWarning(app.)

            var builder = app.ApplicationServices.GetService<ISocRoomBuilder>() as SocBuilding;

            if (builder == null)
                throw new Exception("No Room Builder be Registered");

            var lParam = app.GetParam<MatrixLaunchParam>();
            if (lParam == null)
            {
                throw new Exception("App Launch params is error");
            }

            builder.Port = lParam.Port;
            builder.UnitTypeStr = lParam.UnitType;
            builder.MatrixConfiguration = app.MatrixConfiguration;
            builder.ClientCount = lParam.Count;

            bPrivateClub = lParam.IsPrivateClub;

            app.OnRunObservable.Subscribe(_ => OnRun(app));
            app.OnStopObservable.Subscribe(_ => OnStop(app));

            //set current Id
            ZPropertySocket.ClientID = app.MatrixConfiguration.ServerName;

            return builder;
        }

        static private void OnRun(ISocAppBuilder app)
        {
            var config = app.MatrixConfiguration as MatrixSocConfig;
            //register to master

             Dictionary<string, string> param = new Dictionary<string, string>();
            param["workder"] = config.ServerName;

            var lParam = app.GetParam<MatrixLaunchParam>();
            if (lParam == null)
            {
                throw new Exception("App Launch params is error");
            }

            param["port"] = lParam.Port.ToString();


            if (bPrivateClub)
            {
                Observable.Return(ZNull.Default).Subscribe(_=> CreateClub(app));
            }
            else
            {
                //register server app
                ZPropertyNet.Post<ZDataHub<ZPropertyList<ZRoom>>>(
                        config.ArchitectMasterServer +
                        $"/api/v1/matrix/arch/halles/{config.ServerName}/{lParam.Port}",
                        null).Subscribe(hub =>
                        {
                            CreateBuilding(hub.Node, app);
                        });
            }

        }

        private static void OnStop(ISocAppBuilder app)
        {
            var config = app.MatrixConfiguration as MatrixSocConfig;
            //register to master

            Dictionary<string, string> param = new Dictionary<string, string>();
            param["workder"] = config.ServerName;

            var lParam = app.GetParam<MatrixLaunchParam>();
            if (lParam == null)
            {
                throw new Exception("App Launch params is error");
            }

            param["port"] = lParam.Port.ToString();

            if (!bPrivateClub)
            {
                //unregister server app
                ZPropertyNet.Delete(
                        config.ArchitectMasterServer + $"/api/v1/matrix/arch/halles/{config.ServerName}/{lParam.Port}",
                        null).Subscribe(_ =>
                        {
                        //CreateRoom(hub.Node, app);
                    });
            }

            var builder = app.ApplicationServices.GetService<ISocRoomBuilder>() as SocBuilding;

            builder.Unbuild();
        }


        private static void CreateBuilding(IEnumerable<ZRoom> rooms, ISocAppBuilder app)
        {
            var builder = app.ApplicationServices.GetService<ISocRoomBuilder>();

            if (builder == null)
                throw new Exception("No Room Builder be Registered");

            foreach (var r in rooms)
            {
                var s = builder.AddRoom(r);
            }

            //open the channels
            builder.Build();
        }

        private static void CreateClub(ISocAppBuilder app)
        {
            var intRoomCount = 4;

            var builder = app.ApplicationServices.GetService<ISocRoomBuilder>();
            
            if (builder == null)
                throw new Exception("No Room Builder be Registered");

            for (int i =0; i < intRoomCount; i++)
            {
                var room = ZPropertyMesh.CreateObject<ZRoom>();
                room.Port = builder.Port;
                room.RoomId = i + 1;
                room.Host = ZPropertySocket.ClientIP;

                var s = builder.AddRoom(room);
            }

            //open the channels
            builder.Build();
        }
    }
}
