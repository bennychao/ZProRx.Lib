using System;
namespace ZP.Lib.Net
{
    public interface IConnectable
    {
        void Connect();
        void Disconnect();
    }
}