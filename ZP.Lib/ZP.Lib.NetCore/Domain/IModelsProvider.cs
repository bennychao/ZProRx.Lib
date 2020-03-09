using System;
using ZP.Lib.Web;

namespace ZP.Lib.NetCore.Domain
{
    public interface IModelsProvider
    {
        TModel GetModel<TModel>() where TModel : BaseModel;

        TModel GetSingle<TModel>() where TModel : BaseModel;

        //TModel GetModel(Type) where TModel : BaseModel;
    }
}
