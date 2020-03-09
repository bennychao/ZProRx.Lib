using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

using System.Collections.Generic;
using System.Text;
using ZP.Lib.Web.Domain;
using ZP.Lib.Web.Entity;
using ZP.Lib.Web;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using ZP.Lib.Matrix.Domain;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.NetCore.Entity;

namespace ZP.Lib.Web
{
    public static class MatrixCoreServiceCollectionExtensions
    {
        private static readonly string secretKey = "mysupersecret_secretkey!123";

        public static IServiceCollection AddApiRoute(this IServiceCollection services)
        {
            //singleton
            var sd = new ServiceDescriptor(typeof(ApiRoutesProvider), new ApiRoutesProvider());
            services.Add(sd);
            return services;
        }

        public static IServiceCollection AddMatrix(this IServiceCollection services, IConfiguration configuration)
        {

            //services.Add(new ServiceDescriptor(typeof(IZPWebProgram), ZPProgram.Instance));
            
            //add modelprovider
            services.Add(new ServiceDescriptor(typeof(IModelsProvider), new ModelsProvider(configuration, services)));

            var matrixConfig = new MatrixWebConfig(configuration);
            //if (services.Where(a => a.ServiceType == typeof(IMatrixConfig)).Count() <= 0)
            {
                services.Add(new ServiceDescriptor(typeof(IMatrixConfig), matrixConfig));
            }

            services.AddMvcCore(option =>
            {
                option.Filters.Add(new ZPAuthorizationFilter());
            }).AddAuthorization();//.AddJsonFormatters();

            services.AddAuthentication("Bearer")
                // .AddJwtBearer
                //support ID4
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = matrixConfig.AuthorizationServer;  //"http://localhost:5001";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = matrixConfig.ServerName; // "ZP.WebServer.Demo";
                });
                //.AddJwtBearer(o =>
                //{
                //    o.TokenValidationParameters = new TokenValidationParameters
                //    {
                //        // RequireSignedTokens = false,
                //        NameClaimType = JwtClaimTypes.Name,
                //        RoleClaimType = JwtClaimTypes.Role,
                //        //ValidateIssuerSigningKey = false,
                //        ValidIssuer = "testzb",
                //        ValidAudience = "api",
                //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))

                //        /***********************************TokenValidationParameters的参数默认值***********************************/
                //        // RequireSignedTokens = true,
                //        // SaveSigninToken = false,
                //        // ValidateActor = false,
                //        // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                //        // ValidateAudience = true,
                //        // ValidateIssuer = true, 
                //        // ValidateIssuerSigningKey = false,
                //        // 是否要求Token的Claims中必须包含Expires
                //        // RequireExpirationTime = true,
                //        // 允许的服务器时间偏移量
                //        // ClockSkew = TimeSpan.FromSeconds(300),
                //        // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                //        // ValidateLifetime = true
                //    };
                //});

            //use Zson To result to Json
            services.TryAddSingleton<IActionResultExecutor<ZsonResult>, ZsonResultExecutor>();
            //var sd = new ServiceDescriptor(typeof(ITestInterface), new Class1());
            //services.Add(sd);
            return services;
        }
    }
}
