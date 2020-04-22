using System;
using ZP.Lib;
using ZP.Lib.Card.Domain;

namespace ZP.Lib.Card.Entity
{
    public class ZProduct : ICard
    {
        protected ZProperty<uint> productId = new ZProperty<uint>();

        //type
        protected ZProperty<string> productCode = new ZProperty<string>();

        protected ZProperty<string> productName = new ZProperty<string>();

        protected ZProperty<string> productDesc = new ZProperty<string>();

        //rank
        protected ZProperty<short> rarity = new ZProperty<short>();

        //link info
        protected ZProperty<ProductLinkTypeEnum> linkType = new ZProperty<ProductLinkTypeEnum>();
        protected ZProperty<short> linkId = new ZProperty<short>();
        protected ZProperty<string> linkSubType = new ZProperty<string>();
        protected ZProperty<float> linkValue = new ZProperty<float>();

        [PropertyImageRes(ImageResType.LocalRes, "Test/")]
        private ZProperty<string> image = new ZProperty<string>();

        private ZRankableProperty<ZPrice> prices = new ZRankableProperty<ZPrice>();

        // normal members
        public ZPrice Price => prices.GetRank(rarity.Value);

        public ProductLinkTypeEnum LinkType => linkType.Value;

        public string LinkTypeStr => linkSubType.Value;

        public short LinkId => linkId.Value;

        public float LinkValue => linkValue.Value;

        public ZProduct()
        {
        }

        public void Upgrade(int rank)
        {
            throw new NotImplementedException();
        }
    }
}
