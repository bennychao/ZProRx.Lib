using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Web
{
    public static class ZPCoreMvcBuilderExtensions
    {
        public static IMvcBuilder AddZPBinder(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options =>
            {
                options.ModelBinderProviders.Insert(0, new ZPModelBinderProvider());
            });
            return builder;
        }
    }
}
