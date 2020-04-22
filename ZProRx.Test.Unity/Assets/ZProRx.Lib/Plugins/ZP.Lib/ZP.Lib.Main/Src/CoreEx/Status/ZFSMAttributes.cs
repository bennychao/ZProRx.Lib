using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{

    [AttributeUsage(AttributeTargets.Method , Inherited = true)]
    public class FSMConditionAttribute : Attribute
    {
        public string ConName { get; private set; }
        public string FsmName { get; private set; }
        public FSMConditionAttribute(string conName, string fsmName)
        {
            this.ConName = conName;
            this.FsmName = fsmName;
        }

        public FSMConditionAttribute(string conName)
        {
            this.ConName = conName;
        }
    }

}

