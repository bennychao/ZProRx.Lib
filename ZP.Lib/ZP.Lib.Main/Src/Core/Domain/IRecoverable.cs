using System;
namespace ZP.Lib
{
    public interface IRecoverable
    {
        object RedoToTime(uint time);
        void Reset();
    }
}
