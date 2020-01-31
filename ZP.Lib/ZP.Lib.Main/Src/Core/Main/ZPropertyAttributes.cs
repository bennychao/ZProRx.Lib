using System;
using System.Collections.Generic;

namespace ZP.Lib
{
	

	/// <summary>
	/// Property description attribute, the item is the localization item's ID.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class PropertyDescriptionAttribute : Attribute {
		public string description;
		public string name;
		private bool bLocalized = false;
		public PropertyDescriptionAttribute(string name, string description = "", bool bLocalized = false)
		{
			this.bLocalized = bLocalized;

			if (bLocalized) {
				this.name = name;
				this.description = description;
			} else {
				this.name = name;
				this.description = description;
			}

		}

		public bool IsLocalized(){
			return bLocalized;
		}
	}

	/// <summary>
	/// Property runtime attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class PropertyRuntimeAttribute : Attribute {

		public bool bSupport = true;
		public PropertyRuntimeAttribute(bool bSupport = true)
		{
			this.bSupport = bSupport;
		}
	}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PEventActionAttribute : Attribute
    {
        public string PropName;
        public PEventActionAttribute(string propName)
        {
            this.PropName = propName;
        }
    }



    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PropertyRuntimeClassAttribute : PropertyRuntimeAttribute {

	}

	/// <summary>
	/// Property record attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class PropertyRecordAttribute : Attribute {

		public bool bSupport = true;
		public int maxRecordCount = 10;
		public PropertyRecordAttribute(bool bSupport = true, int max = 10)
		{
			this.bSupport = bSupport;
			maxRecordCount = max;
		}
	}

	/// <summary>
	/// Property default to value attribute.用于Runtime curValue默认值，如果定义该属性会使用Value进行赋值
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class PropertyDefaultToValueAttribute : Attribute {

		public PropertyDefaultToValueAttribute()
		{

		}
	}


    /// <summary>
    /// Property group attribute.分组
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple =true,  Inherited = true)]
    public class PropertyGroupAttribute : Attribute
    {
        public string GroupName;
        public PropertyGroupAttribute(string groupName)
        {
            this.GroupName = groupName;
        }
    }


    public enum PathType
    {
        LocalRes,
        WebRes
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PropertyConfigPathAttribute : Attribute
    {
        public PathType ResType;
        public string Path;
        public PropertyConfigPathAttribute(PathType resType, string path)
        {
            this.ResType = resType;
            this.Path = path;
            this.Path = this.Path.Replace("[APP]", ServerPath.AppName);
        }

        public PropertyConfigPathAttribute(string path)
        {
            this.ResType = PathType.LocalRes;
            this.Path = path;
            this.Path = this.Path.Replace("[APP]", ServerPath.AppName);
        }

    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PropertyValueChangeAnchorClassAttribute : Attribute
    {
        public List<string> MultiPropIds = new List<string>();
        public PropertyValueChangeAnchorClassAttribute(string multiPropId)
        {
            this.MultiPropIds.Add( multiPropId);
        }

        public PropertyValueChangeAnchorClassAttribute(params string[] multiPropIds)
        {
            this.MultiPropIds.AddRange(multiPropIds);
        }
    }

    /// <summary>
    /// Property link attribute.配合IZLinkable使用
    /// </summary>
    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    //public class PropertyLinkAttribute : Attribute
    //{

    //    public string LinkedPropertyID;
    //    public PropertyLinkAttribute(string linkedPropertyID)
    //    {
    //        this.LinkedPropertyID = linkedPropertyID;
    //    }
    //}

}

