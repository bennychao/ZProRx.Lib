using System;
using ZP.Lib;
using ZP.Lib.Server.SQL;

namespace ZP.Lib.Card.Entity
{
    [DBIndex("pid")]
    public class ZMaterial
    {

        private ZProperty<string> code = new ZProperty<string>();

        private ZProperty<int> count = new ZProperty<int>();

        public ZMaterial()
        {
            //this.code.Value = code;
        }

        public ZMaterial(string code)
        {
            this.code.Value = code;
        }

        public string CodeStr => code.Value;

        public int Count
        {
            get => count.Value;
            set => count.Value = value;
        }

        static public ZMaterial Create(string mcode, int count)
        {
            var ret = ZPropertyMesh.CreateObjectWithParam<ZMaterial>(mcode);
            ret.Count = count;

            return ret;
        }
    }

    public class ZMaterial<TType> : ZMaterial// where TType: System.Enum
    {
        public ZMaterial(TType t) : base(t.ToString())
        {

        }

        public TType CurrentType => (TType)Enum.Parse(typeof(TType), CodeStr, true);

        //convert ZCurrency 2 ZCurrency<TType> 
        static public ZMaterial<TType> Convert(ZMaterial cur)
        {
            var t = (TType)Enum.Parse(typeof(TType), cur.CodeStr, true);
            var ret = ZPropertyMesh.CreateObjectWithParam<ZMaterial<TType>>(t);
            ret.Count = cur.Count;

            return ret;
        }

    }
}
