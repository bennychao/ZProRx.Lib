using System;
#if ZP_UNIRX
using UniRx;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.CoreEx.Status;

namespace ZP.Lib
{
    //ZStatusProperty need UniRX Support
    public class ZStatusProperty<S, E> : ZLinkProperty<S>, IStatusProperty<S> where S: IComparable
    {
        public S OldStatus;
        public E CurEvent;

        public S StartStatus;

        public Subject<S> Enter = new Subject<S>();
        public Subject<S> Leave = new Subject<S>();

        public IObservable<uint> EnterObservable => Enter.Select(s => (uint)Convert.ToInt32(s));

        public IObservable<uint> LeaveObservable => Leave.Select(s => (uint)Convert.ToInt32(s));

        public uint CurStatusValue => (uint)Convert.ToInt32(StartStatus);

        public S CurStatus => Value;

        private ZFSM<S, E> zFSM;// = new ZFSM<S, E>();



        public class ZPropStatus : ZP.Lib.Common.Status<S, E> {

            public ZStatusProperty<S, E> parent;
            public override void OnEnter(S from, S to, E e)
            {
                parent.Value = to;
                parent.OldStatus = from;
                parent.CurEvent = e;

                parent.Enter.OnNext(to);
            }

            public override void OnLeave()
            {
                parent.OldStatus = parent.zFSM.CurStatusID;

                parent.Leave.OnNext(parent.OldStatus);
            }
        }

        //public ZStatusProperty()
        //{
        //    //zFSM = ZPropertyMesh.CreateObject<ZFSM<S, E>>();

        //    //zFSM.StatusType = typeof(ZPropStatus);
        //}

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

            Enter.OnNext(Value);

            StartStatus = Value;
        }

        public void Stop()
        {
            zFSM.Reset(StartStatus);
        }

        public void SendEvent(E e) {
            zFSM.SendEvent(e);
        }

        
    }
}


#endif