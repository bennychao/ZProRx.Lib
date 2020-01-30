using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Concurrent;
using ZP.Lib.Core.Relation;
using ZP.Lib.Core.Domain;

namespace ZP.Lib
{

    public static partial class ZPropertyMesh
	{
		static private List<ZPropertyAttributeNode> nodes = new List<ZPropertyAttributeNode>();
		static private ConcurrentDictionary<string, Type> types = new ConcurrentDictionary<string, Type>();

		/// <summary>
		/// Build the specified field.
		/// </summary>
		/// <param name="field">Field.</param>
		static public void Build(Type type){

            //			if (!field.GetType ().IsSubclassOf (typeof(IZProperty)))
            //				return;

            //if (!types.Contains (type) && IsPropertableLowAPI(type)) {
            //	types.Add (type);
            //}

            //for thread safe 
            types.GetOrAdd(type.FullName, (key) =>
            {
                BuildTypeNodes(type);

                return type;
            });

        }

        static private void BuildTypeNodes(Type type)
        {
            FieldInfo[] typeInfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int j = 0; j < typeInfo.Length; j++)
            {

                if (typeInfo[j].MemberType == MemberTypes.Field)
                {

                    //if (typeInfo [j].IsDefined (typeof(ZP.Lib.PropertyDescriptionAttribute), true)) {
                    if (typeof(IZProperty).IsAssignableFrom(typeInfo[j].FieldType))
                    {
                        //create a property node
                        ZPropertyAttributeNode node = new ZPropertyAttributeNode(typeInfo[j], type);
                        nodes.Add(node);
                    }

                    else if (typeof(IZEvent).IsAssignableFrom(typeInfo[j].FieldType))
                    {
                        //create a property node
                        ZPropertyAttributeNode node = new ZPropertyAttributeNode(typeInfo[j], type);
                        nodes.Add(node);
                    }
                }
            }
        }

		/// <summary>
		/// Binds the proptery nodes.
		/// </summary>
		static public void BindPropertyNodes(object obj){
			FieldInfo[] typeInfo = obj.GetType ().GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            List<IZPropertable> links = new List<IZPropertable>();

			for (int j = 0; j < typeInfo.Length; j++)
			{

				if (typeInfo [j].MemberType == MemberTypes.Field) {

					//if (typeInfo [j].IsDefined (typeof(ZP.Lib.PropertyDescriptionAttribute), true)) {
					if (typeof(IZProperty).IsAssignableFrom(typeInfo[j].FieldType)){

						string propertyID = GetTypeName( obj.GetType()) + "." + typeInfo[j].Name;
						var node = nodes.Find (a => a.PropertyID.CompareTo (propertyID) == 0);

						var prop =  typeInfo [j].GetValue (obj) as IZProperty;

                        Assert.IsNotNull(prop, "not create the ZProperty, you should new it " + propertyID);

						prop.AttributeNode = node;
                       
                        //link the property
                        if (IsLinkable(prop) )
                        {
                            links.Add(prop);
                            //var attr = prop.AttributeNode.GetAttribute<PropertyLinkAttribute>();
                            //if (attr != null)
                            //{
                            //    //propID support multiPropertyID ex. .persion.weapon.power
                            //    (prop as IZLinkable).LinkProperty = GetPropertyEx(obj, attr.LinkName);
                            //}
                        }


                        continue;
					}
                    //add for event's support
                    else if (typeof(IZEvent).IsAssignableFrom(typeInfo[j].FieldType))
                    {
                        string propertyID = GetTypeName(obj.GetType()) + "." + typeInfo[j].Name;
                        var node = nodes.Find(a => a.PropertyID.CompareTo(propertyID) == 0);

                        var prop = typeInfo[j].GetValue(obj) as IZEvent;

                        Assert.IsNotNull(prop, "not create the ZEvent, you should new it " + propertyID);

                        prop.AttributeNode = node;

                        if (IsDirectLinkable(prop))
                        {
                            (prop as IDirectLinkable).InitParent(obj);
                        }

                        //link the event
                        if (IsLinkable(prop))
                        {
                            links.Add(prop);
                            //var attr = prop.AttributeNode.GetAttribute<PEventActionAttribute>();
                            //if (attr != null)
                            //{
                            //    (prop as IZLinkable).LinkProperty = GetPropertyEx(obj, attr.PropName);
                            //}
                        }
                        continue;
                    }
                }
			}

            foreach (var prop in links)
            {
                if (ZPropertyMesh.IsProperty(prop))
                {
                    var attr = prop.AttributeNode.GetAttribute<PropertyLinkAttribute>();
                    if (attr != null)
                    {
                        //propID support multiPropertyID ex. .persion.weapon.power
                        (prop as IZLinkable).LinkProperty = GetPropertyEx(obj, attr.LinkName);
                    }
                }
                else if (IsEvent(prop)){
                    var attr = prop.AttributeNode.GetAttribute<PEventActionAttribute>();
                    if (attr != null)
                    {
                        (prop as IZLinkable).LinkProperty = GetPropertyEx(obj, attr.PropName);
                    }
                }
            }
        }
        

		/// <summary>
		/// Binds the property attribute.
		/// </summary>
		static public void BindPropertyAttribute(IZProperty prop, string propertyID){
			var node = nodes.Find (a => a.PropertyID.CompareTo (propertyID) == 0);

			prop.AttributeNode = node;
		}

