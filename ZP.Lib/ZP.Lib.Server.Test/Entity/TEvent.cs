using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Core.Relation;

namespace ZP.Lib.Server.Test.Entity
{
    internal class TEvent
    {
        public ZProperty<int> Id = new ZProperty<int>();
        public ZDirectEvent onEvent = new ZDirectEvent();

    }

    internal class TEvents
    {
        public ZPropertyList<TEvent> events = new ZPropertyList<TEvent>();

        public ZEvent<int> onSelectMain = new ZEvent<int>();
    }
}
