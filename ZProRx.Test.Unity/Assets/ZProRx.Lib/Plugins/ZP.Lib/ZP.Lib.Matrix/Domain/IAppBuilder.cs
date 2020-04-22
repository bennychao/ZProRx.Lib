using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Domain
{
    public interface IAppBuilder
    {
        T GetService<T>();
        IServiceProvider ApplicationServices { get; }

        IServiceProvider BuildSubServiceProvider();
    }
}
