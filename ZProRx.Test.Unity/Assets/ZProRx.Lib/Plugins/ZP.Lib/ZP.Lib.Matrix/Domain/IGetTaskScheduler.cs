using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.Matrix.Domain
{
    public interface IGetMatrixRuntime
    {
        IZMatrixRuntime Runtime { get; }

    }
}
