using System;
using System.Collections;
using System.Collections.Generic;



namespace ZP.Lib
{
    [PropertyValueChangeAnchorClass(".Max", ".Cur")]
    public class ZIntBar : ICalculable<ZIntBar>
    {
        public ZProperty<int> Max = new ZProperty<int>();
        public ZProperty<int> Cur = new ZProperty<int>();

        //view
        public ZRuntimableProperty<float> Value = new ZRuntimableProperty<float>();

        /// <summary>
        /// Raises the create event.
        /// </summary>
        public void OnCreate()
        {
            Value.Select<ZProperty<int>, ZProperty<int>>(Cur, Max, (cur, ma) => {

                return GetValue();
            });
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        public float GetValue()
        {
            if (Max?.Value == null || Max.Value == 0)
                return 0;

            return (float) (Cur.Value) / Max.Value;

        }

        public IZProperty GetCurProp()
        {
            return ZPropertyMesh.GetProperty(this, "ZIntBar.Cur");
        }

        public IZProperty GetValueProp()
        {
            return ZPropertyMesh.GetProperty(this, "ZIntBar.Value");
        }

        /// <param name="d">D.</param>
        public static implicit operator int(ZIntBar d)  // implicit digit to byte conversion operator
        {
            return d.Cur.Value;  // implicit conversion
        }

        /// <summary>
        /// Returns a <see cref="!:System.String"/> that represents the current <see cref="T:ZP.Lib.ZIntBar"/>.
        /// </summary>
        /// <returns>A <see cref="!:System.String"/> that represents the current <see cref="T:ZP.Lib.ZIntBar"/>.</returns>
        public override string ToString()
        {
            return Cur.Value.ToString() + "/" + Max.Value.ToString();
            
        }

        public ZIntBar Add(ZIntBar b)
        {
            Max.Value += b.Max;
            Cur.Value += b.Cur;
            return this;
        }

        public ZIntBar Sub(ZIntBar b)
        {
            Max.Value -= b.Max;
            Cur.Value -= b.Cur;
            return this;
        }
        public ZIntBar Add(int b)
        {
            //Max.Value += b.Max;
            Cur.Value += b;
            return this;
        }

        public ZIntBar Sub(int b)
        {
            //Max.Value -= b.Max;
            Cur.Value -= b;
            return this;
        }
    }
}