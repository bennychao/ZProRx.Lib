using System;
using UniRx;
using ZP.Lib.Net;

namespace ZP.Lib.CoreEx
{

    public interface IObservableWithScheduler
    {
        IScheduler scheduler { get; set; }
    }

    public interface IObservableWithId<T>
    {
        T Id { get; }
    }


}
