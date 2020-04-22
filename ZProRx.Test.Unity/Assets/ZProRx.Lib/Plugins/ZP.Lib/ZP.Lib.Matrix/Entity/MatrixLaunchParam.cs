using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Soc.Entity
{
    public class MatrixLaunchParam
    {
        public string WorkerParam { get; set; }
        public int Port { get; set; }
        public string UnitType { get; set; }

        public int Count { get; set; }

        public bool IsPrivateClub { get; set; } = false;
    }
}
