using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using UnityEngine;
using ZP.Lib.Server.Tools;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc.Tools;

namespace ZP.Lib.Soc
{
    static public class ZLogAppBuilderExtensions
    {
        public static IServiceCollection AddZLog(this IServiceCollection services)
        {
            //load Channels
            //var myfactory = new LoggerFactory();
            services.AddLogging();

            return services;
        }

        public static ISocAppBuilder UseZLog(this ISocAppBuilder app, string configPath)
        {
            //app.GetService
            //var test = app.GetService<ILogger<StartUp>>();

            var fact = app.GetService<ILoggerFactory>();
            //fact.AddConsole();
            fact.AddNLog();

            
            //fact.ConfigureNLog("sysnlog.config");
            if (!string.IsNullOrEmpty( configPath))
                NLog.LogManager.LoadConfiguration(configPath);

            //test.LogWarning("Test");

            //support the Debug

            Debug.AddTargeter(new NLogLogTargeter(fact, app));

            return app;
        }
    }
}
