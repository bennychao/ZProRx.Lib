using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.CoreEx.Status;
using ZP.Lib.Common;

namespace ZP.Lib.Server.Test.Entity
{


    public enum TestStatus
    {
        Status1,
        Status2,
        Status3,
        Status4,
        StatusMax1
    }

    public enum TestStatus2
    {
        Status5 = TestStatus.StatusMax1 + 1,
        Status6,
        Status7,
        Status8
    }

    public enum TestEvent
    {
        Event1,
        Event2,
        Event3,
        Event4,
        EventMax1
    }

    public enum TestEvent2
    {
        Event5 = TestEvent.EventMax1 + 1,
        Event6,
        Event7,
        Event8
    }

    [PropertyAutoLoadClass("ZProApp/Jsons/Runtime/testFsm.json")]
    internal class TestFsm : ZFSMBuilder<TestFsm, MultiEnum<TestStatus, TestStatus2>, MultiEnum<TestEvent, TestEvent2>>
    {
        public DirectTrigger<int> OnEvent1 = new DirectTrigger<int>(TestEvent.Event1);

        public DirectTrigger<int> OnEvent2 = new DirectTrigger<int>(TestEvent.Event2);

        public DirectTrigger<int> OnEvent3 = new DirectTrigger<int>(TestEvent.Event3);

        public DirectTrigger<int> OnEvent4 = new DirectTrigger<int>(TestEvent.Event4);
    }

    internal class TestFsmObj
    {
        public ZProperty<TestFsm> Fsm = new ZProperty<TestFsm>();

        [PEventAction(".Fsm")]
        public TestFsm.Trigger<int> OnEvent1 = new TestFsm.Trigger<int>(TestEvent.Event1);

        [PEventAction(".Fsm")]
        public TestFsm.Trigger<int> OnEvent2 = new TestFsm.Trigger<int>(TestEvent.Event2);

        [PEventAction(".Fsm")]
        public TestFsm.Trigger<int> OnEvent3 = new TestFsm.Trigger<int>(TestEvent.Event3);

        [PEventAction(".Fsm")]
        public TestFsm.Trigger<int> OnEvent4 = new TestFsm.Trigger<int>(TestEvent.Event4);

        [PEventAction(".Fsm")]
        public TestFsm.Trigger<int> OnEvent5 = new TestFsm.Trigger<int>(TestEvent2.Event5);

        [PropertyLink(".Fsm")]
        public ZStatusProperty<MultiEnum<TestStatus, TestStatus2>, MultiEnum<TestEvent, TestEvent2>> Status 
            = new ZStatusProperty<MultiEnum<TestStatus, TestStatus2>, MultiEnum<TestEvent, TestEvent2>>();
    }
}
