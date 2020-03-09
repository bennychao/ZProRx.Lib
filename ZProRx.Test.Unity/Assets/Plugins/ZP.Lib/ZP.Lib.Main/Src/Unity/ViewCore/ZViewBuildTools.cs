//#if ZP_UNITY_CLIENT
using System;
using UnityEngine;
using System.Collections;
using System.Reflection;

#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif

using System.Collections.Generic;
using System.Linq;


namespace ZP.Lib
{

	public static class ZViewCommonObject{
		public static string Name = "Name";
		public static string Description = "Description";
		public static string Data = "Data";

        public static string Icon = "Icon";
        //public static string Count = "Count";

        public static string Bar = "Bar";

        public static string Root = "Root";
        public static string Items = "Items";

        public static string Model = "Model";

        public static string Minute = "Minute";
        public static string Second = "Second";

        public static string Image = "Image";

        public static string HighLight = "HighLight";

    }

	public static class ZViewBuildTools
	{

        /// <summary>
        /// Binds the object.
        /// </summary>
        /// <returns><c>true</c>, if object was bound, <c>false</c> otherwise.</returns>
        /// <param name="obj">Object.</param>
        static public bool BindObject(object obj, Transform transform, bool toShow = true){
            var root = transform.GetComponent<ZPropertyViewRootBehaviour>() as IZPropertyViewRoot;

            
            if (root != null && root.IsBinded(obj))
            {
                transform.gameObject.SetActive(toShow);
                return true;
            }

            //must be setActive first
            transform.gameObject.SetActive(toShow);

            bool ret = _bindObject (obj, transform);
            
            root = transform.GetComponent<ZPropertyViewRootBehaviour>() as IZPropertyViewRoot;

            if (root != null) {
				root.Bind (obj);
			}

            return ret;
		}

        

        static public bool BindObject(object obj, Transform transform, string group, bool toShow = true)
        {
            var root = transform.GetComponent<ZPropertyViewRootBehaviour>() as IZPropertyViewRoot;

            if (root != null && root.IsBinded(obj))
            {
                transform.gameObject.SetActive(toShow);
                return true;
            }
            
            transform.gameObject.SetActive(toShow);

            bool ret = _bindObject(obj, transform, group);

            root = transform.GetComponent<ZPropertyViewRootBehaviour>() as IZPropertyViewRoot;

            if (root != null)
            {
                root.Bind(obj);
            }

            return ret;
        }

        static public bool BindObject(object obj, Transform transform, List<string> groups, bool toShow = true)
        {
            var root = transform.GetComponent<ZPropertyViewRootBehaviour>() as IZPropertyViewRoot;
            if (root != null && root.IsBinded(obj))
            {
                transform.gameObject.SetActive(toShow);
                return true;
            }

            transform.gameObject.SetActive(toShow);

            bool ret = _bindObject(obj, transform, groups);

            root = transform.GetComponent<ZPropertyViewRootBehaviour>() as IZPropertyViewRoot;

            if (root != null)
            {
                root.Bind(obj);
            }

            return ret;
        }


        static public void UnBindObject(object obj)
        {
            InvokePreUnbindMethod(obj);

            //get all child properties
            var proList = ZPropertyMesh.GetPropertiesInSubs(obj);
            //transform.enabled = toHide;

            foreach (var p in proList)
            {
                UnbindUIItem(p);
            }

            UnBindEvent(obj);
           
            InvokeUnbindMethod (obj);
        }

        static public void UnBindObject(object obj, Transform transform, bool toHide = true)
        {
            InvokePreUnbindMethod(obj, transform);
            //get all child properties
            var proList = ZPropertyMesh.GetPropertiesInSubs(obj);
            transform.gameObject.SetActive(!toHide);

            foreach (var p in proList)
            {
                UnbindUIItem(p);
            }

            UnBindEvent(obj);

            InvokeUnbindMethod(obj);
        }

        /// <summary>
        /// Binds the object.
        /// </summary>
        /// <returns><c>true</c>, if object was bound, <c>false</c> otherwise.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="transform">Transform.</param>
        static private bool _bindObject(object obj, Transform transform){

            InvokePreBindMethod(obj, transform);

            AddComponents(obj, transform.gameObject);

            //_bindEvents(obj, transform);

            var list = ZPropertyMesh.GetPropElements (obj);
			if (list == null || list.Count <= 0)
				return false;

			foreach (var a in list) {
                if (ZPropertyMesh.IsProperty(a))
                    BindProperty(a as IZProperty, transform);
                else if (ZPropertyMesh.IsEvent(a))
                    BindEvent(a as IZEvent, transform);

            }

			InvokeBindMethod (obj, transform);

			return true;
		}



