using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace ZP.Lib
{
    //T is Enum
    public class ZFightCount<T> //where T : System.Enum; C4.0 not support
    {
        public ZProperty<T> Type = new ZProperty<T>();

        public ZProperty<int> TotalCount = new ZProperty<int>();

        public ZProperty<int> WinCount = new ZProperty<int>();

        public ZProperty<float> WinRate = new ZProperty<float>();

        public void OnCreate()
        {
            WinRate.Select<ZProperty<int>, ZProperty<int>>(TotalCount, WinCount, (t, w) =>
            {
                return (float)(WinCount.Value) / TotalCount.Value;
            });
        }
    }

    public class ZFightStatistics<T> : ZFightCount<T>
    {
        public ZPropertyList<ZFightCount<T>> Items = new ZPropertyList<ZFightCount<T>>();

        public new void OnCreate()
        {
            TotalCount.Select<ZPropertyList<ZFightCount<T>>>(Items, (its) => GetTotalCount());
            WinCount.Select<ZPropertyList<ZFightCount<T>>>(Items, (its) => GetWinCount());
            WinRate.Select<ZPropertyList<ZFightCount<T>>>(Items, (its) => GetWinRate());
        }

        public int GetTotalCount()
        {
            return Items.Select((item) => item.TotalCount.Value).Sum();
        }

        public int GetWinCount()
        {
            return Items.Select((item) => item.WinCount.Value).Sum();
        }

        public float GetWinRate()
        {
            return Items.Select((item) => item.WinRate.Value).Sum() / Items.Count;
        }

        public void TickTotalCount(T type)
        {
            var v = Items.FindValue(a => a.Type.Value.ToString().Contains(type.ToString()) );

            if (v != null)
                v.TotalCount.Value++;
        }

        public void TickWinCount(T type)
        {
            var v = Items.FindValue(a => a.Type.Value.ToString().Contains(type.ToString()));

            if (v != null)
                v.WinCount.Value++;
        }
    }

}

