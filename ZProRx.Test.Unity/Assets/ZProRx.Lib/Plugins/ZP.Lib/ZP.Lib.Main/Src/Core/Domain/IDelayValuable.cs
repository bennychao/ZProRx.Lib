using System;
namespace ZP.Lib
{
    public interface IDelayValuable<T>
    {
        bool IsReady();
        // T TryValue { get;  }
        //void DelayValue()
    }
}
