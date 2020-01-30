using System;
using UniRx;
using ZP.Lib.Core.Interface;
#if ZP_SERVER
//using UniRx.Async;
#endif

namespace ZP.Lib.CoreEx.Reactive
{
#if ZP_SERVER
    internal class ObservableAwaiter<T> : IAwaiter<T>
    {
        IObservable<T> observable { get; set; }
        bool isCompleted = false;

        T result = default(T);
        public ObservableAwaiter(IObservable<T> observable)
        {
            this.observable = observable;
        }

        //public AwaiterStatus Status => AwaiterStatus.Succeeded;

        public bool IsCompleted => isCompleted;

        public T GetResult()
        {
            return result;
        }

        public void OnCompleted(Action continuation)
        {
            //throw new NotImplementedException();
            this.observable.Subscribe(a => {
                result = a;
                isCompleted = true;
                continuation();
            });
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            //throw new NotImplementedException();
            this.observable.Subscribe(a => {
                    result = a;
                isCompleted = true;
                continuation();
            });
        }

        //public void  GetResult()
        //{
        //    //throw new NotImplementedException();
        //}
    }
#endif

}
