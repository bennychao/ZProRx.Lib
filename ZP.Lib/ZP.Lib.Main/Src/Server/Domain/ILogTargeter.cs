using System;
namespace ZP.Lib.Server.Domain
{
    public interface ILogTargeter
    {
        void Write(string strLog);
    }
}
