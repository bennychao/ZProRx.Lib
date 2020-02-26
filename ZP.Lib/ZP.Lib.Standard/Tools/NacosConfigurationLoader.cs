using Microsoft.Extensions.Configuration;
using Nacos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZP.Lib.Server.Tools;

namespace ZP.Lib.Standard.Tools
{
    public class NacosConfigurationLoader
    {
        private readonly INacosConfigClient configClient;
        public NacosConfigurationLoader(INacosConfigClient configClient, IConfiguration localConfig)
        {
            this.configClient = configClient;

        }

        public async Task<string> Get(string group, string key)
        {

            var res = "";

            try
            {
                res = await configClient.GetConfigAsync(new GetConfigRequest
                {
                    DataId = key,
                    Group = group,
                    //Tenant = "tenant"
                });
            }
            catch (Exception e)
            {
                
            }


            if (!string.IsNullOrWhiteSpace(res))
            {
                JsonTools.Write("appsettings.nacos.json", res);
            }

            return res;
        }

        public async Task<string> Get(string group, string key, string targetPath)
        {
            var res = "";
            try
            {
                res = await configClient.GetConfigAsync(new GetConfigRequest
                {
                    DataId = key,
                    Group = group,
                    //Tenant = "tenant"
                });

            }
            catch
            {

            }

            if (!string.IsNullOrWhiteSpace(res))
            {
                JsonTools.Write(targetPath, res);
            }

            return res;
        }

        public static string ProcessDirectory
        {
            get
            {
#if NETSTANDARD1_3
                return AppContext.BaseDirectory;
#else
                //return AppDomain.CurrentDomain.BaseDirectory;

                return System.Environment.CurrentDirectory;
#endif
            }
        }

        public async Task<IConfigurationRoot> BuildAsync<TStartup>(string group)
        {
            var projName = ZPAppTools<TStartup>.AssemblePath;
           // var shortName = ZPAppTools<TStartup>.AssembleShortName;

           var jsonStr =  await Get(group, projName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json");    //use local

            if (!string.IsNullOrWhiteSpace(jsonStr))
                  builder.AddJsonFile("appsettings.nacos.json", false, true);
            //else
     

            var configurationRet = builder.Build();

            return configurationRet;
        }

    }
}
