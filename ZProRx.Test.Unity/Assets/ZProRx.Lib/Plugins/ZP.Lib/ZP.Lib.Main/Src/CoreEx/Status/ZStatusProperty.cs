using System;
#if ZP_UNIRX
using UniRx;
using ZP.Lib.Common;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.CoreEx.Status;

namespace ZP.Lib
{
    //ZStatusProperty need UniRX Support
    public class ZStatusProperty<S, E> : ZLinkProperty<S>, IStatusProperty<S>
        where S : IComparable
        where E : IComparable
    {
        protected struct TrackPack
        {
            public E e;
            public S to;

            public TrackPack(E e, S to)
            {
                this.e = e;
                this.to = to;
            }

            public TrackPack(S to)
            {
                this.e = default(E);
                this.to = to;
            }
        }

        public class ZPropStatus : ZP.Lib.Common.Status<S, E>
        {

            public ZStatusProperty<S, E> parent;
            public override void OnEnter(S from, S to, E e)
            {
                parent.Value = to;
                parent.OldStatus = from;
                parent.CurEvent = e;

                parent.Enter.OnNext(new TrackPack(e, to));
            }

            public override void OnLeave()
            {
                parent.OldStatus = parent.zFSM.CurStatusID;

                parent.Leave.OnNext(parent.OldStatus);
            }
        }

        public S OldStatus;
        public E CurEvent;

        public S StartStatus;

        protected ZFSM<S, E> zFSM;// = new ZFSM<S, E>();

        protected Subject<TrackPack> Enter = new Subject<TrackPack>();
        protected Subject<S> Leave = new Subject<S>();

        public S CurStatus => Value;

        public uint CurStatusValue => (uint)Convert.ToInt32(StartStatus);

        public IObservable<uint> EnterObservable => Enter.Select(s => (uint)Convert.ToInt32(s.to));

        public IObservable<uint> LeaveObservable => Leave.Select(s => (uint)Convert.ToInt32(s));


        //public ZStatusProperty()
        //{
        //    //zFSM = ZPropertyMesh.CreateObject<ZFSM<S, E>>();

        //    //zFSM.StatusType = typeof(ZPropStatus);
        //}

        public IObservable<ZNull> EnterStatusObservable(S targetS)
        {
            return Enter.Where(p => p.to.CompareTo(targetS) == 0).
                Select(p => ZNull.Default);
        }

        public IObservable<ZNull> LeaveStatusObservable(S targetS)
        {
            return Leave.Where(s => s.CompareTo(targetS) == 0).Select(_ => ZNull.Default);
        }

        protected override void SetLink(IZProperty prop)
        {
            base.SetLink(prop);

            zFSM = prop?.Value as ZFSM<S, E>;
            if (zFSM != null)
                InitParent();

            LinkProperty.OnValueChanged += v =>
            {
                //if it is runtime v is cur value
                zFSM = prop?.Value as ZFSM<S, E>;
                if (zFSM != null)
                    InitParent();
            };
        }

        public void BindTransfers(string str) {
            ZPropertyPrefs.LoadFromStr(zFSM, str);

            InitParent();
        }

        void InitParent()
        {
            foreach (var s in zFSM.StatusList)
            {
                var ps = (s as ZPropStatus);
                if (ps == null)
                {
                    throw new Exception("Status type is error");
                    //break;
                }

                ps.parent = this;
            }
        }


        public void Start() {
            if (zFSM == null)
                throw new Exception("FSM not set");

            zFSM.CurStatusID = Value;

            Enter.OnNext(new TrackPack(Value));

            StartStatus = Value;
        }

        public void Stop()
        {
            zFSM.Reset(StartStatus);
        }

        public void SendEvent(E e) {
            zFSM.SendEvent(e);
        }

        //public void SendEvent(E e, object p)
        //{
        //    zFSM.SendEvent(new ParamableEvent<E, object>(e, p));
        //}
    }


    public class ZStatusWithParamProperty<S, E> : ZStatusProperty<S, ParamableEvent<E, object>>
        where S : IComparable
        where E : IComparable
    {
        public void SendEvent(E e, object p)
        {
            zFSM.SendEvent(new ParamableEvent<E, object>(e, p));
        }

        public IObservable<TParam> EnterStatusWithParamObservable<TParam>(S targetS)           
        {
            return Enter.Where(p => p.to.CompareTo(targetS) == 0 && IsTParam(p, typeof(TParam))).
                Select(p => (TParam)(p.e.Param));
        }

        public IObservable<object> EnterStatusWithParamObservable(S targetS)
        {
            return Enter.Where(p => p.to.CompareTo(targetS) == 0).
                Select(p => p.e.Param);
        }

        private bool IsTParam(TrackPack t, Type paramType)
        {
            if (paramType == typeof(ZNull) || paramType == typeof(Unit))
            {
                return t.e.Param != null && t.e.Param.GetType() == typeof(ZNull);
            }

            return t.e.Param != null && ( t.e.Param.GetType() == paramType || paramType.IsAssignableFrom(t.e.Param.GetType()));
        }
    }
}


#endif