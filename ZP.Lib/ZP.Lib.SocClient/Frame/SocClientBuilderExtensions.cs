using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;
using ZP.Lib.SocClient;

namespace ZP.Lib.SocClient
{
    public static class SocClientBuilderExtensions
    {

        public static IServiceCollection AddRoomClient(this IServiceCollection services)
        {
            //load Channels
            var builder = ZPropertyMesh.CreateObject<RoomClientBuilder>();

            services.Add(new ServiceDescriptor(typeof(ISocClientBuilder), builder));
            return services;
        }

        static public ISocClientBuilder UseRoomClient(this IAppBuilder app)
        {
            var builder = app.GetService<ISocClientBuilder>() as RoomClientBuilder;
            // channelTypes.Add(typeof(T));
            
            if (builder == null)
                throw new Exception("No Client Builder be Registered");

            builder.AppBuilder = app;

            return builder;
        }
    }
}
