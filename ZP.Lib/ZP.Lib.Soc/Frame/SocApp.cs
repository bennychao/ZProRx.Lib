using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using UniRx;

using ZP.Lib.Soc.Entity;
using ZP.Lib.Soc.Domain;
using Microsoft.Extensions.DependencyInjection;
using ZP.Lib.Matrix.Domain;
using UnityEngine;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using ZP.Lib.Matrix;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Soc
{
    public class SocApp : ISocAppBuilder
    {
        static protected ISocAppBuilder singleInstance = null;
        internal static ISocAppBuilder Instance => singleInstance;

        private string[] args;

        private IServiceProvider rootServices = null;

        protected HandObservable<Unit> onRunObservable = new HandObservable<Unit>();
        protected HandObservable<Unit> onStopObservable = new HandObservable<Unit>();

        volatile private int isRunning = 0;

        public bool IsRunning => isRunning != 0;

        public  IServiceCollection Services { get; }
        public IMatrixConfig MatrixConfiguration { get; }
        public  IConfigurationRoot Configuration { get; }

        //root service provider
        public IServiceProvider ApplicationServices => rootServices;

        public IObservable<Unit> OnRunObservable => onRunObservable;

        public IObservable<Unit> OnStopObservable => onStopObservable;

        public static string ProcessDirectory
        {
            get
            {
#if NETSTANDARD1_3
                return AppContext.BaseDirectory;
#else
                return AppDomain.CurrentDomain.BaseDirectory;
#endif
            }
        }

        public SocApp()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            //load matrix config
            MatrixConfiguration = new MatrixSocConfig(Configuration );

            Services = new ServiceCollection();

            Services.Add(new ServiceDescriptor( typeof(IConfiguration), Configuration));

        }

       // static internal ISocAppBuilder Instance => singleInstance;

       static public SocApp CreateSocApp(string[] args) // <Setup>where Setup : new()
        {
            if (singleInstance == null)
            {
                var ret = new SocApp();
                ret.args = args;
                singleInstance = ret;
            }

            // var up = new Setup();

            return singleInstance as SocApp;
        }

        static public ISocAppBuilder CreateSocApp()
        {
            if (singleInstance != null)
            {
                var ret = new SocApp();
                //ret.args = args;
                singleInstance = ret;
            }

            // var up = new Setup();

            return singleInstance as SocApp;
        }

        public IServiceProvider BuildSubServiceProvider()
        {
            return Services.BuildServiceProvider();
        }

        public ISocAppBuilder Build()
        {
            rootServices = Services.BuildServiceProvider();
            return this;
        }

        public void Run()
        {
            if (Interlocked.Exchange(ref isRunning, 1) != 0)
            {
                return;
            }

            onRunObservable.OnNext(Unit.Default);

            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Any Key to Stop!!!!");
                    Console.ReadKey();
                    onStopObservable.OnNext(Unit.Default);
                }
                catch
                {

                }

            });

            while (true)
            {
                if (onStopObservable.IsHand)
                    break;
                Thread.Sleep(1000);
            }

            Thread.Sleep(5000);
            //Console.ReadKey();
        }


        public void Run(Action action)
        {
            if (Interlocked.Exchange(ref isRunning, 1) != 0)
            {
                return;
            }


            onRunObservable.OnNext(Unit.Default);

            var onStopObservableCancellable = onStopObservable.ToCancellable();

            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Any Key to Stop!!!!");
                    Console.ReadKey();
                    
                    onStopObservableCancellable.OnNext(Unit.Default);
                    onStopObservableCancellable.OnCompleted();
                }
                catch
                {

                }

            }, onStopObservableCancellable.Token);

            action();

            while (true)
            {
                if (onStopObservable.IsHand)
                    break;
                Thread.Sleep(1000);
            }
            //Console.ReadKey();

            Thread.Sleep(5000);
        }

        public async Task RunAsync()
        {
            if (Interlocked.Exchange(ref isRunning, 1) != 0)
            {
                return;
            }


            onRunObservable.OnNext(Unit.Default);
            await onStopObservable;

            Thread.Sleep(5000);
        }
        public async Task RunAsync(Action action)
        {
            if (Interlocked.Exchange(ref isRunning, 1) != 0)
            {
                return;
            }


            onRunObservable.OnNext(Unit.Default);

            action();

            await onStopObservable;

            Thread.Sleep(5000);
        }

        public void Stop()
        {
            if (Interlocked.Exchange(ref isRunning, 0) != 1)
            {
                return;
            }


            onStopObservable.OnNext(Unit.Default);
        }

        //to get program start param
        public T GetParam<T>()
        {
            Debug.Log(args[0]);

            var log = GetService<ILogger<SocBuilding>>();
            log.LogWarning("GetParam " + args[0]);

            if (args == null || args.Length <= 0)
                return default(T);

            return JsonConvert.DeserializeObject < T>(args[0]);
        }

        public T GetService<T>()
        {
            return ApplicationServices.GetService<T>();
        }
    }
}
