using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Standard;

namespace ZP.Lib.Soc
{
    static public class SocAppBuilderExtensions
    {

        public static ISocAppBuilder UseNacos<TStartup>(this ISocAppBuilder appBuilder)
        {
            var config = appBuilder.Services.BuildServiceProvider().GetService<IConfiguration>();
            var newConfig = appBuilder.Services.ChangeToNacosConfig<TStartup>(config);
            //builder.UseConfiguration(newConfig);
            appBuilder.Services.Add(new ServiceDescriptor(typeof(IConfiguration), newConfig));

            return appBuilder;
        }
        public static ISocAppBuilder UseStartup<TStartup>(this ISocAppBuilder appBuilder) where TStartup : class
        {
            var sstartUp = Activator.CreateInstance(typeof(TStartup));
            sstartUp.GetType().InvokeMember("ConfigureServices", BindingFlags.InvokeMethod, null, sstartUp, new object[] { appBuilder.Services });

            appBuilder.Build();

            sstartUp.GetType().InvokeMember("Configure", BindingFlags.InvokeMethod, null, sstartUp, new object[] { appBuilder });
            return appBuilder;
        }
    }
}