		/// <summary>
		/// Creates the object.
		/// </summary>
		static public T CreateObject<T>(){
			T obj = PropertyObjPool<T>.CreateInstance ();

			if (obj != null)
				CreateSubObject (obj);

			InvokeCreateMethod (obj);
			return obj;
		}

        static public T CreateObjectWithParam<T>(params object[] args)
        {
            T obj = PropertyObjPool<T>.CreateInstance(args);

            if (obj != null)
                CreateSubObject(obj);

            InvokeCreateMethod(obj);
            return obj;
        }


        static public T CreateObject<T>(bool bCache)
        {
            var ret = CreateObject<T>();

            return ret;
        }

        static public void ReleaseObject<T>(T obj)
        {

            //ZPropertyCache<T>.Cache(obj);

            InvokeDestroyMethod(obj);
        }

        static public void ReleaseObject(object obj)
        {

            //ZPropertyCache<T>.Cache(obj);
            InvokeDestroyMethod(obj);
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        static public object CreateObject(Type type){
			object obj = CreateInstance (type);
			if (obj != null)
				CreateSubObject (obj);

			InvokeCreateMethod (obj);
			return obj;
		}

        static public object CreateObjectWithParam(Type type, params object[] args)
        {
            object obj = CreateInstanceWithParam(type, args);
            if (obj != null)
                CreateSubObject(obj);

            InvokeCreateMethod(obj);
            return obj;
        }

        /// <summary>
        /// Creates the sub object.
        /// </summary>
        static internal void CreateSubObject(object obj)
        {
            List<IZProperty> subList = GetPropertiesWithPropertyable(obj);
            foreach (var s in subList)
            {
                //check if ref is assigned, already!!
                if (ZPropertyMesh.IsRef(s) && s.Value != null)
                {
                    continue;
                }

                var newobj = CreateObject(s.GetDefineType());

                var attr = s.AttributeNode.GetAttribute<PropertyAutoLoadAttribute>();
                if (attr != null)
                {
                    ZPropertyPrefs.LoadFromRes(newobj, attr.Path);
                }
                else
                {
                    var attrClass = ZPropertyAttributeTools.GetAttribute<PropertyAutoLoadClassAttribute>(s);
                    if (attrClass != null)
                    {
                        ZPropertyPrefs.LoadFromRes(newobj, attrClass.Path);
                    }
                }


                s.Value = newobj;
            }
        }

		/// <summary>
		/// Creates the instance.
		/// </summary>
		static internal object CreateInstance(Type type)
		{
			if (type.IsInterface)
				return null;
			
			object obj = Activator.CreateInstance (type);

			if (!IsPropertableLowAPI (type))
				return obj;

			if (!ZPropertyMesh.IsPropertable (type)) {
				ZPropertyMesh.Build (type);
			}

			ZPropertyMesh.BindPropertyNodes (obj);


			return obj;
		}

        static internal object CreateInstanceWithParam(Type type, params object[] args)
        {
            if (type.IsInterface)
                return null;

            object obj = Activator.CreateInstance(type, args);

            if (!IsPropertableLowAPI(type))
                return obj;

            if (!ZPropertyMesh.IsPropertable(type))
            {
                ZPropertyMesh.Build(type);
            }

            ZPropertyMesh.BindPropertyNodes(obj);


            return obj;
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        public static object CloneObject(object obj){

            //to support value type;
            if (!ZPropertyMesh.IsPropertable(obj))
            {
                return obj;
            }

            object newObj = CreateInstance (obj.GetType());
			if (newObj != null)
				CloneProperties (obj, newObj);

			InvokeCreateMethod (newObj);

            //if (SendCopyMsg)
            InvokeCopyMethod(newObj);

            return newObj;
		}

        //public static T CloneObject<T>(T obj)
        //{
        //    return (T)CloneObject(obj);
        //}


        /// <summary>
        /// Creates the sub object.
        /// </summary>
        static public void CloneProperties(object obj, object newObj){
			List<IZProperty> subList = GetProperties (obj);
			foreach (var s in subList) {

				var newProp = ZPropertyMesh.GetProperty (newObj, s.PropertyID);
				Assert.IsNotNull (newProp, "Clone property error");

                if (IsPropertyListLike(s))
                {
                    newProp.Copy(s);
                    continue;
                }

                newProp.Copy(s);


                if (s.Value == null)
                    continue;


                if (IsPropertable (s.Value.GetType())) {
					var newSubObj = CloneObject (s.Value);
					newProp.Value = newSubObj;
					continue;
				}

				
			}
		}

		/// <summary>
		/// Invokes the create method.
		/// </summary>
		static internal void InvokeCreateMethod(object obj){
			Type type = obj.GetType ();
			MethodInfo method = type.GetMethod("OnCreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
			object[] parameters = null;

			if (method != null)
				method.Invoke(obj, parameters);                           // 调用方法，参数为空
		}

		/// <summary>
		/// Invokes the create method.
		/// </summary>
		static public void InvokeLoadMethod(object obj){
			Type type = obj.GetType ();
			MethodInfo method = type.GetMethod("OnLoad", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
			object[] parameters = null;

			if (method != null)
				method.Invoke(obj, parameters);                           // 调用方法，参数为空
		}

        static internal void InvokeCopyMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnCopy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static internal void InvokeDestroyMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

    }

}



