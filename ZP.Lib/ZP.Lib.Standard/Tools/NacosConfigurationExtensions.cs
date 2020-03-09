using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nacos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZP.Lib.Server.Tools;
using ZP.Lib.Standard.Entity;

namespace ZP.Lib.Standard
{
    public static class NacosConfigurationExtensions
    {
        static IServiceCollection InnerServicesInstance = null;
        public static IServiceCollection GetNacosRuntimeServices()
        {
            if (InnerServicesInstance == null)
            {
                InnerServicesInstance = new ServiceCollection();
            }

            return InnerServicesInstance;
        }

        public static IServiceCollection AddNacosConfigFile(this IServiceCollection services, string group, string Id, string targetPath)
        {
            var loader = services.BuildServiceProvider().GetService<NacosConfigurationLoader>();

            //Assert.NotNull(loader);
            if (loader == null)
                throw new Exception("depend Nacos support, you should add [UseNacos] in appbuilder");

            loader.Get(group, Id, targetPath).Wait();

            return services;
        }

        //use for soc server
        public static IConfiguration ChangeToNacosConfig<TStartup>(this IServiceCollection services,  IConfiguration configuration)
        {
            var config = new NacosConfig();
            configuration.GetSection("NacosConfig")?.Bind(config);

            services.AddNacos(configure =>
            {
                // default timeout
                configure.DefaultTimeOut = config.DefaultTimeOut;
                // nacos's endpoint
                configure.ServerAddresses = new List<string> { config.ServerAddresses };
                // namespace
                configure.Namespace = config.Namespace;
                // listen interval
                configure.ListenInterval = 1000;
            });

            services.AddSingleton<NacosConfigurationLoader>();

            var loader = services.BuildServiceProvider().GetService<NacosConfigurationLoader>();
            //var builder = services.BuildServiceProvider().GetService<IConfigurationBuilder>();
            //change the config
            
            var newRoot =  loader.BuildAsync<TStartup>(config.Group).Result;


            //services.Add(new ServiceDescriptor(typeof(IConfiguration), newRoot));
            //foreach (var c in newRoot.)

            //configuration.Providers 

            return newRoot;
        }

        //use for web server
        public static async Task LoadNacosConfigAsync<TStartup>(this IServiceCollection services, IConfiguration configuration)
        {
            //default config
            var config = new NacosConfig();
            configuration.GetSection("NacosConfig")?.Bind(config);

            services.AddNacos(configure =>
            {
                // default timeout
                configure.DefaultTimeOut = config.DefaultTimeOut;
                // nacos's endpoint
                configure.ServerAddresses = new List<string> { config.ServerAddresses };
                // namespace
                configure.Namespace = config.Namespace;
                // listen interval
                configure.ListenInterval = 1000;
            });

            services.AddSingleton<NacosConfigurationLoader>();

            var loader = services.BuildServiceProvider().GetService<NacosConfigurationLoader>();
            //var builder = services.BuildServiceProvider().GetService<IConfigurationBuilder>();
            //change the config

            var projName = ZPAppTools<TStartup>.AssemblePath;
            // var shortName = ZPAppTools<TStartup>.AssembleShortName;

            var jsonStr = await loader.Get(config.Group, projName);
        }

        /// <summary>
        /// add to host's services
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddListenNacosConfig<TStartup>(this IServiceCollection services)
        {
            //add the Nacos client form inner services
            var innerServices = NacosConfigurationExtensions.GetNacosRuntimeServices();
            var client = innerServices.BuildServiceProvider().GetService<INacosConfigClient>();

            services.Add(new ServiceDescriptor(typeof(INacosConfigClient), client));

            services.AddHostedService<ListenConfigurationBgTask>();
            return services;
        }
    }
}
