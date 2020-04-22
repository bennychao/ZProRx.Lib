using System;
using ZP.Lib;

namespace ZP.Lib.Core.Values
{
    [PropertyValueChangeAnchorClass(".min", ".max")]
    public class ZRange<T> where T : IComparable //where T : System.ValueType 
    {
        protected ZProperty<T> min = new ZProperty<T>();
        protected ZProperty<T> max = new ZProperty<T>();

        public bool IsInRange(T cur)
        {
            return true;
        }
    }
}
