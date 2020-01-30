using System;
namespace ZP.Lib
{
    public interface IUpgradable
    {
        object GetRank(int rank);
        void AddRank(object rankData);
        void Upgrade(int rank);
        object ToMap();
    }
}
