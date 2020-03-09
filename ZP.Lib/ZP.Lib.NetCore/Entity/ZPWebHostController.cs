using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using UniRx;
using ZP.Lib.NetCore.Domain;

namespace ZP.Lib.NetCore.Entity
{
    public class ZPWebHostController : IWebHostController
    {
        private Subject<Unit> shutSubject = new Subject<Unit>();
        public IWebHost WebHost { get; set; }

        public IObservable<Unit> ShutObservable => shutSubject;

        public bool Shutdown()
        {
            Task.Run(async () =>
            {
                await WebHost?.StopAsync();

                shutSubject.OnNext(Unit.Default);
            });

            return WebHost != null;
        }
    }
}
