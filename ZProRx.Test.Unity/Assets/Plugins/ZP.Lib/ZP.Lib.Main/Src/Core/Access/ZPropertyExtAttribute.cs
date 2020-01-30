using System;

namespace ZP.Lib
{


	/// <summary>
	/// Property recordable attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class PropertyRecordableAttribute : Attribute {
		public string description;
		public string name;
		private bool bLocalized = false;
		public PropertyRecordableAttribute(string name, string description = "", bool bLocalized = false)
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
    /// Property second property attribute.别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertySecondPropertyAttribute : Attribute
    {

        public string SecondName;
        public PropertySecondPropertyAttribute(string secName)
        {
            this.SecondName = secName;
        }
    }

    /// <summary>
    /// Property second property attribute.关联属性,配合IZLinkable使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyLinkAttribute : Attribute
    {

        public string LinkName;
        public PropertyLinkAttribute(string secName)
        {
            this.LinkName = secName;
        }
    }

    //link its self and this property is ILinkbale
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyLinkSelfAttribute : Attribute
    {

        public PropertyLinkSelfAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyLinkSyncAttribute : Attribute
    {

        public bool bSupport;
        public PropertyLinkSyncAttribute(bool bSupport = true)
        {
            this.bSupport = bSupport;
        }
    }

    //auto load the json, after create
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyAutoLoadAttribute : Attribute
    {
        public string Path;

        public PropertyAutoLoadAttribute(string path)
        {
            this.Path = path;

        }

    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PropertyAutoLoadClassAttribute : PropertyAutoLoadAttribute
    {
        public PropertyAutoLoadClassAttribute(string path) : base(path)
        {

        }
    }


    //public delegate object RefBindEvent();

    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    //public class PropertyRefBindDelegateAttribute : Attribute
    //{


    //    public RefBindEvent onBind;

    //    public PropertyRefBindDelegateAttribute(RefBindEvent onbind)
    //    {
    //        this.onBind = onbind;
    //    }

    //}
}

