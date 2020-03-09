using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Web
{
    static public class AssetAppBuilderExtensions
    {
        public static IApplicationBuilder UseAssetPath(this IApplicationBuilder app, string path)
        {
            ServerPath.WorkPath = path;
            return app;
        }

        public static IApplicationBuilder UseAssetPath(this IApplicationBuilder app, IConfiguration configuration)
        {
            AssetConfig assetConfig = new AssetConfig();
            var section = configuration.GetSection("AssetConfig");
            section.Bind(assetConfig);

            ServerPath.WorkPath = assetConfig?.AssetPath;
            ServerPath.AppName = assetConfig?.AppName;
            return app;
        }
    }
}
