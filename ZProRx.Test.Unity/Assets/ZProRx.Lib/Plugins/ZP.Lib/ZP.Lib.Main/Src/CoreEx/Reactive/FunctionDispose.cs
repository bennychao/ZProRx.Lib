using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.CoreEx
{
    public class FunctionDispose : IDisposable
    {
        Action action;
        public FunctionDispose(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            if (this.action != null)
            this.action();
        }
    }

    //public class FunctionDispose<T> : IDisposable
    //{
    //    Action<T> action;
    //    public FunctionDispose(Action<T> action)
    //    {
    //        this.action = action;
    //    }

    //    public void Dispose()
    //    {
    //        if (this.action != null)
    //            this.action();
    //    }
    //}
}
