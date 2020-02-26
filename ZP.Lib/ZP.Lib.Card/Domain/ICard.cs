using System;
namespace ZP.Lib.Card
{
    public interface ICard{
        //ZProperty<int> CardID{ set; get;}//= new ZProperty<int>();

        void Upgrade(int rank);
    }
}
