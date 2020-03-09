using System;
using ZP.Lib.Net;

namespace ZP.Matrix.Architect.Domain
{
    public enum RoomErrorEnum 
    {
        BaseError = ZNetErrorEnum.MaxError + 0x100,
        UnsupportedUnit,
        NotFindUnit,
        NotFindRoom,
        NotInService,
    }
}