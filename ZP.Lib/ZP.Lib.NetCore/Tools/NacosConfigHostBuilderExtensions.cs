using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Standard;

namespace ZP.Lib.NetCore
{
    static public class NacosConfigHostBuilderExtensions
    {
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

        public static IConfigurationBuilder AddNacosFiles<TProgram>(this IConfigurationBuilder builder)
        {
            //mini services
            var innerConfig = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var innerServices = NacosConfigurationExtensions.GetNacosRuntimeServices(); //new ServiceCollection();

            innerServices.AddLogging();
            innerServices.Add(new ServiceDescriptor(typeof(IConfiguration), innerConfig));

            innerServices.LoadNacosConfigAsync<TProgram>(innerConfig).Wait();

            if (System.IO.File.Exists("appsettings.nacos.json"))
                builder.AddJsonFile("appsettings.nacos.json", false, true);
            //innerServices.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));
            return builder;
        }

        public static IConfigurationBuilder AddNacos<TProgram>(this IConfigurationBuilder builder)
        {
            //mini services
            var innerConfig = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var innerServices = NacosConfigurationExtensions.GetNacosRuntimeServices(); //new ServiceCollection();

            innerServices.AddLogging();
            innerServices.Add(new ServiceDescriptor(typeof(IConfiguration), innerConfig));

            innerServices.LoadNacosConfigAsync<TProgram>(innerConfig).Wait();

            //if (System.IO.File.Exists("appsettings.nacos.json"))
            //    builder.AddJsonFile("appsettings.nacos.json", false, true);
            //innerServices.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));
            return builder;
        }

        public static IHostBuilder UseNacos<TProgram>(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //
                //var config = services.BuildServiceProvider().GetService<IConfiguration>();
                //var newConfig = services.ChangeToNacosConfig<TProgram>(config);

                // ! //it not worked, only update the first provider
                //builder.UseConfiguration(newConfig);

                // config = services.BuildServiceProvider().GetService<IConfiguration>();
                //listen the 
                //services.AddHostedService<ListenConfigurationBgTask>();
            })
           .ConfigureAppConfiguration(configBuilder =>
           {
               if (System.IO.File.Exists("appsettings.nacos.json"))
               {
                   configBuilder.AddJsonFile("appsettings.nacos.json", false, true);

                   configBuilder.AddNacos<TProgram>();
               }
               else
               {
                   configBuilder.AddNacosFiles<TProgram>();
               }

           });

            return builder;
        }

       // [Obsolete]
        public static IWebHostBuilder UseNacos<TProgram>(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
           {
               //
               //var config = services.BuildServiceProvider().GetService<IConfiguration>();
               //var newConfig = services.ChangeToNacosConfig<TProgram>(config);

               // ! //it not worked, only update the first provider
               //builder.UseConfiguration(newConfig);

               // config = services.BuildServiceProvider().GetService<IConfiguration>();
               //listen the 
               //services.AddHostedService<ListenConfigurationBgTask>();
           })
           .ConfigureAppConfiguration( configBuilder =>
           {
               if (System.IO.File.Exists("appsettings.nacos.json")){
                   configBuilder.AddJsonFile("appsettings.nacos.json", false, true);

                   configBuilder.AddNacos<TProgram>();
               }
               else
               {
                   configBuilder.AddNacosFiles<TProgram>();
               }
               
           });

            return builder;
        }
    }
}
