using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Net;

namespace ZP.Lib.CoreEx.Domain
{

    public interface INetResponsable
    {
        void SetError(ZNetErrorEnum error);
        void SetError(Exception e);

        void SetRawResult(string result);
    }
    public interface INetResponsable<TResult> : INetResponsable
    {
        void SetResult(TResult result);
        
    }

    public interface INetResponsable<TResult, TError> : INetResponsable<TResult>
    {
        //void SetResult(TResult result);
        void SetError(TError error);
    }

    public interface INetResponsableWithClientId<TResult>
    {
        void SetResult(string clientId, TResult result);
        void SetError(string clientId, ZNetErrorEnum error);
        void SetError(string clientId, Exception e);
    }
}
