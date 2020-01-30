using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Core.Main;

namespace ZP.Lib.Server.Test.Entity
{
    internal class TestObjectSingletonWIthId : PropObjectSingletonWIthId<string, TestObjectSingletonWIthId>
    {
        public ZProperty<int> IntProp = new ZProperty<int>();
    }

    internal class TestObjectSingletonWIthIdSub : TestObjectSingletonWIthId
    {
        public ZProperty<int> IntProp1 = new ZProperty<int>();
    }

    internal class TestObjectSingle// : PropObjectSingleton<TestObjectSingle>
    {
        public ZProperty<int> IntProp = new ZProperty<int>();
    }

    internal class TestObjectSingleWithTaskScheduler 
        : PropObjectSingletonWithTaskScheduler<TestObjectSingleWithTaskScheduler>
    {
        public ZProperty<int> IntProp = new ZProperty<int>();

        static public bool bReleased = false;
        public override void OnRelease()
        {
            base.OnRelease();

            bReleased = true;
        }
    }

    //will not to Create the sub is error
    //internal class TestObjectSingleSub : TestObjectSingle
    //{
    //    public ZProperty<int> IntProp2 = new ZProperty<int>();
    //}
}
