using System;
using System.Collections;
using System.Collections.Generic;
//[Version:0.81]
namespace ZP.Lib.Common
{
    public class Transfer<S>
    {
        public delegate bool CanTransferCheckDelegate(object obj);

        public S Target;
        public CanTransferCheckDelegate Check;

        public Transfer(S t, CanTransferCheckDelegate check)
        {
            this.Target = t;
            this.Check = check;
        }
    }

    public struct ParamableEvent<E, TParam> : IComparable, IMultiEnumerable
        where E : IComparable
    {
        public E Event;
        public TParam Param;

        public uint UintValue => (uint)(object)Event;

        public ParamableEvent(E e, TParam p)
        {
            this.Param = p;
            this.Event = e;
        }

        public ParamableEvent(E e)
        {
            this.Param = default(TParam);
            this.Event = e;
        }

        //public ParamableEvent(string strEvent)
        //{
        //    this.Param = default(TParam);
        //    //this.Event = (ParamableEvent<E, TParam>)(strEvent);
        //}

        public static explicit operator ParamableEvent<E, TParam>(string strValue)
        {
            if (typeof(E).IsEnum)
            {
                var v = (E)Enum.Parse(typeof(E), strValue, true);
                return new ParamableEvent<E, TParam>(v);
            }
            else
            {
                return new ParamableEvent<E, TParam>((E)(object)strValue);
            }
            //return default(ParamableEvent<E, TParam>);
        }



        public int CompareTo(object obj)
        {
            return ((ParamableEvent<E, TParam>)(obj)).Event?.CompareTo(Event) ?? -1;
        }

        public object Parse(string str)
        {
            if (typeof(E).IsEnum)
            {
                var v = (E)Enum.Parse(typeof(E), str, true);
                return new ParamableEvent<E, TParam>(v);
            }
            else if (ZPropertyMesh.IsMultiEnum(typeof(E)))
            {
                var ret = new ParamableEvent<E, TParam>();

                ret.Event =(E)( (ret.Event as IMultiEnumerable).Parse(str));

                return ret;
            }
            return null;
        }

        //public static implicit operator ParamableEvent<E, TParam>(string strValue)  // implicit digit to byte conversion operator
        //{
        //    if (typeof(E).IsEnum)
        //    {
        //        var v = (E)Enum.Parse(typeof(E), strValue, true);
        //        return new ParamableEvent<E, TParam>(v);
        //    }
        //    else
        //    {
        //        return new ParamableEvent<E, TParam>((E)(object)strValue);
        //    }
        //    return null;
        //}
    }

    public class Status<S, E>
    {
        public delegate void StatusEnterDelegate(S from, S to, E e);
        public delegate void StatusLeaveDelegate();

        public StatusEnterDelegate OnEnterEvent;
        public StatusLeaveDelegate OnLeaveEvent;

        public S ID;

        public List<KeyValuePair< E, Transfer<S>>> TransferMap = new List<KeyValuePair<E, Transfer<S>>>();

        public virtual void OnEnter(S from, S to, E e)
        {

        }
        public virtual void OnLeave()
        {

        }
    }
}
