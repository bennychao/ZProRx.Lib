using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Matrix.Domain
{
    public interface IActionFillter
    {
        object OnFilter(object obj);
    }

    public interface IActionFillter<T>
    {
        SocketPackageHub<T> OnFilter(SocketPackageHub<T> obj);
    }

    //public interface IPackActionFillter<T> : IActionFillter<SocketResponseHub<T>>
    //{
    //    void OnAction(SocketResponseHub<T> obj);
    //}
}
