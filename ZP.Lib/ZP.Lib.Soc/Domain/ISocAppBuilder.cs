using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc;
using ZP.Lib.Soc.Domain;

namespace ZP.Lib.Soc.Domain
{
    public interface ISocAppBuilder : IAppBuilder
    {
        //
        // 摘要:
        //     Gets or sets the System.IServiceProvider that provides access to the application's
        //     service container.

        IMatrixConfig MatrixConfiguration { get; }

        IServiceCollection Services { get; }

        IObservable<Unit> OnRunObservable { get; }
        IObservable<Unit> OnStopObservable { get; }

        IConfigurationRoot Configuration { get; }

        //IServiceProvider BuildSubServiceProvider();

        bool IsRunning { get; }

        T GetParam<T>();
        ISocAppBuilder Build();

        void Run();
        void Run(Action action);

        Task RunAsync();
        Task RunAsync(Action action);

        void Stop();
    }
}
