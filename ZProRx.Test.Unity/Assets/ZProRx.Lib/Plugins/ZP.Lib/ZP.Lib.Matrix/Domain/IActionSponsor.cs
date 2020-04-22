using System;
using System.Collections.Generic;
using System.Text;
using UniRx;

namespace ZP.Lib.Matrix.Domain
{
    public interface IActionSponsor<TAction>
    {
        IObservable<ZNull> PostAction<TData>(TAction cmd, TData data);
    }
}
