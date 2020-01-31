using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace ZP.Lib
{

    /// <summary>
    /// Event method attribute.
    /// </summary>
    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class EventMethodAttribute : Attribute
    {
        public string Method;
        public EventMethodAttribute(string method)
        {
            this.Method = method;
        }


    }

    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyResAttribute : Attribute
    {
        public string Path;

        public PropertyResAttribute(string path)
        {
            this.Path = path;

        }

    }
//#if ZP_UNITY_CLIENT
    /// <summary>
    /// Property image res attribute.
    /// </summary>
    [Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class PropertyImageResAttribute : Attribute
    {
        public ImageResType ResType;
        public string Path;
        public PropertyImageResAttribute(ImageResType resType, string path)
        {
            this.ResType = resType;
            this.Path = path;

        }

        public PropertyImageResAttribute(string path)
        {
            this.ResType = ImageResType.LocalRes;
            this.Path = path;
            this.Path = this.Path.Replace("[APP]", ServerPath.AppName);
        }

    }
//#endif
    /// <summary>
    /// Property user interface item type attribute.
    /// </summary>
    //[Conditional("ZP_UNITY_CLIENT")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class PropertyUIItemResAttribute : Attribute {
		public GameObject prefab;

		protected string resPath;
		public string parentNode;
		public PropertyUIItemResAttribute(string resPath, string parentNode = "")
		{
			this.resPath =  resPath;
            this.resPath = this.resPath.Replace("[APP]", ServerPath.AppName);

			this.parentNode = parentNode;
		}

		public ResourceRequest LoadResAsync<T>(){
#if ZP_SERVER
            return ZServerResources.LoadAsync(this.resPath);
#else
            return Resources.LoadAsync(this.resPath);
#endif
        }

        public virtual GameObject LoadRes(IZProperty prop){

#if ZP_SERVER
            return ZServerResources.Load(this.resPath) as GameObject;
#else
            return Resources.Load(this.resPath) as GameObject;
#endif
        }

        public virtual bool IsRes(string name)
        {
             var indtx = resPath.LastIndexOf('/');
            return (name.CompareTo(resPath.Substring(indtx)) == 0);

        }
    }

	/// <summary>
	/// Property user interface item res class attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
    //[Conditional("ZP_UNITY_CLIENT")]
	public class PropertyUIItemResClassAttribute : PropertyUIItemResAttribute {

		protected string subID;

        private string path;
		public PropertyUIItemResClassAttribute(string resPath, string parentNode = "", string subID="")
			:base(resPath, parentNode)
		{
			this.subID = subID;
		}

		/// <summary>
		/// Loads the res.
		/// </summary>
		/// <returns>The res.</returns>
		/// <param name="prop">Property.</param>
		public override GameObject LoadRes(IZProperty prop){

			string subModelName = "";
            
            //support multi property ex. ".ModleName"
			var modelProp = ZPropertyMesh.GetPropertyEx (prop.Value, subID);
			if (modelProp != null) {
				//Assert.IsNotNull (modelProp.Value, prop.PropertyID + " not define the model name");
				subModelName = (string)modelProp.Value.ToString();
			}

            path = this.resPath + subModelName;

#if ZP_SERVER
            var resOjb = ZServerResources.Load(path);
#else
            var resOjb = Resources.Load(path);
#endif

            return resOjb as GameObject;
		}


        public override bool IsRes(string name)
        {
            var indtx = path.LastIndexOf('/');
            return (name.CompareTo(path.Substring(indtx)) == 0);

        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]//AllowMultiple = true, 
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyBaseAddComponentAttribute : Attribute
    {

        public List<Type> AddTypes = new List<Type>();
        public PropertyBaseAddComponentAttribute(Type Type)
        {
            AddTypes.Add(Type);
        }

        public PropertyBaseAddComponentAttribute(params Type[] Types)
        {
            foreach (var t in Types)
            {
                AddTypes.Add(t);
            }
        }

        public PropertyBaseAddComponentAttribute()
        {

        }

        public virtual void AddComponents(GameObject obj)
        {
            foreach (var t in AddTypes)
            {
                obj.AddComponent(t);
            }
        }

    }

    /// <summary>
    /// Property user interface item add component class attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]//AllowMultiple = true, 
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyAddComponentAttribute : PropertyBaseAddComponentAttribute {

		
		public PropertyAddComponentAttribute(Type Type): base(Type)
		{
		}

		public PropertyAddComponentAttribute(params Type[] Types): base(Types)
		{
		}
	}

    public enum AddComponentFlag
    {
        All,
        RT,
        View
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true, Inherited = false)]
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyAddComponentClassAttribute : PropertyAddComponentAttribute {
        private string group;
        public string Group => group;
        public AddComponentFlag Flag = AddComponentFlag.All;
        public PropertyAddComponentClassAttribute(Type Type, AddComponentFlag flag = AddComponentFlag.All)
			:base(Type)
		{
            Flag = flag;
		}

		public PropertyAddComponentClassAttribute(params Type[] Types)
		{
			foreach (var t in Types) {
				AddTypes.Add (t);
			}
		}


        public PropertyAddComponentClassAttribute(AddComponentFlag flag,  params Type[] Types)
        {
            Flag = flag;
            foreach (var t in Types)
            {
                AddTypes.Add(t);
            }
        }

        public PropertyAddComponentClassAttribute(string groupId, params Type[] Types)
        {
            this.group = groupId;
            foreach (var t in Types)
            {
                AddTypes.Add(t);
            }
        }

        public override void AddComponents(GameObject obj)
        {
            if (ZViewBuildTools.IsView(obj.transform) && Flag == AddComponentFlag.RT)
                return;

            if (!ZViewBuildTools.IsView(obj.transform) && Flag == AddComponentFlag.View)
                return;


            foreach (var t in AddTypes)
            {
                if (obj.GetComponent(t)  == null)
                    obj.AddComponent(t);
            }
        }

    }

	/// <summary>
	/// Property bind self attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property  | AttributeTargets.Class, Inherited = true)]
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyBindSelfAttribute : Attribute {

		//public Type AddType;
		public PropertyBindSelfAttribute()
		{
			//AddType = Type;
		}

	}

    /// <summary>
    /// Property bind stop attribute.如果有这个属性绑定终止，不再对子结点进行绑定
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyBindStopAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyBindCountAttribute : Attribute
    {

        public int Count = 0;
        public PropertyBindCountAttribute(int count)
        {
            this.Count = count;
            //AddType = Type;
        }

    }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyAutoFindAttribute : Attribute
    {

        public string SecondName = "";
        public PropertyAutoFindAttribute(string secname)
        {
            this.SecondName = secname;
            //AddType = Type;
        }

        public bool IfHasSecondName()
        {
            return !String.IsNullOrEmpty(SecondName);
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    //[Conditional("ZP_UNITY_CLIENT")]
    public class PropertyBindCompoentAttribute : Attribute
    {

        public Type CompoentType;
        public string Action;
        public PropertyBindCompoentAttribute(Type comType, string action)
        {
            this.CompoentType = comType;
            this.Action = action;
            //AddType = Type;

            //assert the action valid
        }

    }

    /// <summary>
    /// Property record attribute.
    /// </summary>
    //	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    //	public class PropertyAxes2Attribute : Attribute, IZAxes2 {
    //
    //		public bool bSupport = true;
    //		public int maxRecordCount = 10;
    //		public PropertyAxes2Attribute(bool bSupport = true, int max = 10)
    //		{
    //			this.bSupport = bSupport;
    //			maxRecordCount = max;
    //		}
    //	}
}

