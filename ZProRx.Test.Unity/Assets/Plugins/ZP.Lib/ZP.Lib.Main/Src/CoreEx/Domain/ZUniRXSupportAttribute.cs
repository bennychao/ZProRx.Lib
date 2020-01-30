using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    //not used
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyTimerAttribute : Attribute
    {
        public float Interval = 0;
        public float Min = 0;
        public float Max = 1;
        public float Offset = 1;
        public PropertyTimerAttribute(float interval, float offset = 1, float max = 1, float min = 0)
           
        {
            this.Interval = interval;
            this.Max = max;
            this.Min = min;
            this.Offset = offset;
        }
    }
}
