using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Server.Domain;
using ZP.Lib.Server.Tools;

namespace ZP.Lib.Web
{
    public class NLogLogTargeter : ILogTargeter
    {
        ILogger logger = null;
        public NLogLogTargeter(ILoggerFactory fact, IApplicationBuilder app)
        {

            var appTools = new ZPAppTools(app.GetType());

            logger = fact.CreateLogger(appTools.AssembleShortName);
        }

        public void Write(string strLog)
        {
            logger?.LogWarning(strLog);
        }
    }
}
