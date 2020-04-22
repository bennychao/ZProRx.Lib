using System;
using ZP.Lib;

namespace ZP.Lib.Card.Entity
{
    public class CommonCard
    {
        public ZProperty<int> ID = new ZProperty<int>();
        public ZProperty<int> Rarity = new ZProperty<int>();
        public ZProperty<string> CardName = new ZProperty<string>();   

        
        /// <summary>
        /// Upgrade the specified rank.
        /// </summary>
        public void Upgrade(int rank)
        {

            var rankProps = ZPropertyMesh.GetPropertiesWithRankableInSubs(this);
            foreach (var p in rankProps)
            {
                p.Upgrade(rank);
                //float data = (float)p.Upgrade(rank);
                //Assert.IsTrue(data > 0, "rank data is error ");
            }
        }     
    }
}
