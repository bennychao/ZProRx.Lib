using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
