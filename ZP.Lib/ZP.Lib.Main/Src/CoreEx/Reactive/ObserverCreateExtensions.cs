using System;
using System.Collections.Generic;
using System.Text;
using UniRx;

namespace ZP.Lib.CoreEx.Reactive
{
    static class ObserverEx
    {
        public static IObserver<T> CreateOnTerminate<T>(Action onTerminate)
        {
            return Observer.Create<T>(_=> onTerminate(), _ => onTerminate(), () => onTerminate());
        }
    }
}
