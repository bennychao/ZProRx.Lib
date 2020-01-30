using System.Collections;
using System.Collections.Generic;
using UniRx;
using ZP.Lib;
using System;
using ZP.Lib.Server.SQL;

#if ZP_UNIRX

namespace ZP.Lib
{

    public enum ZTaskStatus
    {
        Init = 0,
        Running,
        Paused,
        Stoped
    }

    public class ZTaskProperty<TId> : ZProperty<ZTask<TId>>// where T : System.Enum  
    { }

    public class ZTaskPropertyList<TId> : ZPropertyList<ZTask<TId>> //where T : System.Enum
    {
        MultiDisposable disposables = new MultiDisposable();
        public ZTaskPropertyList()
        {
            (this as IZPropertyList).AddItemAsObservable().Subscribe( (IZProperty obj) =>
            {
                var task = (obj?.Value as ZTask<TId>);
                if (task != null)
                    task.OnEndObservable.Subscribe( _=>
                    {
                        OnATaskEnd(task);
                    }).AddTo(disposables);
            }).AddTo(disposables);
        }


        void OnATaskEnd(ZTask<TId> task)
        {
            //task.End();
            //task.OnEnd = null; //remove all ref
            disposables?.Dispose();

            propList.RemoveAll((obj) => {
                bool b = (int)(object)(obj.Value as ZTask<TId>).Code.Value == (int)(object)task.Code.Value;
                return b;
            });
        }

        public void ClearTask(TId id)
        {
            Remove((obj) => (object)id == (object)obj.Code.Value );
        }
    }

    public class ZTask : ZTask<string> {

        new public static ZTask Create(string t, bool bAutoStart = false)
        {
            var n = ZPropertyMesh.CreateObject<ZTask>();
            n.CreateTime.Value = ZDateTime.Now();
            n.Code.Value = t;

            if (bAutoStart)
                n.Start();
            return n;
        }
    }

    [DBIndex("PlayerID")]
    public class ZTask<TId> : IZTaskable //where T : System.Enum
    {
        [DBIndex(true)] //primary index name is [tpid]
        public ZProperty<uint> Id = new ZProperty<uint>();

        public ZProperty<TId> Code = new ZProperty<TId>();

        public ZProperty<ZDateTime> CreateTime = new ZProperty<ZDateTime>();

        public ZProperty<float> Duration = new ZProperty<float>();

        public ZProperty<ZTaskStatus> Status = new ZProperty<ZTaskStatus>();


        public ZProperty<IRawDataPref> RawParam = new ZProperty<IRawDataPref>();

        IDisposable timer = null;

        private Action OnEnd = null;
        private Action<float> OnTick = null;

        public IObservable<Unit> OnEndObservable => Observable.FromEvent(h => OnEnd += h, h => OnEnd -= h);

        public IObservable<float> OnTickObservable => Observable.FromEvent<float>(h => OnTick += h, h => OnTick -= h);

        private ZProperty<float> lastSecond = new ZProperty<float>(0);

        public static ZTask<TId> Create(TId t, bool bAutoStart = false)
        {
            var n = ZPropertyMesh.CreateObject<ZTask<TId>>();
            n.CreateTime.Value = ZDateTime.Now();
            n.Code.Value = t;

            if (bAutoStart)
                n.Start();
            return n;
        }

        private void OnCreate()
        {
            this.CreateTime.Value = ZDateTime.Now();
        }

        //start or resume
        public void Start()
        {
            Cancel();

            Status.Value = ZTaskStatus.Running;

            var lastRunTime = (float)CreateTime.Value.Duration(ZDateTime.Now()).TotalSeconds;

            //if (Value.Duration <= lastRunTime)
            //{
            //    End();
            //    return this;
            //}

            //new TimeSpan(0, 0, (int)(Duration - lastRunTime))
            timer = UniRx.Observable.Timer(TimeSpan.FromSeconds(0.01f)).Repeat().Subscribe(_ => {
                innerTick();
            });

            //lastSecond.Value = lastRunTime;

            return;
        }

        public void Reset()
        {
            Cancel();
            this.CreateTime.Value = ZDateTime.Now();
            lastSecond.Value = 0;
        }

        public void Cancel()
        {
            //will start last timer
            if (timer != null)
            {
                Status.Value = ZTaskStatus.Stoped;
                //base.Value = 0;
                timer.Dispose();
                timer = null;
            }
        }

        public void Pause()
        {
            //will start last timer
            if (timer != null)
            {
                Status.Value = ZTaskStatus.Paused;
                //base.Value = 0;
                timer.Dispose();
                timer = null;
            }
        }

        public bool IsCompleted()
        {
            var curRunTime = (float)CreateTime.Value.Duration(ZDateTime.Now()).TotalSeconds;

            return (Duration <= curRunTime);
        }

        public void End()
        {
            if (timer != null)
            {
                //base.Value = 0;
                Status.Value = ZTaskStatus.Paused;
                timer.Dispose();
                timer = null;
            }

            if (OnEnd != null)
            {
                OnEnd();
            }

        }

        private void innerTick()
        {
            //break
            if (timer == null)
            {
                return;
            }

            var curRunTime = (float)CreateTime.Value.Duration(ZDateTime.Now()).TotalSeconds;
            //Debug.Log("curRunTime " + curRunTime.ToString());
            OnTick?.Invoke((curRunTime - lastSecond));


            if (Duration <= curRunTime)
            {
                End();
                return;
            }


            lastSecond.Value = curRunTime;

        }

        public float GetDegree()
        {
            return lastSecond.Value / Duration.Value;
        }

        public TT GetParam<TT>()
        {
            TT ret = ZPropertyMesh.CreateObject<TT>();

            if (ZPropertyMesh.IsPropertable(ret))
                ZPropertyPrefs.LoadFromRawData(ret, RawParam.Value);
            else
            {
                return (TT)ZPropertyPrefs.LoadValueFromRawData(typeof(TT), RawParam.Value);
            }

            return ret;
        }

        public void SetParam(object param)
        {
            if (ZPropertyMesh.IsPropertable(param))
            {
                RawParam.Value = ZPropertyPrefs.ConvertToRawData(param);
            }
            else
            {
                RawParam.Value = ZPropertyPrefs.ConvertToRawData(param);
            }
        }

        public void SetParam<TT>(TT t)
        {
            RawParam.Value = ZPropertyPrefs.ConvertToRawData(t);
        }

        public IObservable<float> TickObservable()
        {
            return Observable.FromEvent<Action<float>, float>(h => a => h(a), h => OnTick += h, removeHandler: h => OnTick -= h);
        }
        public IObservable<Unit> EndObservable()
        {
            return Observable.FromEvent<Action>(h => ()=> h(), h => OnEnd += h, h => OnEnd -= h);
        }
    }

}


#endif