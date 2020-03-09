using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix
{
    public class ZChannelAction
    {
        public ZProperty<string> Name = new ZProperty<string>();
        public ZProperty<string> PackageParamType = new ZProperty<string>();
        public ZProperty<string> ReturnType = new ZProperty<string>();
        public ZProperty<bool> IsInServer = new ZProperty<bool>();
    }
}
