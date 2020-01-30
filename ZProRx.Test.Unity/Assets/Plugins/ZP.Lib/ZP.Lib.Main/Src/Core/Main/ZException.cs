using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Common;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.Core.Main
{
    public class ZException : Exception
    {
        public virtual string ErrorStr { get; set; }

        public ZException()
        {
            //ErrorStr = errorStr;
        }


        public ZException(string errorStr)
        {
            ErrorStr = errorStr;
        }

        public override string ToString()
        {
            return ErrorStr; 
        }
    }

    public class ZProException : ZException<ZExceptionEnum> {
        public ZProException(ZExceptionEnum error) : base(error)
        {
        }
    }

    public class ZException<T> : ZException
    {
        virtual public T Error { set; get; }

        public ZException(T error)
        {
            Error = error;
        }

        public override string ErrorStr
        {
            get  {
               return  Error.ToString();
            }
            set
            {
                if (ZPropertyMesh.IsMultiEnum(typeof(T)))
                {
                    this.Error =MultiEnum<ZExceptionEnum, T>.ParseStr(value);
                }
                else if (typeof(T).IsEnum)
                {
                    this.Error = (T)Enum.Parse(typeof(T), value);
                }
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
