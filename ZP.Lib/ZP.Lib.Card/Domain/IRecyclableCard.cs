using System;
using ZP.Lib;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Card.Domain
{
    public interface IRecyclableCard
    {
        //ZRankableProperty<IRecyclable> Recycles { get; }
        IRecyclable GetRecycles(int rank);
    }
}
