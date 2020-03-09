using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZP.Lib;
using ZP.Lib.Net;
using ZP.Lib.Core.Values;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.NetCore.Domain;

namespace ZP.Lib.Web
{
    public class BaseMatrixController : Controller
    {
        IMatrixConfig matrixConfig = null;
        protected IModelsProvider modelsProvider = null;
        public IMatrixConfig MatrixConfig
        {
            get => matrixConfig ?? (matrixConfig = GetMatrixConfig());
        }

        public BaseMatrixController()
        {
            //this.modelsProvider = modelsProvider;
        }

        public BaseMatrixController(IModelsProvider modelsProvider)
        {
            this.modelsProvider = modelsProvider;
        }


        protected IMatrixConfig GetMatrixConfig()
        {
            return HttpContext.RequestServices.GetService(typeof(IMatrixConfig)) as IMatrixConfig;
        }

        protected ZNetErrorEnum OK()
        {
            return ZNetErrorEnum.NoError;
        }

        protected ZPropertyListHub<TData> ListHub<TData>(List<TData> datas)
        {
            var hub = ZPropertyMesh.CreateObject<ZPropertyListHub<TData>>();
            hub.Node.AddRange(datas);
            return hub;
        }

        protected ZPropertyHub<TData> Hub<TData>(TData data)
        {
            var hub = ZPropertyMesh.CreateObject<ZPropertyHub<TData>>();
            hub.Node.Value = data;
            return hub;
        }

        protected ZPropertyHub<TData> Hub<TData>(ZProperty< TData> data)
        {
            var hub = ZPropertyMesh.CreateObject<ZPropertyHub<TData>>();
            hub.Node = data;
            return hub;
        }
    }
}
