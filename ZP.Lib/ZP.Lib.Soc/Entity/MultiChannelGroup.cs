using System;
using System.Collections.Generic;

namespace ZP.Lib.Soc
{
    public class MultiChannelGroup
    {
        public string Group;

        public List<string> Clients = new List<string>();
        public MultiChannelGroup()
        {
        }

        public MultiChannelGroup(string group)
        {
            this.Group = group;
        }
    }
}
