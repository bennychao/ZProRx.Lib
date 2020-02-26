

using System;
using ZP.Lib.Common;

namespace ZP.Lib.Net
{
    static public class ZNetExceptionExtension
    {
        static public bool IsError<TErrorEnum>(this Exception exception, TErrorEnum error)
        {
            var e = (exception as ZNetException<TErrorEnum>);

            return e != null && ((int)(object)e.Error == (int)(object)error);
        }

        static public bool IsError(this Exception e, ZNetErrorEnum error)
        {
            ZNetErrorEnum convertError;
            return (e as ZNetException<ZNetErrorEnum>)?.Error == error ||
                (Enum.TryParse<ZNetErrorEnum>(e.ToString(), out convertError) && convertError == error);
        }

        static public bool IsErrorType<TErrorEnum>(this Exception exception)
        {
            var e = (exception as ZNetException<TErrorEnum>);

            return e != null;
        }

        //for check if custom Error Exceptions
        static public bool IsMultiError<TError>(this Exception e, TError error) 
            where TError : struct, IComparable
        {
            TError outResult = default(TError);
            return (e as ZNetException<MultiEnum<ZNetErrorEnum, TError>>)?.Error.CompareTo(error) == 0
                || (Enum.TryParse<TError>(e.ToString(), out outResult) && outResult.CompareTo(error) == 0);
        }

    }

}
