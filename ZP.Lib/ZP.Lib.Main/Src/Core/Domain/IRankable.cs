using System;
using System.Collections.Generic;

namespace ZP.Lib
{
    public interface IRankable
    {
        object GetRank(int rank);
        void AddRank(object rankData);
        object Upgrade(int rank);
        object ConvertToArray();
    }

    public interface IRankable<T> : IRankable
    {
        new T GetRank(int rank);
        new T Upgrade(int rank);
        new IList<T> ConvertToArray();
    }
}
