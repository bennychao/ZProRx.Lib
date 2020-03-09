using System;
namespace ZP.Lib.Matrix.Domain
{
    public enum RoomStatusEnum
    {
        Unused, //will delete in socket server
        Ordered, //user have created
        Occupied,
        Released
    }
}
