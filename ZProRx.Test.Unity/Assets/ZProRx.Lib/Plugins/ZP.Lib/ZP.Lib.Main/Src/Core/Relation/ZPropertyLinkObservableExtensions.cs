using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;

namespace ZP.Lib.Main
{
    public static class ZPropertyLinkObservableExtensions
    {
        static public IDisposable Link<T>(this IZProperty<T> zProperty, IObservable<T> observer){

            return observer.Subscribe(v => zProperty.Value = v);
        }

        static public IDisposable Link<T>(this IZPropertyList<T> propList, IObservable<T> addObserver, IObservable<T> subObserver)
        {
            MultiDisposable disposables = new MultiDisposable();
            addObserver.Subscribe(v => {
                Debug.Log("Link addObserver " + v.ToString());
                propList.Add(v);
             }).AddTo(disposables);

            subObserver.Subscribe(v => {
                Debug.Log("Link subObserver " + v.ToString());
                propList.Remove(pv=> (object)pv == (object)v);
            }).AddTo(disposables);

            return disposables;
        }

        static public IDisposable Link<T>(this ZEvent<T> zEvent, IObservable<T> observer)
        {
            return observer?.Subscribe(v => zEvent.Invoke(v));
        }

        static public IDisposable Link(this ZEvent zEvent, IObservable<ZNull> observer)
        {
            return observer?.Subscribe(v => zEvent.Invoke());
        }

        static public IDisposable Link(this ZEvent zEvent, IObservable<Unit> observer)
        {
            return observer?.Subscribe(v => zEvent.Invoke());
        }

    }
}
