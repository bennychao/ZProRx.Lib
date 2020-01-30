using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UniRx.Operators;

namespace ZP.Lib.CoreEx.Reactive
{
    abstract public class MultiOperatorObservableBase<T>: OperatorObservableBase<T>
    {
        private bool isDisposed;

        protected CompositeDisposable suObservers = new CompositeDisposable();

        public MultiOperatorObservableBase(bool isRequiredSubscribeOnCurrentThread) : base(isRequiredSubscribeOnCurrentThread)
        {

        }

        ~MultiOperatorObservableBase()
        {
            this.Dispose(false);
        }

        protected IDisposable Register(IDisposable disposable)
        {
            suObservers.Add(disposable);
            return disposable;
        }

        protected void UnRegister(IDisposable disposable)
        {
            suObservers.Remove(disposable);

            //check dispose
            if (suObservers.Count <= 0)
                Dispose(true);
        }

        /// <summary>
        /// Not Thread Safe
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    suObservers.Dispose();
                }
            }
            isDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

    }
}
