namespace ZP.Lib
{
    [PropertyValueChangeAnchorClass(".Max", ".Cur")]
    public class ZDataBar : ICalculable<ZDataBar>
    {
        public ZProperty<float> Max = new ZProperty<float>();
        public ZProperty<float> Cur = new ZProperty<float>();

        public ZRuntimableProperty<float> Value = new ZRuntimableProperty<float>();

        /// <summary>
        /// Raises the create event.
        /// </summary>
        public void OnCreate()
        {
            Value.Select<ZProperty<float>, ZProperty<float>>(Cur, Max, (cur, ma) => {

                return GetValue();
            });
        }

        public float GetValue()
        {
            if (Max?.Value == null || Max.Value == 0)
                return 0;
            return Cur.Value/ Max.Value;

        }

        public IZProperty GetCurProp()
        {
            return ZPropertyMesh.GetProperty(this, "ZDataBar.Cur");
        }

        public IZProperty GetValueProp()
        {
            return ZPropertyMesh.GetProperty(this, "ZDataBar.Value");
        }

        /// <param name="d">D.</param>
        public static implicit operator float(ZDataBar d)  // implicit digit to byte conversion operator
        {
            return d.Cur.Value;  // implicit conversion
        }

        public override string ToString()
        {
            return Cur.Value.ToString() + "/" + Max.Value.ToString();
        }

        public ZDataBar Add(ZDataBar b)
        {
            Max.Value += b.Max;
            Cur.Value += b.Cur;
            return this;
        }

        public ZDataBar Sub(ZDataBar b)
        {
            Max.Value -= b.Max;
            Cur.Value -= b.Cur;
            return this;
        }
    }
}