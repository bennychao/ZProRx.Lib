using System;
using UniRx;
using ZP.Lib;

namespace UniRx
{
#if ZP_SERVER
    public enum FrameCountType
    {
        Update,
        FixedUpdate,
        EndOfFrame,
    }
#endif

    public partial class ObservableEx
    {
        //run one time
        public static IObservable<Unit> NextFrame(FrameCountType frameCountType = FrameCountType.Update)
        {
            IObservable<Unit> ret = null;
#if ZP_SERVER
            ret = MainThreadDispatcher.Instance.RegisterUpdateObservable();
            ret.Subscribe(_ => (ret as SubjectObservable<Unit>)?.Dispose());
#else
            ret = Observable.NextFrame(frameCountType);
#endif
            return ret.ObserveOn(Scheduler.CurrentThread);
        }

        public static IObservable<Unit> EveryUpdate(FrameCountType frameCountType = FrameCountType.Update)
        {
            IObservable<Unit> ret = null;
#if ZP_SERVER
            ret = MainThreadDispatcher.Instance.RegisterUpdateObservable();
#else
            ret = Observable.NextFrame(frameCountType);
#endif
            return ret.ObserveOn(Scheduler.CurrentThread);
        }
    }
}
