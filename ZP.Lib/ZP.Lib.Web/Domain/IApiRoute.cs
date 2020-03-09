using System;
namespace ZP.Lib.Web.Domain
{
    public interface IApiRoute
    {
        string Template { get; set; }
        MethodTypeEnum Method { get; set; }

        Type ReturnType { get;}

        void OnNext(object data);

        void OnError(Exception error);
    }
}
