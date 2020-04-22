using System;
using Microsoft.Extensions.Logging;
using ZP.Lib.Server.Domain;
using ZP.Lib.Server.Tools;
using ZP.Lib.Soc.Domain;

namespace ZP.Lib.Soc.Tools
{
    public class NLogLogTargeter : ILogTargeter
    {
        ILogger logger = null;
        public NLogLogTargeter(ILoggerFactory fact, ISocAppBuilder app)
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
