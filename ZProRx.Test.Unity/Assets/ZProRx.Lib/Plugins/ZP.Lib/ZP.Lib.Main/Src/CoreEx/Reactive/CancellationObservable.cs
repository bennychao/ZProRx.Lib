using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UniRx;
using ZP.Lib.CoreEx.Domain;

namespace ZP.Lib.CoreEx
{
    internal class CancellationObservable<T> : RefCountObservable<T>, IDisposable, ICancellableObserver<T>
    {
        bool isDisposed = false;
        private readonly CancellationTokenSource  cancellationTokenSource;

        public CancellationObservable(IObservable<T> source, CancellationTokenSource cancellationTokenSource)
            : base(source)
        {
            if (source as IObserver<T> == null)
                throw new Exception("source is must be a observer");

            this.cancellationTokenSource = cancellationTokenSource;

            this.cancellationTokenSource.Token.Register(Dispose);
        }

        public CancellationObservable(IObservable<T> source)
        : base(source)
        {
            cancellationTokenSource = new CancellationTokenSource();

            this.cancellationTokenSource.Token.Register(Dispose);
        }

        /// <summary>
        /// you can call Token's Cancel, but you should Call Dispose instead.
        /// </summary>
        public CancellationToken Token
        {
            get { return cancellationTokenSource.Token; }
        }

        public bool IsDisposed
        {
            get { return cancellationTokenSource.IsCancellationRequested; }
        }

        /// <summary>
        /// auto call when refcount is 0
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                this.cancellationTokenSource.Cancel();
            }
        }

        public void Cancel()
        {
            
        }

        public void OnNext(T value)
        {
            (source as IObserver<T>)?.OnNext(value);
        }

        public void OnError(Exception error)
        {
            try { (source as IObserver<T>)?.OnError(error); } finally { Dispose(); };
        }

        public void OnCompleted()
        {
            try
            {
                (source as IObserver<T>)?.OnCompleted();
            }
            finally { Dispose(); };
        }
    }
}
