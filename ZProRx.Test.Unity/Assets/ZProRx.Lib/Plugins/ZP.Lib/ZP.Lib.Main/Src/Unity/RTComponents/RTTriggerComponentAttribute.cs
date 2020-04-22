using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZP.Lib.Unity.RTComponents
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class RTAddTriggerComponentAttribute : PropertyAddComponentAttribute
    {
        public string SubEventId { get; protected set; }
        public RTAddTriggerComponentAttribute(Type type, string subEventId) : base(type)
        {
            this.SubEventId = subEventId;
        }

        public override void AddComponents(GameObject obj)
        {
            if (ZViewBuildTools.IsView(obj.transform))
                return;

            base.AddComponents(obj);
        }
    }

    [AttributeUsage(AttributeTargets.Class,  AllowMultiple =true, Inherited = true)]
    public class AddTriggerComponentClassAttribute : PropertyAddComponentClassAttribute
    {
        public string SubEventId { get; protected set; }
        public AddTriggerComponentClassAttribute(Type type, string subEventId) : base(type)
        {
            this.SubEventId = subEventId;
        }

        public override void AddComponents(GameObject obj)
        {
            if (ZViewBuildTools.IsView(obj.transform))
                return;

            base.AddComponents(obj);
        }
    }
}
