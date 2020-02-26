using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Card.Domain
{

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CardAssetConfigAttribute : Attribute
    {
        public string ConfigPath { get; private set; }
        public string AssetPath { get; private set; }
        public CardAssetConfigAttribute(string configPath, string assetPath = "")
        {
            this.ConfigPath = configPath;
            this.AssetPath = assetPath;
        }
    }
}
