using ZP.Lib.Common;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Net;
using ZP.Lib.Web;
using ZP.Lib;

namespace ZP.Lib.Web
{
    public static partial class ZPropertyNetCore
    {
        static public ZsonResult ZsonErrorResult<TErrorEnum>(TErrorEnum error, string msg)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, TErrorEnum>>();
            ret.Error = error;
            ret.Msg = msg;

            return new ZsonResult(ret);
        }

        static public ZsonResult ZsonErrorResult<TErrorEnum>(TErrorEnum error)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, TErrorEnum>>();
            ret.Error = error;
            //ret.Msg = msg;

            return new ZsonResult(ret);
        }

        static public ZsonResult ZsonErrorResult(ZNetErrorEnum error, string msg)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            ret.Error = error;
            ret.Msg = msg;

            return new ZsonResult(ret);
        }

        static public ZsonResult ZsonErrorResult(ZNetErrorEnum error)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            ret.Error = error;
            ret.Msg = EnumStrTools.GetStr(error);

            return new ZsonResult(ret);
        }

        static public ZsonResult ZsonErrorResult(string error)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            ret.Error = ZNetErrorEnum.CustomError;
            ret.Msg = EnumStrTools.GetStr(error);

            return new ZsonResult(ret);
            
        }

        static public ZsonResult ZsonOkResult<T>(T data)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            //ret.Error = default(T);
            ret.Data = data;
            return new ZsonResult(ret);
        }

        static public ZsonResult ZsonOkListResult<T>(IZPropertyList<T> data)
        {
            var ret = ZPropertyMesh.CreateObject<NetListResponce<T, ZNetErrorEnum>>();
            ret.AddRange(data);
            return new ZsonResult(ret);
        }

        static public ZsonResult ZsonOkResult()
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<ZNull, ZNetErrorEnum>>();
            //ret.Data = data;
            return new ZsonResult(ret);
        }
    }
}
