using System;
using System.Collections.Generic;
using System.Text;
using UniRx;

namespace ZP.Lib.CoreEx.Status
{
    public interface IStatusProperty
    {
        IObservable<uint> EnterObservable { get; }

        IObservable<uint> LeaveObservable { get; }

        uint CurStatusValue { get; }
    }

    public interface IStatusProperty<S> : IStatusProperty
    {
        S CurStatus { get; }
    }
}