        static private bool _bindObject(object obj, Transform transform, string group)
        {
            //_bindEvents(obj, transform);
            var groups = new List<string>();
            groups.Add(group);
            InvokePreBindMethod(obj, transform, groups);

            AddComponents(obj, transform.gameObject, groups);

            //bind properties
            var list = ZPropertyMesh.GetPropElements(obj);
            if (list == null || list.Count <= 0)
                return false;

            foreach (var a in list)
            {
                var attr = a.AttributeNode.GetAttribute<PropertyGroupAttribute>();

                if (attr == null || group.CompareTo(attr.GroupName) != 0)
                    continue;

                if (ZPropertyMesh.IsProperty(a))
                    BindProperty(a as IZProperty, transform);
                else if (ZPropertyMesh.IsEvent(a))
                    BindEvent(a as IZEvent, transform);
            }

            InvokeBindMethod(obj, transform);

            return true;
        }

        static private bool _bindObject(object obj, Transform transform, List<string> groups)
        {
            //_bindEvents(obj, transform);
            InvokePreBindMethod(obj, transform, groups);

            AddComponents(obj, transform.gameObject, groups);

            var list = ZPropertyMesh.GetPropElements(obj);
            if (list == null || list.Count <= 0)
                return false;

            foreach (var a in list)
            {
                var attr = a.AttributeNode.GetAttribute<PropertyGroupAttribute>();

                if (attr == null || !groups.Contains(attr.GroupName))
                    continue;

                if (ZPropertyMesh.IsProperty(a))
                    BindProperty(a as IZProperty, transform);
                else if (ZPropertyMesh.IsEvent(a))
                    BindEvent(a as IZEvent, transform);
            }

            InvokeBindMethod(obj, transform);

            return true;
        }

        /// <summary>
        /// Binds the events. not used
        /// </summary>
        /// <param name="obj">Object.</param>
        static private void _bindEvents(object obj, Transform transform)
        {
            //bind event
            var eventList = ZPropertyMesh.GetEvents(obj);

            if (eventList != null)
            {
                foreach (var e in eventList)
                {
                    BindEvent(e, transform);
                }
            }
        }

        /// <summary>
        /// Binds the event.
        /// </summary>
        /// <param name="prop">Property.</param>
        /// <param name="transform">Transform.</param>
        static internal void BindEvent(IZEvent prop, Transform transform = null)
        {
            var attr = prop.AttributeNode.GetAttribute<EventMethodAttribute>();
            if (attr == null ||(attr != null && EventMethod.OnClick.Contains(attr.Method)))
            {
                var parent = ZViewBuildTools.FindInChilds(transform, prop.AttributeNode.PropertyID);
                if (parent == null)
                {
                    //Debug.LogWarning("No Button on UI " + prop.AttributeNode.PropertyID);
                    return;
                }

#if ZP_UNITY_CLIENT
                var btn = parent.GetComponent<Button>();
                if (btn != null)
                //{
                //    Debug.LogWarning("No Button on UI " + prop.AttributeNode.PropertyID);
                //}
                //else
                {
#if DEBUG
                    //var item = BindDebugEvent(btn.transform);
                    ////item.PropName = prop.PropertyID;
                    //item.Bind(prop);
                    
#endif

                    prop.TransNode = parent;
                    //Button.ButtonClickedEvent
                    btn.onClick.AddListener(()=> { 
                        prop.Invoke(); 
                    });
                    //btn.onClick = (Button.ButtonClickedEvent)(prop.Value);
                }
                else
#endif
                {
                    //for custom event behaviour
#if DEBUG && ZP_UNITY_CLIENT
                   // BindDebugEvent(btn.transform);
#endif

                    var item = parent.GetComponent<IZEventItem>() as IZEventItem;
                    if (item != null)
                    {
                        item.Bind(prop);
                        prop.TransNode = parent;
                    }

                }

            }
        }

