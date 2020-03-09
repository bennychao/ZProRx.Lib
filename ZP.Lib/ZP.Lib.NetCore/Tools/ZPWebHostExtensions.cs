using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using ZP.Lib.NetCore.Domain;
using System.Threading;
using System.Threading.Tasks;
using ZP.Lib.CoreEx;
using UniRx;

namespace ZP.Lib.NetCore
{
    static public class ZPWebHostExtensions
    {
        static public async Task RunAsync(this IWebHost webHost, int delayTimeout)
        {
            //program.WebHost = webHost;
            var ctl = webHost.Services.GetService(typeof(IWebHostController)) as IWebHostController;

            ctl.WebHost = webHost;

            var shutTask = ctl.ShutObservable.ToTask();

            webHost.Run();

            await shutTask;

            Thread.Sleep(delayTimeout);

            
            //webHost.StopAsync(TimeSpan.FromSeconds(0.1));
        }
    }
}
