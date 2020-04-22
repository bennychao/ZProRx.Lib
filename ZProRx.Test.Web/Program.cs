using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ZP.Lib.NetCore;

namespace ZProRx.Matrix.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            //.ConfigureLogging((ILoggingBuilder logBuilder) =>
            //{
            //    logBuilder.AddNLog();
            //    logBuilder.AddConsole();
            //    //logBuilder.confi
            //    NLog.LogManager.LoadConfiguration("sysnlog.config");
            //})
            .UseNacos<Startup>()
            .UseUrls("http://*:6008")
                .UseStartup<Startup>();
    }
}
