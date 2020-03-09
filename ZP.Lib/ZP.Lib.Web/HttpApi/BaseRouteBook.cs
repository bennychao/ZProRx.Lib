using System;
using Microsoft.AspNetCore.Routing;

using Microsoft.AspNetCore.Builder;
using System.Reflection;
using ZP.Lib.Web.Domain;
using System.Collections.Generic;

namespace ZP.Lib.Web
{
    public class BaseRouteBook<T>
    {

        IApplicationBuilder app;
        public BaseRouteBook(IApplicationBuilder app)
        {
            this.app = app;
        }

        public void ConfigRoute(IRouteBuilder routes)
        {
            
            var routeFileds = GetFiled();

            foreach (var r in routeFileds)
            {
                routes.MapRoute(
                     name: r.name,
                     template: r.template,
                     defaults: new { controller = "RouteBook", action = r.action });
            }

            //routes.MapRoute(
            //     name: "about-route",
            //     template: "about11/{id}",
            //     defaults: new { controller = "Home", action = "Index2" });
        }

        private List<(string name, string template, string action)> GetFiled()
        {
            List<(string name, string template, string action)> rets = new List<(string name, string template, string action)>();

            FieldInfo[] typeInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var apisProvider = app.ApplicationServices.GetService(typeof(ApiRoutesProvider)) as ApiRoutesProvider;

            foreach (var t in typeInfos)
            {
                var api = t.GetValue(this) as IApiRoute;
                if (api == null)
                    continue;
                apisProvider.Apis.Add(api);

                rets.Add(("about-route", api.Template, "PostApiFarwordAction"));
            }

            return rets;

        }
    }


}
