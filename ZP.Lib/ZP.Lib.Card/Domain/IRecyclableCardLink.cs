using System;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Card.Domain
{
    public interface IRecyclableCardLink
    {
        IRecyclable CurRankRecycle { get; }
    }
}
