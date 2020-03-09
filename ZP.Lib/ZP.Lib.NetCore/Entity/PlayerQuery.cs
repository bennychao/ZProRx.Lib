using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.NetCore.Entity
{
    public class PlayerQuery
    {
        public static Dictionary<string, string> Convert(string playerId)
        {
            var ret = new Dictionary<string, string>();
            ret.Add("playerId", playerId);
            return ret;
        }
    }
}
