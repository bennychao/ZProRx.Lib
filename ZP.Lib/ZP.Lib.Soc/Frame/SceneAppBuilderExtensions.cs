using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UniRx;
using System.Diagnostics;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc.Domain;

namespace ZP.Lib.Soc
{
    public static class SceneAppBuilderExtensions
    {
        static AssetConfig assetConfig = new AssetConfig();
        public static ISocAppBuilder UseScene(this ISocAppBuilder app)
        {
            var builder = app.ApplicationServices.GetService<ISocRoomBuilder>();
            //builder = app.BuildSubServiceProvider().GetService<ISocRoomBuilder>();

            if (builder == null)
                throw new Exception("No Room Builder be Registered, Can't Create Scene");

            var section = app.Configuration.GetSection("AssetConfig");
            section.Bind(assetConfig);

            builder.OnConnected.Subscribe(roomServer =>
           {
               //roomServer.
               roomServer.RunInRoom(() =>
              {
                  Debug.Assert(roomServer.IsRunInRoom(), "Check Load Scene thread must be in room scheduler");

                  //TODO
                  ZServerScene.Instance.Load(assetConfig.MainScene);
              });
           });


            return app;
        }
    }
}