        static public void UnBindEvent(object obj)
        {
            //get all child properties
            var eventList = ZPropertyMesh.GetEventsInSubs(obj);
            //transform.enabled = toHide;

            foreach (var e in eventList)
            {
                UnBindEvent(e);
            }

            //InvokeUnbindMethod(obj);
        }

        /// <summary>
        /// Binds the property.
        /// </summary>
        /// <returns><c>true</c>, if property was bound, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        /// <param name="transform">Transform.</param>
        static public bool BindProperty(IZProperty prop, Transform transform = null, List<string> groups = null){

            Transform item = null;
            var bListItem = ZPropertyMesh.IsListItem(prop);

            var secAttr = prop.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>();
            if (secAttr != null)
            {
                item = FindItem(secAttr.SecondName, transform);
            }

            if (item == null && ZPropertyMesh.IsPartProperty(prop) && !bListItem)
            {
                item = FindItem((prop as IZPartProperty)?.PartName, transform);
            }

            if (item == null && !bListItem)
            {
                item  = FindItem(prop.PropertyID, transform);
            }

            if (item == null && !bListItem)
            {
                item = FindItem(prop.SimplePropertyID, transform);
            }

            if (item == null && !bListItem)
            {
                item = FindItem(transform, tran => tran.name.Contains(prop.SimplePropertyID));
                if (item != null)
                {
                    BindMultiProperty(prop, "", item); ///prop.SimplePropertyID
                }
            }

            if (prop.PropertyID.CompareTo ("") == 0) {
                
            }

			if (item == null) {
				var UIItemAttr = ZPropertyAttributeTools.GetAttribute<PropertyUIItemResAttribute> (prop);
				if (UIItemAttr != null && !ZPropertyMesh.IsPropertyList(prop) && !ZPropertyMesh.IsPropertyRefList(prop)) {
					item = CreateViewItem (prop, transform);

                    if (bListItem)
                    {
                        item.gameObject.name += transform.childCount.ToString();
                    }
				} 
//				else if (prop.AttributeNode.GetAttribute<PropertyBindSelfAttribute> () != null) {
//					item = transform;
//				}
				else {
                    //Debug.LogWarning("Not Find UIItem match to " + prop.PropertyID);
					return false;
				}
			}

			Transform parent = (item == null ? transform : item);

            //if (prop.Value == null)
            //{
            //    Debug.LogWarning(" value is null " + prop.PropertyID);
            //}

            //list property
            if (//ZPropertyMgr.IsRankable (prop) || 认为Rank的没有再是子类
            ZPropertyMesh.IsPropertyList (prop) ||
            ZPropertyMesh.IsPropertyRefList(prop)) {
                BindPropertyList(prop, parent);
            } 
			// sub object
			else if (prop.Value != null &&
                ZPropertyMesh.IsPropertable (prop.Value.GetType())){
				_bindObject (prop.Value, parent);
			}

            if (!ZPropertyMesh.IsPropertyList(prop) && !ZPropertyMesh.IsPropertyRefList(prop))
                BindUIItem(prop, item, groups);
            else
                BindUIList(prop, item, groups);

            return true;
		}

        static private bool _bindMultiObject(object obj, string multiPropId, Transform transform, List<string> groups = null)
        {

            InvokePreBindMethod(obj, transform);

            //_bindEvents(obj, transform);

            var list = ZPropertyMesh.GetPropElements(obj);
            if (list == null || list.Count <= 0)
                return false;

            foreach (var a in list)
            {
                if (ZPropertyMesh.IsProperty(a))
                    BindMultiProperty(a as IZProperty, multiPropId, transform);
                else if (ZPropertyMesh.IsEvent(a))
                    BindEvent(a as IZEvent, transform);

            }

            InvokeBindMethod(obj, transform);

            return true;
        }

