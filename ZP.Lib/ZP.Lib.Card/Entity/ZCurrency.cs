using System;
using ZP.Lib;

namespace ZP.Lib.Card.Entity
{
    public class ZCurrency
    {
        protected ZProperty<string> currencyType = new ZProperty<string>();

        protected ZProperty<float> value = new ZProperty<float>();

        public string CurrentTypeStr => currencyType.Value;

        public float Value
        {
            get => value.Value;
            set => this.value.Value = value;
        }

        public ZCurrency()
        {
        }

        public ZCurrency(string currentType)
        {
            this.currencyType.Value = currentType;
        }

        static public ZCurrency Create(string currentType, float value)
        {
            var ret = ZPropertyMesh.CreateObjectWithParam<ZCurrency>(currentType);
            ret.Value = value;

            return ret;
        }
    }

    public class ZCurrency<TType> : ZCurrency //where TType: System.Enum
    {
        public TType CurrentType => (TType)Enum.Parse(typeof(TType), currencyType.Value, true);

        public ZCurrency(TType t) : base(t.ToString())
        {
            
        }

        //convert ZCurrency 2 ZCurrency<TType> 
        static public ZCurrency<TType> Convert(ZCurrency cur)
        {
            var t = (TType)Enum.Parse(typeof(TType), cur.CurrentTypeStr, true);
            var ret = ZPropertyMesh.CreateObjectWithParam<ZCurrency<TType>>(t);
            ret.Value = cur.Value;

            return ret;
        }

    }
}
