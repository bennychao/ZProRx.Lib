using System;
using ZP.Lib.Server.Domain;

namespace ZP.Lib.Server.Engine
{
    public class ConsoleLogTargeter : ILogTargeter
    {
        public void Write(string strLog)
        {
            Console.WriteLine(strLog);
        }
    }
}
