using System;
using ZP.Lib.Common;
using ZP.Lib.Core.Relation;
#if ZP_UNIRX

namespace ZP.Lib
{

    public class ZFSM<S, E> : FSM<S, E> , IZEventAction where S : IComparable
    {
        public interface IFsmTrigger
        {
            E Event { get; }
        }


        public class Trigger : ZLinkEvent, IFsmTrigger
        {
            E IFsmTrigger.Event => CurEvent;

            public E CurEvent;
            public Trigger(E e)
            {
                this.CurEvent = e;
            }            

            public override void Invoke()
            {
                base.Invoke();

                var fsm = LinkProperty?.Value as ZFSM<S, E>;
                fsm?.SendEvent(CurEvent);
            }
        }

        public class Trigger<T> : ZLinkEvent<T>, IFsmTrigger
        {
            E IFsmTrigger.Event => CurEvent;

            public E CurEvent;
            public Trigger(E e)
            {
                this.CurEvent = e;
            }

            public override void Invoke(T data)
            {
                base.Invoke(data);

                var fsm = LinkProperty?.Value as ZFSM<S, E>;
                fsm?.SendEvent(CurEvent);
            }
        }

        public class DirectTrigger : ZDirectEvent, IFsmTrigger
        {
            public E Event => CurEvent;

            public E CurEvent;
            public DirectTrigger(E e)
            {
                this.CurEvent = e;
            }

            public override void Invoke()
            {
                base.Invoke();

                var fsm = parentObj as ZFSM<S, E>;
                fsm?.SendEvent(CurEvent);
            }
        }

        public class DirectTrigger<T> : ZDirectEvent<T>, IFsmTrigger
        {
            E IFsmTrigger.Event => CurEvent;

            public E CurEvent;
            public DirectTrigger(E e)
            {
                this.CurEvent = e;
            }

            public override void Invoke(T data)
            {
                base.Invoke(data);

                var fsm = parentObj as ZFSM<S, E>;
                fsm?.SendEvent(CurEvent);
            }
        }

        public class Transfer 
        {
            public ZProperty<S> From = new ZProperty<S>();
            public ZProperty<S> To = new ZProperty<S>();
            public ZProperty<E> Event = new ZProperty<E>();
        }

        public ZPropertyList<Transfer> Transfers = new ZPropertyList<Transfer>();

        public void OnCreate()
        {
            StatusType = typeof(ZStatusProperty<S, E>.ZPropStatus);
        }

        public void OnLoad()
        {
            //init the Fsm
            foreach (var t in Transfers)
            {
                AddTransfer(t.From.Value, t.To, t.Event);
            }
        }

        // public static ZFSM<S, E> CreateFSM()
        // {
        //    var  zFSM = ZPropertyMesh.CreateObject<ZFSM<S, E>>();

        //     zFSM.StatusType = typeof(ZStatusProperty<S, E>.ZPropStatus);

        //     return zFSM;
        // }

        public void OnEvent(IZEvent e)
        {
            //e.AttributeNode.Ge
            SendEvent(((e as IFsmTrigger).Event));

        }
    }
}
#endif