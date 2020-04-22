using System;
using ZP.Lib.Common;
using ZP.Lib.Core.Relation;
using ZP.Lib.Main;
using System.Collections.Generic;
#if ZP_UNIRX
using UniRx;

namespace ZP.Lib
{

    public interface IZFSM
    {
        void AddCondition(string name, Func<object, bool> check);
    }


    public class ZFSM<S, E> : FSM<S, E> , IZEventAction, IZFSM
        where S : IComparable
        where E : IComparable
    {
        public interface IFsmTrigger
        {
            E Event { get; }
        }
        public interface IFsmAction
        {
            S Status { get; }
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

        public class Action : ZLinkEvent, IFsmAction
        {
            S IFsmAction.Status => CurStatus;

            public S CurStatus;
            public Action(S s)
            {
                this.CurStatus = s;
            }

            public Action()
            {
                //this.CurStatus = s;
            }

            protected override void OnLinked()
            {              
                var fsmStatus = LinkProperty?.Value as ZLinkProperty<S>;

                var tagS = ZPropertyMesh.GetTag<S>(this, TagNameSpace.CoreFsmAction);// ?? this.CurStatus;

                if (tagS == null)
                    tagS = this.CurStatus;

                //fsmStatus?.SendEvent(CurEvent);
                fsmStatus.ValueChangeAsObservable().Subscribe(s =>
                {
                    if (s.CompareTo(tagS) == 0)
                    {
                        base.Invoke();
                    }
                });
            }
        }

        public class BoolAction : ZLinkEvent<bool>, IFsmAction
        {
            S IFsmAction.Status => CurStatus;

            public S CurStatus;
            public BoolAction(S s)
            {
                this.CurStatus = s;
            }

            public BoolAction()
            {
                //this.CurStatus = s;
            }

            protected override void OnLinked()
            {
                var fsmStatus = LinkProperty?.Value as ZStatusProperty<S, E>;

                var tagS = ZPropertyMesh.GetTag<S>(this, TagNameSpace.CoreFsmAction);
                if (tagS == null)
                    tagS = this.CurStatus;

                bool bEnter = false;
                //fsmStatus?.SendEvent(CurEvent);
                fsmStatus.ValueChangeAsObservable().Subscribe(s =>
                {
                    if (s.CompareTo(tagS) == 0)
                    {
                        bEnter = true;
                        base.Invoke(true);
                    }
                    else if (bEnter)
                    {
                        base.Invoke(false);
                    }
                });
            }
        }

        public class Condition
        {
            public string Name { get; internal set; }
            //public Func<object, bool> CheckFunc;
            public Transfer<S>.CanTransferCheckDelegate CheckFunc;
            public Transfer<S>.CanTransferCheckDelegate DoCheck => inDoCheck;
            bool inDoCheck(object o)
            {
                if (CheckFunc == null)
                    return false;

                return CheckFunc(o);
            }
        }

        //public class DirectAction : ZDirectEvent, IFsmAction
        //{
        //    public E Event => CurEvent;

        //    public E CurEvent;
        //    public DirectAction(E e)
        //    {
        //        this.CurEvent = e;
        //    }

        //    public void OnLinked()
        //    {
        //        var fsmStatus = LinkProperty?.Value as ZStatusProperty<S, E>;

        //        var tagS = ZPropertyMesh.GetTag<S>(this, TagNameSpace.CoreFsmAction);
        //        //fsmStatus?.SendEvent(CurEvent);
        //        fsmStatus.ValueChangeAsObservable().Subscribe(s =>
        //        {
        //            if (s.CompareTo(tagS) == 0)
        //            {
        //                base.Invoke();
        //            }
        //        });
        //    }
        //}

        //public class DirectBoolAction : ZDirectEvent<bool>, IFsmAction
        //{
        //    E IFsmAction.Event => CurEvent;

        //    public E CurEvent;
        //    public DirectBoolAction(E e)
        //    {
        //        this.CurEvent = e;
        //    }

        //    public override void Invoke(T data)
        //    {
        //        base.Invoke(data);

        //        var fsm = parentObj as ZFSM<S, E>;
        //        fsm?.SendEvent(CurEvent);
        //    }
        //}

        public class Transfer 
        {
            public ZProperty<S> From = new ZProperty<S>();
            public ZProperty<S> To = new ZProperty<S>();
            public ZProperty<E> Event = new ZProperty<E>();

            public ZProperty<string> Condition = new ZProperty<string>();
        }

        private List<Condition> conditions = new List<Condition>();

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
                var cn = AddCondition(t.Condition.Value);

                //Transfer<S>.CanTransferCheckDelegate check = cn?.DoCheck;
                AddTransfer(t.From.Value, t.To, t.Event, cn?.DoCheck);
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

        public Condition AddCondition(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var cn = conditions.Find(c => string.Compare(c.Name, name) == 0);
            if (cn == null)
            {
                cn = new Condition();
                conditions.Add(cn);
                cn.Name = name;
            }

            return cn;
        }

        public void AddCondition(string name, Func<object, bool> check)
        {
            var cn = conditions.Find(c => string.Compare(c.Name, name) == 0);
            if (cn == null)
            {
                cn = new Condition();
                conditions.Add(cn);
                cn.Name = name;
            }

            cn.CheckFunc = o=> check(o);
        }
    }
}
#endif