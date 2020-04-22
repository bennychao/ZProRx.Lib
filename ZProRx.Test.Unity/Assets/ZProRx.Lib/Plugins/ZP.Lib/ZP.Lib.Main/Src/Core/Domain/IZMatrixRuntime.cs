using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Core.Domain
{
    public interface IZMatrixRuntime
    {
        string RunId { get; }

        short RoomId { get; }

        bool IsServer {get;}
    }
}
