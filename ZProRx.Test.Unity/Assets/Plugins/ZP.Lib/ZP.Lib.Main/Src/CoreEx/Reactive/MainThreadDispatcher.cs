using System;
using ZP.Lib.Common;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using ZP.Lib.Core.Main;

namespace ZP.Lib
{

#if ZP_SERVER
    public sealed class MainThreadDispatcher : PropBehaviourSingletonWithRoom<MainThreadDispatcher>
    {
        Subject<Unit> update = new Subject<Unit>();
        public MainThreadDispatcher()
        {
        }

        private void Awake()
        {

        }

        private void Start()
        {

        }

        private void Update()
        {
            update.OnNext(Unit.Default);
        }

        public IObservable<Unit> RegisterUpdateObservable()
        {   
            return new SubjectObservable<Unit>(update);
        }

        //public IObservable<Unit> UnRegisterUpdateObservable()
        //{
        //    return new SubjectObservable<Unit>(update).ObserveOn(Scheduler.CurrentThread);
        //}
    }
#endif
}
