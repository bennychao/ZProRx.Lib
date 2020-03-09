using System;
using Microsoft.AspNetCore.Builder;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.Web;

namespace ZP.Lib.NetCore
{
    static public class ModelsAppBuilderExtensions
    {
        public static IApplicationBuilder InitModel<TModel>(this IApplicationBuilder app)
            where TModel : BaseModel, ICreatableModel
        {
            var aService = app.ApplicationServices.GetService(typeof(IModelsProvider)) as IModelsProvider;

            aService?.GetSingle<TModel>()?.CheckOrCreateDB();

            return app;
        }
    }
}
