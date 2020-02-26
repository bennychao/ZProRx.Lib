using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Main.CommonTools
{
    public class InterCount
    {
        public int Count = 0;
    }

    public class InterCountReactiveProperty : ReactiveProperty<InterCount>
    {
        public InterCountReactiveProperty()
        {
            Value = new InterCount();
            Value.Count = 0;
        }

        public InterCountReactiveProperty(int count)
        {
            Value = new InterCount();
            Value.Count = count;
        }

        public int Increment()
        {
            Interlocked.Increment(ref Value.Count);
            SetValueAndForceNotify(Value);
            return Value.Count;
        }

        public int Decrement()
        {
            Interlocked.Decrement(ref Value.Count);
            SetValueAndForceNotify(Value);
            return Value.Count;
        }

        //public IObservable<int> WaitFor(Func<int, bool> comparer)
        //{
        //    return this.Select(cur => cur.Count).Where(cur => comparer(cur)).Fetch().Timeout(TimeSpan.FromSeconds(5));
        //}

        public IObservable<int> WaitFor(Func<int, bool> comparer, int waitSeconds = 5)
        {
            return this.Select(cur => cur.Count).Where(cur => comparer(cur)).Fetch().Timeout(TimeSpan.FromSeconds(waitSeconds));
        }
    }
}
