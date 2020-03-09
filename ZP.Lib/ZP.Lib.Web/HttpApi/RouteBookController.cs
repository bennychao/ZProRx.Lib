using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZP.Lib;
using ZP.Lib.Net;
using ZP.Lib.Web.Domain;

namespace ZP.Lib.Web
{
    internal class RouteBookController : Controller
    {
        public ActionResult<string> PostApiFarwordAction([FromBody] string queryData)
        {
            var apiProvider = HttpContext.RequestServices.GetService(typeof(ApiRoutesProvider)) as ApiRoutesProvider;

            if (apiProvider == null)
            {
                return ZPropertyNetCore.ZsonErrorResult<ZNetErrorEnum>(ZNetErrorEnum.UnsupportedApi);
            }

            //HttpContext.Request.
            var api = apiProvider.Apis.Find(a => MatchAction(a, HttpContext.Request.Path));

            if (api == null)
            {
                return ZPropertyNetCore.ZsonErrorResult<ZNetErrorEnum>(ZNetErrorEnum.UnsupportedAction);
            }

            var obj = ZPropertyMesh.CreateObject(api.ReturnType);

            ZPropertyPrefs.LoadFromStr(obj, queryData);

            api.OnNext(obj);

            return "";
        }

        //TODO should support {id} template
        private bool MatchAction(IApiRoute api, string actionStr)
        {
            return string.Compare(api.Template, actionStr, StringComparison.Ordinal) == 0;
        }
    }
}
