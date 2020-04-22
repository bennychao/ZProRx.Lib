using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZP.Lib.CoreEx.Domain
{
   public interface ICancellableObserver<T> : IObserver<T>, IDisposable
    {
        CancellationToken Token { get; }

        void Cancel();

    }
}
