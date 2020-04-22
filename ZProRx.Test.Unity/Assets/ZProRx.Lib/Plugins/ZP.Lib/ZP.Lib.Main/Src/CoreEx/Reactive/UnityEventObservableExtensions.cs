using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine.Events;


namespace ZP.Lib.CoreEx
{
    static public class ZObservableUnityEventExtensions
    {
        public static IObservable<Unit> ToObservable(this UnityEngine.Events.UnityEvent unityEvent)
        {
            return Observable.FromEvent<UnityAction>(h => new UnityAction(h), h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<T> ToObservable<T>(this UnityEngine.Events.UnityEvent<T> unityEvent)
        {
            return Observable.FromEvent<UnityAction<T>, T>(h => new UnityAction<T>(h), h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<Tuple<T0, T1>> ToObservable<T0, T1>(this UnityEngine.Events.UnityEvent<T0, T1> unityEvent)
        {
            return Observable.FromEvent<UnityAction<T0, T1>, Tuple<T0, T1>>(h =>
            {
                return new UnityAction<T0, T1>((t0, t1) =>
                {
                    h(Tuple.Create(t0, t1));
                });
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<Tuple<T0, T1, T2>> ToObservable<T0, T1, T2>(this UnityEngine.Events.UnityEvent<T0, T1, T2> unityEvent)
        {
            return Observable.FromEvent<UnityAction<T0, T1, T2>, Tuple<T0, T1, T2>>(h =>
            {
                return new UnityAction<T0, T1, T2>((t0, t1, t2) =>
                {
                    h(Tuple.Create(t0, t1, t2));
                });
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<Tuple<T0, T1, T2, T3>> ToObservable<T0, T1, T2, T3>(this UnityEngine.Events.UnityEvent<T0, T1, T2, T3> unityEvent)
        {
            return Observable.FromEvent<UnityAction<T0, T1, T2, T3>, Tuple<T0, T1, T2, T3>>(h =>
            {
                return new UnityAction<T0, T1, T2, T3>((t0, t1, t2, t3) =>
                {
                    h(Tuple.Create(t0, t1, t2, t3));
                });
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }
    }
}