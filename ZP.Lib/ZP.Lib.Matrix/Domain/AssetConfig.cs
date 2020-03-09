using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Domain
{
    public class AssetScene
    {
        public string Name { get; set; }
    }

    public class AssetConfig
    {
        public List<AssetScene> Scenes { get; set; }

        public string AssetPath { get; set; } = "./";

        public string AppName { get; set; } = "ZProApp";

        public string MainScene => Scenes != null ? Scenes[0]?.Name : "";
    }
}
