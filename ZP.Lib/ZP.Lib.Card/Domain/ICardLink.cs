using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Card.Domain
{
    public interface ICardLink
    {
        uint CardId { get; }

        int CardCount { get; set; }
    }

    public interface ICardLink<TCard> : ICardLink
    {
        TCard Card { get; }
    }
}
