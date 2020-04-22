using System;
using System.Net;
using ZP.Lib.Common;
using ZP.Lib.Core.Main;

namespace ZP.Lib.Net
{ 
    //ZP Net Exception for ZPropertySocket and ZPropertyNet, throw by Server commonly
    public class ZNetException<T> : ZException<T>
    {
        public ZNetException(T error) :base(error)
        {
            Error = error;
        }

        override public T Error { set; get; }

        //TODO support attribute
        public string ErrorMsg
        {
            get => Error.ToString();
        }

        public override string Message => ErrorMsg;

        public override string ToString()
        {
            return ErrorMsg;
        }

    }

    public class ZNetException : ZNetException<ZNetErrorEnum>
    {
        public ZNetException(ZNetErrorEnum error) : base(error)
        {
        }
    }

    public class ZNetMultiException<TErrorEnum> : ZNetException<MultiEnum< ZNetErrorEnum, TErrorEnum>>
    {
        public ZNetMultiException(ZNetErrorEnum error) : base(error)
        {
        }

        public ZNetMultiException(TErrorEnum error) : base(error)
        {
        }
    }

    public class ZNetHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ZNetHttpException(HttpStatusCode code)
        {
            this.StatusCode = code;
        }

        public override string ToString()
        {
            return StatusCode.ToString();
        }
    }


}
 