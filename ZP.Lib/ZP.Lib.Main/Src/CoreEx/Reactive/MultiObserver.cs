using System;
using System.Collections.Generic;
using System.Text;
using UniRx.Operators;

namespace ZP.Lib.CoreEx
{
    public class MultiObserver<TSource> : IDisposable, IObserver<TSource>
    {
        List<IObserver<TSource>> observers = new List<IObserver<TSource>>();
        IDisposable cancel;

        public MultiObserver()
        {

        }

        public MultiObserver(IDisposable cancel)
        {
            this.cancel = cancel;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    //observer = UniRx.InternalUtil.EmptyObserver<TResult>.Instance;
                    var target = System.Threading.Interlocked.Exchange(ref cancel, null);
                    if (target != null)
                    {
                        target.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~MultiObserver()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion


        static public MultiObserver<TSource> Create(IObserver<TSource> observer)
        {
            var ret = new MultiObserver<TSource>();
            ret.AddObserver(observer);

            return ret;
        }

        static public MultiObserver<TSource> Create(IObserver<TSource> observer, IObserver<TSource> observer2)
        {
            var ret = new MultiObserver<TSource>();
            ret.AddObserver(observer);
            ret.AddObserver(observer2);

            return ret;
        }

        static public MultiObserver<TSource> Create(IObserver<TSource> observer, IObserver<TSource> observer2, IObserver<TSource> observer3)
        {
            var ret = new MultiObserver<TSource>();
            ret.AddObserver(observer);
            ret.AddObserver(observer2);
            ret.AddObserver(observer3);
            return ret;
        }

        public void AddObserver(IObserver<TSource> observar)
        {
            observers.Add(observar);
        }

        public void OnCompleted()
        {
            foreach (var i in observers)
            {
                i.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var i in observers)
            {
                i.OnError(error);
            }
        }

        public void OnNext(TSource value)
        {
            foreach (var i in observers)
            {
                i.OnNext(value);
            }
        }

    }
}
