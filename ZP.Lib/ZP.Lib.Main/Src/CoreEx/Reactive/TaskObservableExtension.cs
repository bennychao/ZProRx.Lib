using System;
using System.Threading.Tasks;
using UniRx;

namespace ZP.Lib.CoreEx.Reactive
{

    #if ZP_SERVER
    public static class TaskObservableExtension
    {
        //IObservable<T>
        
        /// <summary>
        /// Not Used
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        internal static ObservableAwaiter<T> GetAwaiter<T>(this IObservable<T> observable){
            ObservableAwaiter<T> observableAwaiter = new ObservableAwaiter<T>(observable);

            return observableAwaiter;
        }

        /// <summary>
        /// Not Used
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        internal static Task<T> ToTask<T>(this IObservable<T> observable)
        {

            return Task.Run<T>(async () => {
               return  await observable;
                });
        }
    }
    #endif
}
