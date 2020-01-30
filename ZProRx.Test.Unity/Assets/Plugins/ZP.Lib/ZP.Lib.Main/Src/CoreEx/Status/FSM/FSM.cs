using System;
using System.Collections;
using System.Collections.Generic;
using ZP.Lib;

//[Version:0.82]
namespace ZP.Lib.Common
{
    public class FSM<S, E>   where S : IComparable
    {
        public S CurStatusID;

        public List<Status<S, E>> StatusList = new List<Status<S, E>>();

        public System.Type StatusType = typeof(Status<S, E>);

        public void AddTransfer(S from, S to, E e, Transfer<S>.CanTransferCheckDelegate check = null)
        {
            var s = this.FindStatus(from);
            if (s == null)
            {
                s = Activator.CreateInstance(StatusType) as Status<S, E>;
                s.ID = from;
                this.StatusList.Add(s);
            }

            var t = this.FindStatus(to);

            if (t == null)
            {
                t = Activator.CreateInstance(StatusType) as Status<S, E>;
                t.ID = to;
                this.StatusList.Add(t);
            }

            s.TransferMap[e] = new Transfer<S>(t.ID, check);


        }

        public void SendEvent(E e)
        {

            Status<S, E> curStatus = FindStatus(CurStatusID);
            if (curStatus != null)
            {

                if (curStatus.TransferMap.ContainsKey(e))
                {

                    if (curStatus.TransferMap[e].Check != null && !curStatus.TransferMap[e].Check())
                        return;

                    S nextID = curStatus.TransferMap[e].Target;

                    curStatus.OnLeave();

                    Status<S, E> nextStatus = FindStatus(nextID);
                    if (nextStatus != null)
                    {
                        nextStatus.OnEnter(CurStatusID, nextID, e);
                    }
                    //switch status
                    CurStatusID = nextID;
                }
            }
        }

        public void Reset(S resetStatus)
        {
            Status<S, E> curStatus = FindStatus(CurStatusID);
            if (curStatus != null)
            {
                curStatus.OnLeave();

                CurStatusID = resetStatus;
            }
        }

        public Status<S, E> FindStatus(S id)
        {
            Status<S, E> curStatus = StatusList.Find((a) =>
            {
                return a.ID.CompareTo(id) == 0;
            });

            return curStatus;
        }


        public Transfer<S> FindTrans(S from, E e)
        {
            var s = FindStatus(from);

            return s?.TransferMap[e];
        }


    }
}
