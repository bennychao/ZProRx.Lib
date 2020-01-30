//#if ZP_UNITY_CLIENT

using System;
using System.Collections.Generic;
using UniRx;
using ZP.Lib.Core.Relation;

namespace ZP.Lib
{
    public interface IZMsg
    {
        int Mid { get; }
        float Timer { set; get; }
    }

    [PropertyUIItemResClass("[APP]/Msgs/Msg")]
    //for hint msg UI
    public class ZMsg : IZMsg, IIndexable
    {
        private int msgID;

        [PropertyImageRes("Msgs/")]
        public ZProperty<string> Icon = new ZProperty<string>("");
        public ZProperty<string> Title = new ZProperty<string>("");
        public ZProperty<string> Description = new ZProperty<string>("");

        public int Mid {
            get {
                return msgID;
            }
        }

        public int Index
        {
            get
            {
                return msgID;
            }

            set
            {
                // throw new NotImplementedException();
                msgID = value;
            }
        }

        public float Timer
        {
            set; get;
        } = 3;

        public ZMsg()
        {
        }

        static public ZMsg Create(string msg, string desc)
        {
            var msgObj = ZPropertyMesh.CreateObject<ZMsg>();
            msgObj.Icon.Value = "msgIcon";
            msgObj.Title.Value = msg;

            msgObj.Description.Value = desc;

            return msgObj;
        }
    }

    //auto add component to support update 
    [PropertyAddComponentClass(typeof(ZUIPropertyListItem))]
    public class ZMsgList : ZPropertyRefList<IZMsg> {

        private List<KeyValuePair<int, IDisposable>> Timers = new List<KeyValuePair<int, IDisposable>>();
        private object lockObj = new object();
        //void OnBind(Transform trans) { 
        //    base.OnAddItem += (IZProperty obj) =>
        //    {
        //        var msg = obj.Value as ZMsg;

        //        if (msg != null)
        //        {

        //        }
        //    };
        //}


        //add the timer
        public void AddTimer(IZMsg msg) {
            var inmsg = msg as IIndexable;
            if (inmsg == null)
                return;

            var curScheduler = new ZPRxScheduler();

            var t = Observable.Timer(new TimeSpan(0, 0, (int)msg.Timer))
                .ObserveOn(curScheduler)
                .Subscribe( _=>{
                    lock (lockObj)
                    {
                        base.Remove(a => a.Mid == inmsg.Index);
                        RemoveFromTimer(inmsg.Index);
                    }
                });

            lock (lockObj)
            {
                Add(msg as IIndexable);
                Timers.Add(new KeyValuePair<int, IDisposable>(inmsg.Index, t));
            }
        }

        private void RemoveFromTimer(int index)
        {
            Timers.RemoveAll(a => a.Key == index);
        }
    }
}

//#endif