using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Core.Domain
{
    public interface IDirectLinkable
    {
        object Parent { get; }

        void InitParent(object obj);
    }
}
