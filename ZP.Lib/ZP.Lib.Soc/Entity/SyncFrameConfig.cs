using System;
namespace ZP.Lib.Soc.Entity
{
    public class SyncFrameConfig
    {

        public int MaxFrame { get; set; }

        public int FrameInterval { get; set; }

        public int RoundHandTimeout { get; set; }       //ms

        public int HandInterval { get; set; }       //ms

        public int RoundInterval { get; set; }       //ms
         
    }
}
