using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZP.Lib
{
    public static partial class ZPropertyMesh
    {
        /// <summary>
        /// Gets the events.
        /// </summary>
        static public List<IZEvent> GetEvents(object obj)
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZEvent;
                return prop;
            }).Where(a => a != null).ToList<IZEvent>();
        }


        static public IZEvent GetEvent(object obj, string propID)
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZEvent;
                return prop;
            }).Where(a => a != null && (a.PropertyID.CompareTo(propID) == 0 || a.PropertyID.Contains(propID)))?.FirstOrDefault();
        }

        static public object GetEventValue(IZEvent zEvent)
        {
            return (zEvent as IZEventWithParam)?.CurValue;
        }

        static public T GetEventValue<T>(IZEvent<T> zEvent)
        {
            return zEvent.CurValue;
        }

        //support ".*.xxx     .xxx.xxx"
        static public List<IZEvent> GetEventsEx(object obj, string multiPropID)
        {
            //multiPropID.IndexOf()
            List<IZEvent> rets = new List<IZEvent>();

            var subs = multiPropID.Split('*');

            if (subs.Length == 1)
            {
                var e = GetEventEx(obj, subs[0]);
                if (e != null)
                    rets.Add(e);

                return rets;
            }

            var parentProp = GetPropertyEx(obj, subs[0].Trim('.'));

            if (parentProp == null)
                return rets;

            //add subs
            if (ZPropertyMesh.IsPropertyList(parentProp))
            {
                rets.AddRange(ZPropertyMesh.GetEventsEx(parentProp as IZPropertyList, subs[1]));
            }
            else if (ZPropertyMesh.IsPropertyRefList(parentProp))
            {
                rets.AddRange(ZPropertyMesh.GetEventsEx(parentProp as IZPropertyRefList, subs[1]));
            }
            else if (parentProp.Value != null)
            {
                var subProps = ZPropertyMesh.GetProperties(parentProp.Value);

                rets.AddRange(ZPropertyMesh.GetEventsEx(subProps, subs[1]));
            }

            return rets;
        }




        static public IZEvent GetEventEx(object obj, string multiPropID)
        {
            if (obj == null)
                return null;

            if (multiPropID.Length == 0)
            {
                return null;
            }

            var subs = multiPropID.Split('.');

            if (subs.Length == 1)
            {
                return GetEvent(obj, subs[0]);
            }

            int changelen = 0;
            //subs.length > 1
            string propID = "";
            if (subs[0].Length == 0) //"" empty string
            {
                subs[0] = GetTypeName(obj.GetType());
                changelen = subs[0].Length;
            }

            propID = subs[0] + "." + subs[1];



            var LeftPropID = multiPropID.Substring(propID.Length - changelen);

            if (LeftPropID.Length <= 0)
            {
                return GetEvent(obj, propID);
            }

            //find next property
            IZProperty pRet = GetProperty(obj, propID);

            if (pRet == null)
                return null;

            return GetEventEx(pRet.Value, LeftPropID);

        }

        static public IZEvent<T> GetEventEx<T>(object obj, string multiPropID)
        {
            return GetEventEx(obj, multiPropID) as IZEvent<T>;
        }


        static public List<IZEvent> GetEventsEx<T>(IEnumerable<T> list, string multiPropID)
        {
            List<IZEvent> rets = new List<IZEvent>();

            foreach (var item in list)
            {
                IZEvent p = null;
                if (ZPropertyMesh.IsProperty(item))
                {
                    p = GetEventEx((item as IZProperty)?.Value, multiPropID);
                    if (p != null)
                        rets.Add(p);
                }
                else
                {
                    p = GetEventEx(item, multiPropID);
                    if (p != null)
                        rets.Add(p);
                }
            }

            return rets;
        }

        static public List<IZEvent> GetEventsEx(IEnumerable list, string multiPropID)
        {
            List<IZEvent> rets = new List<IZEvent>();

            foreach (var item in list)
            {
                IZEvent p = null;
                if (ZPropertyMesh.IsProperty(item))
                {
                    p = GetEventEx((item as IZProperty)?.Value, multiPropID);
                    if (p != null)
                        rets.Add(p);
                }
                else
                {
                    p = GetEventEx(item, multiPropID);
                    if (p != null)
                        rets.Add(p);
                }

            }

            return rets;
        }

        static public List<IZEvent> GetEventsInSubs(object obj)
        {
            List<IZEvent> retList = new List<IZEvent>();
            var propList = GetProperties(obj);

            retList.AddRange(GetEvents(obj));

            var subList = new List<IZEvent>();
            foreach (var s in propList)
            {
                if (ZPropertyMesh.IsPropertyList(s))
                {
                    var valueList = (s as IZPropertyList).PropList.Select(p => p.Value).Where(v => ZPropertyMesh.IsPropertable(v));
                    foreach (var v in valueList)
                    {
                        subList.AddRange(GetEventsInSubs(v));
                    }
                }
                else if (ZPropertyMesh.IsPropertyRefList(s))
                {
                    var valueList = (s as IZPropertyRefList).PropList.Select(p => p.Value).Where(v => ZPropertyMesh.IsPropertable(v));
                    foreach (var v in valueList)
                    {
                        subList.AddRange(GetEventsInSubs(v));
                    }

                }
                else
                {
                    subList.AddRange(GetEventsInSubs(s.Value));
                }

            }

            retList.AddRange(subList);
            return retList;
        }

        /// <summary>
        /// Gets the property I ds.这个只是获取编辑时（非运行时）Type树中的PropertyID，
        /// 对于Interface结点在运行时绑定的对象，无法获取到相应对象
        /// 的PropertyID.
        /// </summary>
        static public List<string> GetPropertyIDs(Type type)
        {
            List<string> rets = new List<string>();

            FieldInfo[] typeInfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int j = 0; j < typeInfo.Length; j++)
            {

                if (typeInfo[j].MemberType == MemberTypes.Field)
                {

                    //if (typeInfo [j].IsDefined (typeof(ZP.Lib.PropertyDescriptionAttribute), tr     /// <summary>

                    if (typeof(IZProperty).IsAssignableFrom(typeInfo[j].FieldType))
                    {

                        string propertyID = GetTypeName(type) + "." + typeInfo[j].Name;

                        rets.Add(propertyID);

                        continue;
                    }
                    else if (ZPropertyMesh.IsPropertableLowAPI(typeInfo[j].FieldType))
                    {

                        rets.AddRange(GetPropertyIDs(typeInfo[j].FieldType));
                        continue;
                    }
                }
            }

            return rets;
        }



        static public string GetPropertyIdWithAttribute<A>(Type t) where A : Attribute
        {
            FieldInfo[] typeInfo = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var subId = typeInfo.Where(a => a != null && a.GetCustomAttribute<A>() != null)?.ToList()?.FirstOrDefault()?.Name;

            return string.IsNullOrEmpty(subId) ? "" : ZPropertyMesh.GetTypeName(t) + "." + subId;
        }

        
        //------------ property access functions ------------------------------------

        static public IZProperty GetProperty(object obj, string propID)
        {
            if (!IsPropertable(obj.GetType()))
                return null;


            if (propID.Count() == 0)
                return null;

            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZProperty;
                return prop;
            }).Where(a => a != null && (a.PropertyID.CompareTo(propID) == 0 || a.PropertyID.Contains(propID)))?.FirstOrDefault();
        }

        static public T GetProperty<T>(object obj, string propID)
        {
            return (GetProperty(obj, propID) as IZProperty<T>).Value;
        }


        static public IZProperty GetProperty(object obj, Type type)
        {
            return GetProperties(obj, type)?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <returns>The property.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="propID">Property I.</param>
        static public IZProperty GetProperty(IZProperty propParent, string propID)
        {

            if (propID.Count() == 0)
                return null;

            if (propParent.Value == null)
                return null;

            if (!IsPropertable(propParent.Value.GetType()))
                return null;

            FieldInfo[] typeInfo = propParent.Value.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(propParent.Value) as IZProperty;
                return prop;
            }).Where(a => a != null && (a.PropertyID.CompareTo(propID) == 0 || a.PropertyID.Contains(propID)))?.FirstOrDefault();
        }

        static public T GetProperty<T>(IZProperty propParent, string propID)
        {
            return (GetProperty(propParent, propID) as IZProperty<T>).Value;
        }

        /// <summary>
        /// Gets the property in subs.
        /// </summary>
        /// <returns>The property by sub.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="propID">Property I.</param>
        static public IZProperty GetPropertyInSubs(object obj, string propID)
        {
            IZProperty ret = GetProperty(obj, propID);

            if (ret != null)
                return ret;

            var propList = GetProperties(obj);

            foreach (var p in propList)
            {

                if (ZPropertyMesh.IsPropertyListLike(p))
                {
                    foreach (var sub in p as IEnumerable)
                    {
                        ret = GetPropertyInSubs(sub, propID);

                        if (ret != null)
                            return ret;
                    }
                    continue;
                }

                if (p.Value == null)
                    continue;

                ret = GetPropertyInSubs(p.Value, propID);

                if (ret != null)
                    return ret;
            }

            return ret;
        }


        static public IZProperty GetPropertyWithAttribute<A>(object obj) where A : Attribute
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZProperty;
                return prop;
            })?.Where(a => a != null && a.AttributeNode.GetAttribute<A>() != null)?.ToList<IZProperty>()?.FirstOrDefault();
        }

        static public IZProperty GetPropertyEx(IZProperty property, string multiPropID)
        {
            return GetPropertyEx(property?.Value, multiPropID);
        }

        static public IZProperty GetPropertyEx(object obj, string multiPropID)
        {
            if (obj == null)
                return null;

            if (multiPropID.Length == 0)
            {
                return null;
            }

            var subs = multiPropID.Split('.');

            if (subs.Length == 1)
            {
                return GetProperty(obj, subs[0]);
            }

            int changelen = 0;
            //subs.length > 1
            string propID = "";
            if (subs[0].Length == 0) //"" empty string
            {
                subs[0] = GetTypeName(obj.GetType());
                changelen = subs[0].Length;
            }

            propID = subs[0] + "." + subs[1];

            IZProperty pRet = GetProperty(obj, propID);

            if (pRet == null)
                return null;

            var LeftPropID = multiPropID.Substring(propID.Length - changelen);

            if (LeftPropID.Length <= 0)
            {
                return pRet;
            }

            return GetPropertyEx(pRet.Value, LeftPropID);

        }

        static public T GetPropertyEx<T>(object obj, string multiPropID)
        {
            var prop = GetPropertyEx(obj, multiPropID);
            return prop != null ? (T)prop.Value : default(T);
        }


        // properties functions  -----------------------------------------------------------------------------

        /// <summary>
        /// get properties which type is type or derive form type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public List<IZProperty> GetProperties(object obj, Type type)
        {
            if (!IsPropertable(obj.GetType()))
                return null;

            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a =>
            {
                var prop = a.GetValue(obj) as IZProperty;
                return prop;
            }).Where(a => a != null && (type.IsAssignableFrom(a.GetDefineType())))
            .ToList();
            //.FirstOrDefault();
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        static public List<IZProperty> GetProperties(object obj)
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);   

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZProperty;
                return prop;
            }).Where(a => a != null).ToList<IZProperty>();
        }

        
        /// <summary>
        /// Gets the sub objects.
        /// </summary>
        /// <returns>The sub objects.</returns>
        /// <param name="obj">Object.</param>
        static public List<IZProperty> GetPropertiesWithPropertyable(object obj)
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZProperty;
                return prop;
            }).Where(a => a != null
                && IsPropertableLowAPI(a.GetDefineType())
                //              && !a.GetValueType().IsValueType
                //              && !a.GetValueType().IsInterface
                && !IsPropertyList(a)
            ).ToList<IZProperty>(); // 
        }

        /// <summary>
        /// Gets the rankable property by sub.
        /// </summary>
        /// <returns>The rankable property by sub.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="propID">Property I.</param>
        static public List<IRankable> GetPropertiesWithRankable(object obj)
        {

            List<IRankable> ranks = new List<IRankable>();

            var propList = GetProperties(obj);

            foreach (var p in propList)
            {
                if (!ZPropertyMesh.IsPropertyListLike(p) && p.Value == null)
                    continue;

                var r = p as IRankable;
                if (r == null)
                    continue;

                ranks.Add(r);
            }

            return ranks;
        }
                
        /// <summary>
        /// Gets the rankable properties by sub.
        /// </summary>
        /// <returns>The rankable properties by sub.</returns>
        /// <param name="obj">Object.</param>
        static public List<IRankable> GetPropertiesWithRankableInSubs(object obj)
        {

            List<IRankable> ranks = GetPropertiesWithRankable(obj);

            var propList = GetProperties(obj);

            foreach (var p in propList)
            {

                if (ZPropertyMesh.IsPropertyListLike(p))
                {
                    foreach (var sub in p as IEnumerable)
                    {
                        ranks.AddRange(GetPropertiesWithRankableInSubs(sub));
                    }
                    continue;
                }

                if (p.Value == null)
                    continue;

                ranks.AddRange(GetPropertiesWithRankableInSubs(p.Value));
            }

            return ranks;
        }

        
        /// <summary>
        /// Gets the all properties and sub(child) properties.
        /// </summary>
        static public List<IZProperty> GetPropertiesInSubs(object obj)
        {
            var retList = GetProperties(obj);

            var subList = new List<IZProperty>();
            foreach (var s in retList)
            {
                if (ZPropertyMesh.IsPropertyList(s))
                {
                    var valueList = (s as IZPropertyList).PropList.Select(p=> p.Value).Where(v => ZPropertyMesh.IsPropertable(v));
                    foreach (var v in valueList)
                    {
                        subList.AddRange(GetProperties(v));
                    }

                }
                else if (ZPropertyMesh.IsPropertyRefList(s))
                {
                    var valueList = (s as IZPropertyRefList).PropList.Select(p => p.Value).Where(v => ZPropertyMesh.IsPropertable(v));
                    foreach (var v in valueList)
                    {
                        subList.AddRange(GetProperties(v));
                    }

                }
                else
                {
                    subList.AddRange(GetProperties(s.Value));
                }
            }

            retList.AddRange(subList);
            return retList;
        }

        static public List<IZProperty> GetPropertiesWithAttribute<A>(object obj) where A : Attribute
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZProperty;
                return prop;
            })?.Where(a => a != null && a.AttributeNode.GetAttribute<A>() != null)?.ToList<IZProperty>();
        }

        //static public IZProperty<T> GetPropertyEx<T>(object obj, string multiPropID)
        //{
        //    return GetPropertyEx(obj, multiPropID) as IZProperty<T>;
        //}

        static public List<IZProperty> GetPropertiesEx(object obj, string multiPropID)
        {
            //multiPropID.IndexOf()
            List<IZProperty> rets = new List<IZProperty>();

            var subs = multiPropID.Split('*');

            if (subs.Length == 1)
            {
                var e = GetPropertyEx(obj, subs[0]);
                if (e != null)
                    rets.Add(e);

                return rets;
            }

            var parentProp = GetPropertyEx(obj, subs[0].Trim('.'));

            if (parentProp == null)
                return rets;

            //add subs
            if (ZPropertyMesh.IsPropertyListLike(parentProp))
            {
                rets.AddRange(ZPropertyMesh.GetPropertiesEx(parentProp as IEnumerable, subs[1]));
            }
            else if (parentProp.Value != null)
            {
                var subProps = ZPropertyMesh.GetProperties(parentProp.Value);

                rets.AddRange(ZPropertyMesh.GetPropertiesEx(subProps, subs[1]));
            }

            return rets;
        }

        static public List<IZProperty<T>> GetPropertiesEx<T>(object obj, string multiPropID)
        {
            return GetPropertiesEx(obj, multiPropID).Select(t => t as IZProperty<T>).ToList();
        }

        static public List<IZProperty<T>> GetPropertiesEx<T>(object obj, string multiPropID, Func<T, bool> itemSelector)
        {
            return GetPropertiesEx(obj, multiPropID).Select(t => t as IZProperty<T>).Where(t => itemSelector(t.Value)).ToList();
        }

        //get list's item's property
        static public List<IZProperty> GetPropertiesEx(IEnumerable list, string multiPropID)
        {
            List<IZProperty> rets = new List<IZProperty>();

            foreach (var item in list)
            {
                var p = GetPropertyEx((item as IZProperty).Value, multiPropID);
                if (p != null)
                    rets.Add(p);
            }

            return rets;
        }

        static public List<IZProperty> GetPropertiesEx<IT>(IEnumerable<IT> list, string multiPropID, Func<IT, bool> itemSelector)
        {
            List<IZProperty> rets = new List<IZProperty>();

            foreach (var item in list)
            {
                if (!itemSelector(item))
                    continue;

                var p = GetPropertyEx(item, multiPropID);
                if (p != null)
                    rets.Add(p);
            }

            return rets;
        }

        static public List<IZProperty> GetPropertiesEx<T>(IEnumerable<T> list, string multiPropID)
        {
            List<IZProperty> rets = new List<IZProperty>();

            foreach (var item in list)
            {
                var p = GetPropertyEx(item, multiPropID);
                if (p != null)
                    rets.Add(p);
            }

            return rets;
        }




        static public IZProperty FindPropertyInSubs(object obj, Func<IZProperty, bool> matcher)
        {
            IZProperty ret = null;

            var propList = GetProperties(obj);

            foreach (var p in propList)
            {

                if (ZPropertyMesh.IsPropertyListLike(p))
                {
                    foreach (var subProp in (p as IZPropertyList).PropList)
                    {
                        ret = FindPropertyInSubs(subProp, matcher);

                        if (ret != null)
                            return ret;
                    }
                    continue;
                }

                if (p.Value == null)
                    continue;

                ret = FindPropertyInSubs(p, matcher);

                if (ret != null)
                    return ret;
            }

            return ret;
        }

        static public IZProperty FindPropertyInSubs(IZProperty property, Func<IZProperty, bool> matcher)
        {
            IZProperty ret = null;

            if (matcher(property))
                return property;

            var propList = GetProperties(property.Value);

            foreach (var p in propList)
            {

                if (ZPropertyMesh.IsPropertyListLike(p))
                {
                    foreach (var subProp in (p as IZPropertyList).PropList)
                    {
                        ret = FindPropertyInSubs(subProp, matcher);

                        if (ret != null)
                            return ret;
                    }
                    continue;
                }

                if (p.Value == null)
                    continue;

                ret = FindPropertyInSubs(p, matcher);

                if (ret != null)
                    return ret;
            }

            return ret;
        }





        //inner functions

        //get event property and setting elements
        static internal List<IZPropertable> GetPropElements(object obj)
        {
            FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Select(a => {
                var prop = a.GetValue(obj) as IZPropertable;
                return prop;
            }).Where(a => a != null).ToList<IZPropertable>();
        }
    }
}

