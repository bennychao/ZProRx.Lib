
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ZP.Lib
{
    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class RTTransformAttribute : PropertyAddComponentAttribute
    {
        public string SubPropID;
        public Vector2 Scale = Vector2.one;
        public RTTransformAttribute(string subPropID): this(subPropID, Vector2.one)
        {

        }

        public RTTransformAttribute(string subPropID, Vector2 scale)
        {
            this.Scale = scale;
            this.SubPropID = subPropID;
            if (String.IsNullOrEmpty(SubPropID))
                base.AddTypes.Add(typeof(RTTransform3));
            else
                base.AddTypes.Add(typeof(RTTransform3Ex));
        }

        public override void AddComponents(GameObject obj)
        {
            if (ZViewBuildTools.IsView(obj.transform))
                return;

            base.AddComponents(obj);

            if (!String.IsNullOrEmpty(SubPropID))
            {
                obj.GetComponent<RTTransform3Ex>().SubMultiPropID = SubPropID;
                obj.GetComponent<RTTransform3Ex>().AxisScale = Scale;
            }
        }
    }

    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class RTTransformClassAttribute : RTTransformAttribute
    {

        public RTTransformClassAttribute(string subPropID) : base(subPropID)
        {

        }
        public RTTransformClassAttribute(string subPropID, Vector2 scale) : base(subPropID, scale)
        {

        }

        public RTTransformClassAttribute(string subPropID, float scaleX, float scaleY): base(subPropID)
        {
            Scale = new Vector2(scaleX, scaleY);
        }
    }

    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class RTLocalTransformAttribute : PropertyAddComponentAttribute
    {
        public string SubPropID;
        public Vector2 Scale = Vector2.one;
        public RTLocalTransformAttribute(string subPropID): this(subPropID, Vector2.zero)
        {
        }

        public RTLocalTransformAttribute(string subPropID, Vector2 scale)
        {
            this.Scale = scale;
            this.SubPropID = subPropID;
            if (String.IsNullOrEmpty(SubPropID))
                base.AddTypes.Add(typeof(RTLocalTransform3));
            else
                base.AddTypes.Add(typeof(RTLocalTransform3Ex));
        }

        public override void AddComponents(GameObject obj)
        {
            if (ZViewBuildTools.IsView(obj.transform))
                return;

            base.AddComponents(obj);

            if (!String.IsNullOrEmpty(SubPropID))
            {
                obj.GetComponent<RTLocalTransform3Ex>().SubMultiPropID = SubPropID;
                obj.GetComponent<RTLocalTransform3Ex>().AxisScale = Scale;
            }
        }
    }

    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class RTLocalTransformClassAttribute : RTLocalTransformAttribute
    {

        public RTLocalTransformClassAttribute(string subPropID) : base(subPropID)
        {

        }
        public RTLocalTransformClassAttribute(string subPropID, Vector2 scale) : base(subPropID, scale)
        {

        }

        public RTLocalTransformClassAttribute(string subPropID, float scaleX, float scaleY) : base(subPropID)
        {
            Scale = new Vector2(scaleX, scaleY);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class RTAxisScaleAttribute : Attribute
    {
        public Vector2 Scale;
        public RTAxisScaleAttribute(Vector2 scale)
        {
            this.Scale = scale;
        }
    }
}