        static internal bool BindMultiProperty(IZProperty prop, string multiPropId, Transform transform = null, List<string> groups = null)
        {
            Transform item = null;
            var secAttr = prop.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>();
            if (secAttr != null)
            {
                var curMultiId = multiPropId + "." + secAttr.SecondName;
                if (string.Compare(transform.name, curMultiId, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    item = transform;
                }
                else if (transform.name.Contains(curMultiId))
                {
                    _bindMultiObject(prop.Value, curMultiId, transform, groups);
                }
            }

            if (item == null && ZPropertyMesh.IsPartProperty(prop))
            {
                var curMultiId = multiPropId + "." + (prop as IZPartProperty)?.PartName;

                //item = FindItem((prop as IZPartProperty)?.PartName, transform);
                if (string.Compare(transform.name, curMultiId, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    item = transform;
                }
                else if (transform.name.Contains(curMultiId))
                {
                    _bindMultiObject(prop.Value, curMultiId, transform, groups);
                }
            }

            if (item == null)
            {
                var curMultiId = multiPropId + prop.SimplePropertyID; //include "."
                if (string.Compare(transform.name, curMultiId, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    item = transform;
                }
                else if (transform.name.Contains(curMultiId))
                {
                    _bindMultiObject(prop.Value, curMultiId, transform, groups);
                }
            }

            if (prop.PropertyID.CompareTo("") == 0)
            {

            }

            if (item == null)
            {
                //var UIItemAttr = ZPropertyAttributeTools.GetAttribute<PropertyUIItemResAttribute>(prop);
                //if (UIItemAttr != null && !ZPropertyMesh.IsPropertyList(prop) && !ZPropertyMesh.IsPropertyRefList(prop))
                //{
                //    item = CreateViewItem(prop, transform);
                //}
                ////				else if (prop.AttributeNode.GetAttribute<PropertyBindSelfAttribute> () != null) {
                ////					item = transform;
                ////				}
                //else
                //{
                    //Debug.LogWarning("Not Find UIItem match to " + prop.PropertyID);
                    return false;
                //}
            }

            Transform parent = (item == null ? transform : item);

            //if (prop.Value == null)
            //{
            //    Debug.LogWarning(" value is null " + prop.PropertyID);
            //}

            //list property
            if (//ZPropertyMgr.IsRankable (prop) || 认为Rank的没有再是子类
            ZPropertyMesh.IsPropertyList(prop) ||
            ZPropertyMesh.IsPropertyRefList(prop))
            {
                BindPropertyList(prop, parent);
            }
            // sub object
            else if (prop.Value != null &&
                ZPropertyMesh.IsPropertable(prop.Value.GetType()))
            {
                _bindObject(prop.Value, parent);
            }

            if (!ZPropertyMesh.IsPropertyList(prop) && !ZPropertyMesh.IsPropertyRefList(prop))
                BindUIItem(prop, item, groups);
            else
                BindUIList(prop, item, groups);

            return true;
        }


        /// <summary>
        /// Binds the property list.
        /// </summary>
        static public void BindPropertyList(IZProperty prop, Transform parent)
        {

            int? count = prop.AttributeNode.GetAttribute<PropertyBindCountAttribute>()?.Count;

            int index = 0;
            foreach (var p in prop as IEnumerable)
            {
                if (p as IZProperty != null)
                {
                    BindProperty(p as IZProperty, parent);
                }

                if (count != null && index++ > count)
                    break;
            }
        }

        static public void BindPropertyList<T>(List<T> prop, Transform parent)
        {

            foreach (var p in prop as IEnumerable)
            {
                if (p as IZProperty != null)
                {
                    BindProperty(p as IZProperty, parent);
                }

            }
        }

        /// <summary>
        /// Bind the specified prop and item.
        /// </summary>
        static private void BindUIItem(IZProperty prop, Transform item, List<string> groups = null)
        {
			if (item != null) {

				AddComponents (prop, item.gameObject);
#if DEBUG  && ZP_UNITY_CLIENT
               /// BindDebugItem(item);
#endif

                prop.TransNode = item;

                var propItems = item.GetComponents<IZPropertyViewItem>();
				if (propItems != null) {

                    //prop.ViewItem = propItem;
                    foreach (var p in propItems)
                    {
                        if (p.IsBind)
                            continue;

                        if (!p.Bind(prop)) {
                            //delete the bind
                            //prop.ViewItem = null;
                        }
                    }

                    return;
				}

			}
		}


        static private void UnbindUIItem(IZProperty prop){
			if (prop.TransNode != null) {

                var bProperty = prop.Value != null &&
                ZPropertyMesh.IsPropertable(prop.Value.GetType());

                if (bProperty)
                    InvokePreUnbindMethod(prop.Value, prop.TransNode);

                //AddComponents (prop, item.gameObject);
#if DEBUG && ZP_UNITY_CLIENT
               /// UnBindDebugItem(item);
#endif
                var propItems = prop.TransNode.GetComponents<IZPropertyViewItem>();
				if (propItems != null) {

                    //prop.ViewItem = propItem;
                    foreach (var p in propItems)
                    {
                        p.Unbind();
                    }                    
				}
                prop.TransNode = null;

                if (bProperty)
                    InvokeUnbindMethod(prop.Value);

                return;
            }
		}

        static private void UnBindEvent(IZEvent ev)
        {
            if (ev.TransNode != null)
            {
                //AddComponents (prop, item.gameObject);
#if DEBUG && ZP_UNITY_CLIENT
               /// UnBindDebugItem(item);
#endif
                var propItems = ev.TransNode.GetComponents<IZEventItem>();
                if (propItems != null)
                {

                    //prop.ViewItem = propItem;
                    foreach (var p in propItems)
                    {
                        p.Unbind();
                    }
                }
                ev.TransNode = null;

                return;
            }
        }


        static private void BindUIList(IZProperty prop, Transform item, List<string> groups = null)
        {
            if (item != null)
            {
                //only support the ZProperty List's Attributes
                // AddComponents(prop, item.gameObject);
                AddPropertyClassComponents(prop, item.gameObject, groups);
#if DEBUG && ZP_UNITY_CLIENT
               // BindDebugItem(item);
#endif

                prop.TransNode = item;

                var propItems = item.GetComponents<IZPropertyViewItem>();
                if (propItems != null)
                {

                    //prop.ViewItem = propItem;
                    foreach (var p in propItems)
                    {
                        if (!p.Bind(prop))
                        {
                            //delete the bind
                            //prop.ViewItem = null;
                        }
                    }

                    return;
                }

            }
        }

        /// <summary>
        /// Adds the components.
        /// </summary>
        /// <param name="property">Property.</param>
        /// <param name="gameObject">Game object.</param>
        static private void AddComponents(IZProperty property, GameObject gameObject, List<string> groups = null)
        {
			//add Component
			var attrs = ZPropertyAttributeTools.GetAttributesExcludeListClass<PropertyAddComponentAttribute> (property);
			if (attrs != null) {

                foreach (var a in attrs)
                {
                    a.AddComponents(gameObject);
                }
               
				return;
			}

			var attr2s =  ZPropertyAttributeTools.GetAttributesExcludeListClass<PropertyAddComponentClassAttribute> (property);
			if (attr2s != null) {

                foreach (var a in attr2s)
                {
                    if (groups == null || groups.FindIndex(g => string.Compare(g, a.Group) == 0) >= 0)
                    {
                        a.AddComponents(gameObject);
                    }
                }
            }
		}

        //only bind PropertyAddComponentClassAttribute for List like ZProperty
        static private void AddPropertyClassComponents(IZProperty property, GameObject gameObject, List<string> groups = null)
        {
            var propType = property.GetType();
            if (propType == null)
            {
                return;
            }

             var attrs = ZPropertyAttributeTools.GetTypeAttributes<PropertyAddComponentClassAttribute>(propType);
            //add Component
            //var attrs = ZPropertyAttributeTools.GetAttributes<PropertyAddComponentAttribute>(property);
            if (attrs != null)
            {

                foreach (var a in attrs)
                {
                    if (groups == null || groups.FindIndex(g => string.Compare(g, a.Group) == 0) >= 0)
                    {
                        a.AddComponents(gameObject);
                    }
                }

                return;
            }
        }

        static private void AddComponents(object obj, GameObject gameObject, List<string> groups = null)
        {
            var attrs = ZPropertyAttributeTools.GetTypeAttributes<PropertyAddComponentClassAttribute>(obj.GetType());
            //add Component
            //var attrs = ZPropertyAttributeTools.GetAttributes<PropertyAddComponentAttribute>(property);
            if (attrs != null)
            {

                foreach (var a in attrs)
                {
                    if (groups == null || groups.FindIndex(g => string.Compare(g, a.Group) == 0) >= 0)
                    {
                        a.AddComponents(gameObject);
                    }
                }

                return;
            }
        }

        /// <summary>
        /// Finds the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="name">Name.</param>
        static private Transform FindItem(string name, Transform transform){
            if (String.IsNullOrEmpty(name))
                return null;

			if (transform == null)
				return null;

			var obj = ZViewBuildTools.FindInChilds (transform, name);
			return obj;
			//return obj != null ? obj.GetComponent<ZUIPropertyItemBehaviour> () as IZUIPropertyItem : null;
		}

        static private Transform FindItem(Transform transform, Func<Transform, bool> selector)
        {
            if (transform == null)
                return null;

            var obj = ZViewBuildTools.FindInChilds(transform, selector);
            return obj;
            //return obj != null ? obj.GetComponent<ZUIPropertyItemBehaviour> () as IZUIPropertyItem : null;
        }


        static public void AutoFindAndBindItems(MonoBehaviour view)
        {
            FieldInfo[] typeInfo = view.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var f in typeInfo)
            {
                if (f.FieldType != typeof(Transform))
                {
                    continue;
                }

                var attribute = f.GetCustomAttributes(typeof(PropertyAutoFindAttribute), true)?.FirstOrDefault() as PropertyAutoFindAttribute;

                if (attribute == null)
                {
                    continue;
                }
                string itemName = f.Name;
                if (attribute.IfHasSecondName())
                {
                    itemName = attribute.SecondName;
                }

                var child = FindInChilds(view.transform, itemName);
                if (child != null)
                    f.SetValue(view, child);
            }
        }

        //[TODO]
        static internal void BindCompoent(IZProperty property, Transform transform)
        {
            var attribute = ZPropertyAttributeTools.GetAttribute<PropertyBindCompoentAttribute>(property);

            if (attribute == null)
            {
                return;
            }


        }

        /// <summary>
        /// Creates the user interface item.
        /// </summary>
        /// <returns>The user interface item.</returns>
        /// <param name="prop">Property.</param>
        static private Transform CreateViewItem(IZProperty prop, Transform transform){
			var attr = ZPropertyAttributeTools.GetAttribute<PropertyUIItemResAttribute> (prop);
			if (attr == null)
				attr =  ZPropertyAttributeTools.GetAttribute<PropertyUIItemResClassAttribute> (prop);
			
			if (attr != null) {
				//attr.type
				var prefab = attr.LoadRes(prop);

				var parent =  ZViewBuildTools.FindInChilds (transform, attr.parentNode);
                if (parent == null)
                    parent = transform;

				if (prefab != null) {
					var obj =GameObject.Instantiate (prefab, parent) ;
                    //obj.name = prefab.name;

                    obj.name = prop.PropertyID;

                    return obj != null ? obj.transform : null;
					//return obj != null ? obj.GetComponent<ZUIPropertyItemBehaviour> () as IZUIPropertyItem : null;
				}
                else
                {
                    Debug.LogError("No UIItem Prefab Res " + prop.PropertyID);
                }
            }

			return null;
		}

        /// <summary>
        /// Finds the component in childs.
        /// </summary>
        /// <returns>The component in childs.</returns>
        /// <param name="node">Node.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        static public T FindComponentInChildren<T>(Transform node, string name)
        {
            var tran = FindInChilds(node, name);
            if (tran == null)
                return default(T);

            return tran.GetComponent<T>();
        }

        /// <summary>
        /// Finds the in childs.
        /// </summary>
        /// <returns>The in childs.</returns>
        /// <param name="node">Node.</param>
        /// <param name="name">Name.</param>
        static public Transform FindInChilds(Transform node, string name)
		{
			if (node == null)
				return null;

			Transform ret = node.Find (name);
			if (ret != null)
				return ret;

			for (int i = 0; i < node.childCount; i++) {
				ret = ZViewBuildTools.FindInChilds (node.GetChild (i), name);
				if (ret != null) {
					return ret;
				}
			}

			return null;
		}


        static public Transform FindInChilds(Transform node, Func<Transform, bool> selector)
        {
            if (node == null)
                return null;

            Transform ret = null;
            for (int i = 0; i < node.childCount; i++)
            {
                ret = node.GetChild(i);
                if (selector(ret))
                {
                    return ret;
                }
            }

            //not find
            for (int i = 0; i < node.childCount; i++)
            {
                ret = ZViewBuildTools.FindInChilds(node.GetChild(i), selector);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        /// <summary>
        /// Invokes the bind method.
        /// </summary>
        /// <param name="obj">Object.</param>
        static internal void InvokeBindMethod(object obj, Transform transform){
			Type type = obj.GetType ();
			MethodInfo method = type.GetMethod("OnBind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] {typeof(Transform)});      // 获取方法信息
			object[] parameters = {transform};

			if (method != null)
				method.Invoke(obj, parameters);                           // 调用方法，参数为空
		}

        static internal void InvokeBindMethod(object obj, Transform transform, List<string> groups = null)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnBind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] { typeof(Transform), typeof(List<string>) });      // 获取方法信息
            object[] parameters = { transform, groups};

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static internal void InvokePreBindMethod(object obj, Transform transform)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnPreBind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] { typeof(Transform) });      // 获取方法信息
            object[] parameters = { transform };

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static internal void InvokePreBindMethod(object obj, Transform transform, List<string> groups = null)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnPreBind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] { typeof(Transform), typeof(List<string>) });      // 获取方法信息
            object[] parameters = { transform, groups};

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        /// <summary>
        /// Invokes the unbind method.
        /// </summary>
        /// <param name="obj">Object.</param>
        static internal void InvokePreUnbindMethod(object obj, Transform transform)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnPreUnbind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] { typeof(Transform) });      // 获取方法信息
            object[] parameters = new object[] { transform };

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static internal void InvokePreUnbindMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnPreUnbind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] { typeof(Transform) });      // 获取方法信息
            object[] parameters = new object[] { null };

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static internal void InvokeUnbindMethod(object obj){
			Type type = obj.GetType ();
			MethodInfo method = type.GetMethod("OnUnbind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);//, new Type[] {});      // 获取方法信息
            object[] parameters = new object[] { };

			if (method != null)
				method.Invoke(obj, parameters);                           // 调用方法，参数为空
		}



        /// <summary>
        /// Finds the transform.
        /// </summary>
        /// <returns>The transform.</returns>
        /// <param name="prop">Property.</param>
        static internal Transform FindTransform(IZProperty prop){
            return (prop.TransNode);
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <returns>The property.</returns>
		/// <param name="propID">Property I.</param>
		static public IZProperty GetProperty(Transform transform, string propID){
			object targetObj = null;
			var root = transform.GetComponent<ZPropertyViewRootBehaviour>();

			var item = transform.GetComponent<ZPropertyViewItemBehaviour>();
			if (root != null) {
				targetObj = root.GetRoot ();
			} else if (item != null) {
				targetObj = item.GetProperty ().Value;
			}

			if (targetObj != null) {
				return ZPropertyMesh.GetProperty (targetObj, propID);
			}

			return null;
		}

		/// <summary>
		/// Gets the property by sub.
		/// </summary>
		/// <returns>The property by sub.</returns>
		/// <param name="transform">Transform.</param>
		/// <param name="propID">Property I.</param>
		static public IZProperty GetPropertyInSubs(Transform transform, string propID){
			object targetObj = null;
			var root = transform.GetComponent<ZPropertyViewRootBehaviour>();

			var item = transform.GetComponent<ZPropertyViewItemBehaviour>();
			if (root != null) {
				targetObj = root.GetRoot ();
			} else if (item != null) {
				targetObj = item.GetProperty ().Value;
			}

			if (targetObj != null) {
				return ZPropertyMesh.GetPropertyInSubs (targetObj, propID);
			}

			return null;
		}



        /// <summary>
        /// Changes the color of the image.
        /// </summary>
        static public void ChangeImageColor(Transform root, Color co)
        {
#if ZP_UNITY_CLIENT
            var imgs = root.GetComponentsInChildren<Image>();
            foreach (var i in imgs)
            {
                i.color = co;
            }
#endif
        }

        /// <summary>
        /// Ises the bind.
        /// </summary>
        static public bool IsBind(IZProperty prop, Transform tran)
        {
            bool bret = false;

            if (prop.TransNode != null)
            {

                return prop.TransNode == tran;
            }

            return bret;
        }

        static public bool IsView(Transform transform)
        {
            //UnityEngine.EventSystems.UIBehaviour
            return transform.GetComponent<RectTransform>() != null;
        }

#if ZP_UNITY_CLIENT && DEBUG
        // internal static ZDebugPropertyItem BindDebugItem(Transform trans)
        // {
        //     return trans.gameObject.AddComponent<ZDebugPropertyItem>();
        // }

        // internal static ZDebugEventBehaviour BindDebugEvent(Transform trans)
        // {
        //     return trans.gameObject.AddComponent<ZDebugEventBehaviour>();
        // }
#endif
    }
}

//#endif
