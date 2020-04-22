using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.Matrix.Entity
{
    public class RoundPackage
    {
        public ZProperty<int> CurRound = new ZProperty<int>();
        public ZProperty<string> CurClientId = new ZProperty<string>();
    }
}
