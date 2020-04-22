using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.CoreEx;
using ZP.Lib.Core.Relation;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Unity.RTComponents;

namespace ZP.Lib.Core.Values
{
    public interface IZHint
    {
        int Mid { get; }
        float Timer { set; get; }

        ZTransform3 Transfrom { get; set; }
    }


    [RTTransformClass(".transform")] //add RTTransfrom
    [PropertyUIItemResClass("[APP]/Hints/", "", ".modelName")]
    [AddTriggerComponentClass(typeof(RTHoldable), ".OnHold")]
    public class ZHint : IZHint, IIndexable
    {
        //optional
        protected ZProperty<IZMsg> msgRef = new ZProperty<IZMsg>();

        protected ZProperty<string> modelName = new ZProperty<string>("Hint");

        protected ZProperty<ZTransform3> transform = new ZProperty<ZTransform3>();

        public ZTransform3 Transfrom {
            get => transform.Value;
            set => transform.Value = value;
        }

        public ZEvent<bool> OnHold = new ZEvent<bool>();

        private int msgID;

        protected IDisposable disposable = null;

        public int Mid
        {
            get
            {
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
        } = 3; //set default

        static public ZHint Create()
        {
            return ZPropertyMesh.CreateObject<ZHint>();
        }

        static public ZHint Create(string msg, string desc)
        {
            var ret =  ZPropertyMesh.CreateObject<ZHint>();
            var msgObj = ZPropertyMesh.CreateObject<ZMsg>();
            msgObj.Title.Value = msg;

            msgObj.Description.Value = desc;

            ret.msgRef.Value = msgObj;

            //ret.msgRef.Value

            return ret;
        }

        void OnCreate()
        {
            if (msgRef.Value != null)
            {
                msgRef.ActiveNode(false);

                disposable = OnHold.OnEventObservable().Subscribe(bhold =>
                {
                    msgRef.ActiveNode(bhold);
                });
            }
        }

        void OnDestroy()
        {
            disposable?.Dispose();
        }
    }

    [PropertyAddComponentClass(typeof(RTPropertyList))]
    public class ZHintList : ZPropertyRefList<IZHint>
    {
        private List<KeyValuePair<int, IDisposable>> Timers = new List<KeyValuePair<int, IDisposable>>();

        private object lockObj = new object();
        //add the timer
        public void AddTimer(IZHint msg)
        {
            var inmsg = msg as IIndexable;
            if (inmsg == null)
                return;

            var curScheduler = new ZPRxScheduler();

            var t = Observable.Timer(new TimeSpan(0, 0, (int)msg.Timer)).ObserveOn(curScheduler).Subscribe(_ => {
                //var runId = MatrixRuntimeTools.GetRunId();
                lock(lockObj)
                {
                    base.Remove(a => a.Mid == inmsg.Index);
                    RemoveFromTimer(inmsg.Index);
                }
            });

            lock(lockObj)
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
