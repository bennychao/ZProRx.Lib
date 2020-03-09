using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZP.Lib;

namespace ZP.Lib.Web
{
    //for Frombody bind
    class ZPModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            //throw new NotImplementedException();

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var typ = context.Metadata.ModelType;
            if ((ZPropertyMesh.IsPropertable(typ) || ZPropertyMesh.IsPropertableLowAPI(typ))             //
                && context.BindingInfo.BindingSource == BindingSource.Body)
                return new BinderTypeModelBinder(typeof(ZPModelBinder));

            //to do default binder
            return null;
        }
    }

    public class ZPModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string key = bindingContext.ModelName;
            //if (bindingContext.BindingSource == BindingSource.Body
            //    && (ZPropertyMesh.IsPropertable(bindingContext.ModelType) || ZPropertyMesh.IsPropertableLowAPI(bindingContext.ModelType))) 
            {
                using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
                {
                    var body = reader.ReadToEndAsync().Result;
                    //bindingContext.Model = body;
                    // Do something

                    var model = ZPropertyMesh.CreateObject(bindingContext.ModelType);
                     ZPropertyPrefs.LoadFromStr(model, body);
                    bindingContext.Model = model;
                }
                if (bindingContext.Model != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
                }
            }

            return Task.CompletedTask;
        }
    }
}
