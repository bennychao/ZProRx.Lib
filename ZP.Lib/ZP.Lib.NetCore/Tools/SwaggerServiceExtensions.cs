using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZP.Lib.Server.Tools;

namespace ZP.Lib.NetCore
{
    static public class SwaggerServiceExtensions
    {
        public static IServiceCollection AddZPSwagger<Program>(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                var projName = ZPAppTools<Program>.AssemblePath;
                var shortName = ZPAppTools<Program>.AssembleShortName;

                var baseUrl = "https://github.com/bennychao/ZPWorkSpace/blob/master/ZP.Matrix/";

                var info = new  OpenApiInfo
                 {
                    Title = $"{projName} API",
                     Version = "v1",
                     Contact = new OpenApiContact
                     {
                         Name = "Readme",
                         Email = string.Empty,
                         Url = new Uri( baseUrl + $"{projName}/Readme.md")
                     },
                     License = new OpenApiLicense
                     {
                         Name = "许可证名字",
                         Url = new Uri("http://www.cnblogs.com/yilezhu/")
                     },
                 };
                
                c.SwaggerDoc(shortName, info);

                //c.SwaggerDoc(shortName, new OpenApiInfo { Title = $"{projName} API", Version = "v1" });

                // 为 Swagger JSON and UI设置xml文档注释路径

                //Console.WriteLine("Path is " + projName);
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, $"{projName}.xml");
                c.IncludeXmlComments(xmlPath);

                //c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                //{
                //    Description = "权限认证(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",//jwt默认的参数名称
                //    //In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                //    Type = "apiKey"
                //});//Authorization的设置
                //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    { "Bearer", Enumerable.Empty<string>() }
                //});

                c.OperationFilter<SwaggerOperationFilter>();
            });

            return services;
        }

        public static IApplicationBuilder UseZPSwagger<Program>(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var shortName = ZPAppTools<Program>.AssembleShortName;
                c.SwaggerEndpoint($"/swagger/{shortName}/swagger.json", "API V1");
                
                c.InjectJavascript("/swagger_custom.js"); // 加载中文包
            });

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".md"] = "text/plain";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            return app;
        }
    }
}
