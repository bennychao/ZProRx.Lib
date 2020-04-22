using ZP.Lib.Soc;
using ZP.Server.Demo;
using ZP.Lib.Common;
using UniRx;
using ZP.Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZProRx.Test.Server.Entity;

namespace ZPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestChannel baseChannel = new TestChannel();
            //baseChannel.Open();
            List<IObservable<ZNull>> rets = new List<IObservable<ZNull>>();
            var o1 = Observable.Return<ZNull>(ZNull.Default).Delay(TimeSpan.FromSeconds(1));
            var o2 = Observable.Return<ZNull>(ZNull.Default).Delay(TimeSpan.FromSeconds(2));
            var o3 = Observable.Return<ZNull>(ZNull.Default).Delay(TimeSpan.FromSeconds(2));
            rets.Add(o1);
            rets.Add(o2);
            rets.Add(o3);

            Observable.WhenAll(rets).Select(_ =>
            {
                return ZNull.Default;
            }).Subscribe(_ => 
            {
                Debug.Log("Test");
            });

            var app = SocApp.CreateSocApp(args)
                .UseNacos<Program>()    //to manage the config
                .UseStartup<StartUp>();

            //use scenes per room or only one

            app.Run( ()=>{
                    //for Test Socket work with Untiy 
                    //it can run without ths ZP.Lib.Soc support, only base the ZP.Lib.Main
                    TestSocketRawServer.RunServer(app);
                });
            //Run end

        }
    }
}
