using System.Collections;
using System.Collections.Generic;
//[Version:0.81]
namespace ZP.Lib.Common
{
    public class Transfer<S>
    {
        public delegate bool CanTransferCheckDelegate();

        public S Target;
        public CanTransferCheckDelegate Check;

        public Transfer(S t, CanTransferCheckDelegate check)
        {
            this.Target = t;
            this.Check = check;
        }
    }

    public class Status<S, E>
    {
        public delegate void StatusEnterDelegate(S from, S to, E e);
        public delegate void StatusLeaveDelegate();

        public StatusEnterDelegate OnEnterEvent;
        public StatusLeaveDelegate OnLeaveEvent;

        public S ID;

        public Dictionary<E, Transfer<S>> TransferMap = new Dictionary<E, Transfer<S>>();

        public virtual void OnEnter(S from, S to, E e)
        {

        }
        public virtual void OnLeave()
        {

        }
    }
}
