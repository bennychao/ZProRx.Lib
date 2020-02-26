using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UniRx.Operators;
using UniRx;
using ZP.Lib.CoreEx.Domain;

namespace ZP.Lib.CoreEx
{
    //ToCancellable Operator
    internal class CancellableOperator<T> : IDisposable, ICancellableObserver<T>
    {
        protected internal volatile IObserver<T> observer;
        bool isDisposed = false;
        private readonly CancellationTokenSource cancellationTokenSource;
        public CancellableOperator(IObserver<T> observer, CancellationTokenSource cancellationTokenSource)
            // : base(observer, cancel)
        {
            this.observer = observer;

            this.cancellationTokenSource = cancellationTokenSource;

            this.cancellationTokenSource.Token.Register(Dispose);
        }

        public CancellableOperator(IObserver<T> observer)
        // : base(observer, cancel)
        {
            this.observer = observer;

            this.cancellationTokenSource = new CancellationTokenSource(); ;

            this.cancellationTokenSource.Token.Register(Dispose);
        }

        public CancellationToken Token
        {
            get { return cancellationTokenSource.Token; }
        }

        public void Cancel()
        {
            //throw new NotImplementedException();
            Dispose();
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                this.cancellationTokenSource.Cancel();
            }
        }

        public void OnNext(T value)
        {
            (observer as IObserver<T>)?.OnNext(value);
        }

        public void OnError(Exception error)
        {
            try { (observer as IObserver<T>)?.OnError(error); } finally { Dispose(); };
        }

        public void OnCompleted()
        {
            try
            {
                (observer as IObserver<T>)?.OnCompleted();
            }
            finally { Dispose(); };
        }
    }
}
