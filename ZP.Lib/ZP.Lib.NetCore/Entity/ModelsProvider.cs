using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.Web;

namespace ZP.Lib.NetCore.Entity
{
    public class ModelsProvider : IModelsProvider
    {
        IConfiguration configuration;
        IServiceProvider serviceProvider; 
        Dictionary<Type, BaseModel> models = new Dictionary<Type, BaseModel>();
        public ModelsProvider(IConfiguration configuration, IServiceCollection services)
        {
            this.configuration = configuration;
            serviceProvider = services.BuildServiceProvider();

        }

        public TModel GetModel<TModel>() where TModel : BaseModel
        {
            //var model = Activator.CreateInstance(typeof(TModel), new object[] { configuration }) as TModel;

            var model = ActivatorUtilities.CreateInstance<TModel>(serviceProvider, new object[] { configuration }) as TModel;
            //TODO cache it 
            return model;
        }

        public TModel GetSingle<TModel>() where TModel : BaseModel
        {
            BaseModel model = null;
            if (!models.TryGetValue(typeof(TModel), out model))
            {
                //model = Activator.CreateInstance(typeof(TModel), new object[] { configuration }) as TModel;

                model = ActivatorUtilities.CreateInstance<TModel>(serviceProvider, new object[] { configuration }) as TModel;

                models[typeof(TModel)] = model;

                model.Connect();
            }
            
            //TODO cache it 
            return model as TModel;
        }
    }
}
