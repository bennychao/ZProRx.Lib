using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix
{
    public class ZChannelInfo
    {
        //public ZPropertyInterfaceRef<IRawDataPref> channel = new ZProperty<IRawDataPref>();

        public ZPropertyList<ZChannelAction> Actions = new ZPropertyList<ZChannelAction>();

        public ZProperty<IZChannel> ChannelRef = new ZProperty<IZChannel>();


    }
}
