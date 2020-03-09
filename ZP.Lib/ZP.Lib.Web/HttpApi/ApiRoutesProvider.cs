using System;
using System.Collections.Generic;
using ZP.Lib.Web.Domain;

namespace ZP.Lib.Web
{
    public class ApiRoutesProvider
    {
        public List<IApiRoute> Apis = new List<IApiRoute>();

        public ApiRoutesProvider()
        {
        }
    }
}
