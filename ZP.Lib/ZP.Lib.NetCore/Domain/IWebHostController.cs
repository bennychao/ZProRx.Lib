using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using UniRx;

namespace ZP.Lib.NetCore.Domain
{
    public interface IWebHostController
    {
        IWebHost WebHost { get; set; }
        bool Shutdown();
        IObservable<Unit> ShutObservable { get; }
    }
}
