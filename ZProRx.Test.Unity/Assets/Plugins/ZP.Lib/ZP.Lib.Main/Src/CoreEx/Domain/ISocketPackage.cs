using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.CoreEx
{
    public interface ISocketPackage
    {
        string Key { get; }

        string Value { get; set; }
        string Topic { get; }
    }

    public interface ISocketPackageGetable
    {
        ISocketPackage SocketPackage { get; }
    }
}
