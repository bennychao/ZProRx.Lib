using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using ZP.Lib;
using IAuthorizationFilter = Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter;

namespace ZP.Lib.Web.Domain
{
    public class ZPAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // ReflectedAsyncActionDescriptor
            // ReflectedActionDescriptor
             var list = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.CustomAttributes.ToList().Where(a => {
                 return typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute).IsAssignableFrom(a.AttributeType);
                 }).ToList();

            //int  list?.FindIndex(a => typeof(AuthorizeAttribute) == a.AttributeType);
             bool bRequireAuth = list.Count() > 0;
            //typeof(AuthorizeAttribute).IsAssignableFrom( a.AttributeType)
            //var type1 = context.ActionDescriptor.GetType();
            //bool bRequireAuth = true;// context.ActionDescriptor.

            if (!bRequireAuth ||  context.HttpContext.User.Identity.IsAuthenticated)
                Console.WriteLine("OnAuthorization");
            else
            {
                context.Result = new JsonResult(ZPropertyNet.ErrorResult(Net.ZNetErrorEnum.UnAuthorized));
            }

        }
    }
}
