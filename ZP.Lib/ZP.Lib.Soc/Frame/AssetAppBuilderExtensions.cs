using System;
using Microsoft.Extensions.Configuration;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc.Domain;

namespace ZP.Lib.Soc
{
    static public class AssetAppBuilderExtensions
    {
        public static ISocAppBuilder UseAssetPath(this ISocAppBuilder app, string path)
        {
            ServerPath.WorkPath = path;
            return app;
        }

        public static ISocAppBuilder UseAssetPath(this ISocAppBuilder app, IConfigurationRoot configuration)
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
