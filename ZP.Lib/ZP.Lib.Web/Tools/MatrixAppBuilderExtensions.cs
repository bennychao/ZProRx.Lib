using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Web.Domain;

namespace ZP.Lib.Web
{
    public static class MatrixAppBuilderExtensions
    {
        public static IApplicationBuilder UseMatrix(this IApplicationBuilder app)
        {
           //var ss =   app.ApplicationServices.GetService(typeof(ITestInterface));
            app.UseAuthentication();

            //TODO
            // var iconfig  = app.ApplicationServices.GetService(typeof(IMatrixConfig)) as IMatrixConfig;

            // app.UseAssetPath();

            //set current Id
            var iconfig  = app.ApplicationServices.GetService(typeof(IMatrixConfig)) as IMatrixConfig;
            ZPropertySocket.ClientID = iconfig?.ServerName;

            return app;
        }

        public static IApplicationBuilder UseZLog(this IApplicationBuilder app, string configPath)
        {

            var fact = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            //fact.AddConsole();
            fact.AddNLog();


            //fact.ConfigureNLog("sysnlog.config");
            if (!string.IsNullOrEmpty(configPath))
                NLog.LogManager.LoadConfiguration(configPath);

            //test.LogWarning("Test");

            //support the Debug
            Debug.AddTargeter(new NLogLogTargeter(fact, app));
            return app;
        }
    }
}
