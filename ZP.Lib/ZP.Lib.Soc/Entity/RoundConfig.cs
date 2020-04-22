using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Soc.Entity
{
    public class RoundConfig
    {
        public int MaxRound { get; set; } = 100;

        public int RoundHandTimeout { get; set; } = 30000;      //ms

        public int HandInterval { get; set; } = 1000;     //ms

        public int RoundInterval { get; set; } = 300;       //ms

        //public bool AutoRun { get; set; } = true;
    }
}
