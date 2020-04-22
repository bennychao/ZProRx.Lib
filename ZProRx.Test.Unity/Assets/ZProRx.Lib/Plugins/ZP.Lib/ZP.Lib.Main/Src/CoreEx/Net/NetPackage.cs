using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Common;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Net
{
    // public class NetPackageHeader
    // {
    // }

    // ZProRx Net Package
    public class NetPackage<T, TErrorEnum> // where TErrorEnum : System.Enum
    {
        private ZProperty<T> data = new ZProperty<T>();
        private ZProperty<TErrorEnum> status = new ZProperty<TErrorEnum>();
        private ZProperty<string> msg = new ZProperty<string>();

        //headers
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        public bool IsResponceOk()
        {
            if (ZPropertyMesh.IsMultiEnum(status))
            {
                return (status.Value as IMultiEnumerable)?.UintValue == 0;
            }
            else if (status.Value.GetType().IsEnum)
            {
                return (int)(object)(status.Value) == 0;
            }
            else if (status.Value.GetType() == typeof(string)) //support string error for common Error Info. need client to convert to Enum
            {
                return string.Compare(status.Value.ToString(), "NoError", StringComparison.OrdinalIgnoreCase) == 0;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, string> Headers => headers;

        public TErrorEnum Error
        {
            get => (TErrorEnum)(status.Value); // Enum.Parse(TErrorEnum.GetType())
            set => status.Value = value;
        }

        public string Msg
        {
            get => msg.Value;
            set => msg.Value = value;
        }

        public T Data
        {
            get => data.Value;
            set => data.Value = value;
        }

        public static (T data, TErrorEnum error) Parse(ISocketPackage socketPackage)
        {
            return Parse(socketPackage?.Value);
        }

        public static (T data, TErrorEnum error) Parse(string strPack)
        {
            var packOjb = ZPropertyMesh.CreateObject<NetPackage<T, TErrorEnum>>();

            ZPropertyPrefs.LoadFromStr(packOjb, strPack);
            if (packOjb.IsResponceOk())
            {
                return (packOjb.data, packOjb.Error);
            }
            else
            {
                return (default(T), packOjb.Error);
            }
        }

    }

    // ZProRx Net Package Array
    public class NetListResponce<T, TErrorEnum> // where TErrorEnum : System.Enum
    {
        private ZPropertyList<T> data = new ZPropertyList<T>();
        private ZProperty<TErrorEnum> status = new ZProperty<TErrorEnum>();
        private ZProperty<string> msg = new ZProperty<string>();

        public bool IsResponceOk()
        {
            return (int)(object)(status.Value) == 0;
        }

        public TErrorEnum Error
        {
            get => (TErrorEnum)(status.Value); // Enum.Parse(TErrorEnum.GetType())
            set => status.Value = value;
        }

        public string Msg
        {
            get => msg.Value;
            set => msg.Value = value;
        }

        public void AddRange(IZPropertyList<T> list)
        {
            data.AddRange(list);
        }
    }


    //internal class ErrorResponceTErrorEnum> // where TErrorEnum : System.Enum
    //{
    //    private ZProperty<T> data = new ZProperty<T>();
    //    private ZProperty<TErrorEnum> status = new ZProperty<TErrorEnum>();
    //    private ZProperty<string> msg = new ZProperty<string>();

    //    public bool IsResponceOk()
    //    {
    //        return (int)(object)(status.Value) == 0;
    //    }

    //    public TErrorEnum Error => (TErrorEnum)(status.Value); // Enum.Parse(TErrorEnum.GetType())
    //}
}
