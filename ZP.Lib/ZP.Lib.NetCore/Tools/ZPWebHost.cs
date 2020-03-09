using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ZP.Lib;
using ZP.Lib.Net;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.NetCore.Entity;
using ZP.Lib.Web;
using UniRx;
using Microsoft.Extensions.Hosting;

namespace ZP.Lib.NetCore
{


    static public class ZPWebHost
    {

        public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.Add(new ServiceDescriptor(typeof(IWebHostController), new ZPWebHostController()));
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.Add(new ServiceDescriptor(typeof(IWebHostController), new ZPWebHostController()));
            });
    }
}
