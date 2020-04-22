using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZP.Lib.NetCore;
using ZP.Lib.Web;
using ZProRx.Test.Web.Entity;

namespace ZProRx.Matrix.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddZPBinder();

            services.AddMatrix(Configuration);

            services.AddZPSwagger<Program>(Configuration);

            services.Configure<TestConfig>(Configuration.GetSection("TestConfig"));

            services.AddCors(option =>
                option.AddPolicy("cors", policy =>
                policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true))
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            //env.ConfigureNLog("sysnlog.config");
            app.UseZLog("sysnlog.config");

            app.UseMatrix();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors("cors");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseZPSwagger<Program>();
        }
    }
}
