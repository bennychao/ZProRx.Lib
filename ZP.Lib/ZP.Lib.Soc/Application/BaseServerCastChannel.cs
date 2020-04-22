using System;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc;
using ZP.Lib.Soc.Entity;

namespace ZP.Lib.Soc
{
    public class BaseServerCastChannel : ServerChannel
    {

        public BaseServerCastChannel(IRoomServer roomServer, string clientName)
        : base(roomServer, clientName)
        {


        }

    }
}
