using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;
using UnityEngine;
using ZP.Lib.Core.Domain;
using ZP.Lib.Common;

namespace ZP.Lib
{
    public static partial class ZPropertyMesh
    {

        static public bool IsPartProperty(IZProperty prop)
        {
            return prop as IZPartProperty != null;
        }

        static internal bool IsEvent(IZPropertable prop)
        {
            return prop is IZEvent;
        }

        static internal bool IsProperty(IZPropertable prop)
        {
            return prop is IZProperty;
        }

        static public bool IsValueType(Type ptype)
        {
            return( ptype.IsValueType || ptype == typeof(string) ) && (!IsNullValue(ptype) && !IsUnitValue(ptype));
        }

        static internal bool IsProperty(object prop)
        {
            return prop is IZProperty;
        }
        static internal bool IsNullValue(IZProperty prop)
        {
            return prop.Value is ZNull;
        }

        static internal bool IsUnitValue(IZProperty prop)
        {
            return prop.Value is Unit;
        }


        static internal bool IsNullValue(object obj)
        {
            return obj is ZNull;
        }

        static internal bool IsNullValue(Type ptype)
        {
            return ptype == typeof( ZNull);
        }

        static internal bool IsUnitValue(Type ptype)
        {
            return ptype == typeof(Unit);
        }

        /// <summary>
        /// Determines if is propertable the specified type.
        /// </summary>
        static public bool IsPropertable(Type type)
        {
            return types.ContainsKey(type.FullName);
        }

        static public bool IsPropertable(object obj)
        {
            return types.ContainsKey(obj.GetType().FullName);
        }


        static internal bool IsRecoverable(IZProperty prop)
        {
            return prop as IRecoverable != null;
        }

        /// <summary>
        /// Determines if is propertable low AP the specified type.
        /// </summary>
        /// <returns><c>true</c> if is propertable low AP the specified type; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
        static public bool IsPropertableLowAPI(Type type)
        {
            FieldInfo[] typeInfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return typeInfo.Where(a => 
                    typeof(IZProperty).IsAssignableFrom(a.FieldType) ||
                    typeof(IZEvent).IsAssignableFrom(a.FieldType)
                )
                .Count() > 0;
        }

        /// <summary>
        /// Determines if is property list the specified prop.
        /// </summary>
        /// <returns><c>true</c> if is property list the specified prop; otherwise, <c>false</c>.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsPropertyList(IZProperty prop)
        {
            return prop as IZPropertyList != null;
        }

        /// <summary>
        /// Ises the property reference list.
        /// </summary>
        /// <returns><c>true</c>, if property reference list was ised, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsPropertyRefList(IZProperty prop)
        {
            return prop as IZPropertyRefList != null;
        }

        /// <summary>
        /// Ises the property list like.判断是IsPropertyList or IsPropertyRefList
        /// </summary>
        /// <returns><c>true</c>, if property list like was ised, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsPropertyListLike(IZProperty prop)
        {
            return IsPropertyRefList(prop) || IsPropertyList(prop);
        }


        static internal bool IsPropertyListLike(Type type)
        {
            return IsPropertyList(type) || IsPropertyRefList(type);
        }

        static internal bool IsPropertyList(Type type)
        {
            return typeof(IZPropertyList).IsAssignableFrom(type);
        }

        static internal bool IsPropertyRefList(Type type)
        {
            return typeof(IZPropertyRefList).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines if is rankable the specified prop.
        /// </summary>
        /// <returns><c>true</c> if is rankable the specified prop; otherwise, <c>false</c>.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsRankable(IZProperty prop)
        {
            return prop as IRankable != null;
        }

        /// <summary>
        /// Ises the rankable.
        /// </summary>
        /// <returns><c>true</c>, if rankable was ised, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        static internal bool IsRankable(Type type)
        {
            return typeof(IRankable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Ises the reference.
        /// </summary>
        /// <returns><c>true</c>, if reference was ised, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsRef(IZProperty prop)
        {
            return prop as IRefable != null;
        }

        /// <summary>
        /// Ises the reference.
        /// </summary>
        /// <returns><c>true</c>, if reference was ised, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        static internal bool IsRef(Type type)
        {
            return typeof(IRefable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Ises the enum.
        /// </summary>
        /// <returns><c>true</c>, if enum was ised, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsEnum(IZProperty prop)
        {
            return prop.GetDefineType().IsEnum;
        }

        static internal bool IsMultiEnum(Type type)
        {
            return typeof(IMultiEnumerable).IsAssignableFrom(type);
        }

        static internal bool IsMultiEnum(IZProperty prop)
        {
            return typeof(IMultiEnumerable).IsAssignableFrom(prop.GetDefineType());
        }

        /// <param name="prop">Property.</param>
        static public bool IsRuntimable(IZProperty prop)
        {
            return prop as IRuntimable != null;// && prop.AttributeNode.GetAttribute<PropertyRuntimeAttribute> () != null;
        }

        static internal bool IsIndexable(Type type)
        {
            return typeof(IIndexable).IsAssignableFrom(type);
        }

        static public bool IsRawDataRef(Type type)
        {
            return typeof(IRawDataPref).IsAssignableFrom(type);
        }

        static public bool IsRawDataRef(IZProperty p)
        {
            return typeof(IRawDataPref).IsAssignableFrom(p.GetDefineType());
        }

        /// <summary>
        /// Ises the define interface.T是否为接口
        /// </summary>
        /// <returns><c>true</c>, if define interface was ised, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        static internal bool IsDefineInterface(IZProperty prop)
        {
            return prop.GetDefineType().IsInterface;
        }


        static public bool IsDirectLinkable(IZEvent ze){
            return (ze as IDirectLinkable) != null;
        }


        /// <summary>
        /// Ises the linkable.
        /// </summary>
        /// <returns><c>true</c>, if linkable was ised, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        static public bool IsLinkable(IZProperty prop)
        {
            return prop as IZLinkable != null;
        }

        static public bool IsLinkable(IZEvent prop)
        {
            return prop as IZLinkable != null;
        }

        static public bool IsLinkedPropertyList(IZProperty prop)
        {
            var link = (prop as IZLinkable).LinkProperty;
            return IsLinkable(prop) && link != null && IsPropertyList(link);
        }

        static public bool IsLinkedRecoveriable(IZProperty prop)
        {
            var link = (prop as IZLinkable).LinkProperty;
            return IsLinkable(prop) && link != null && IsRecoverable(link);
        }

        static public bool IsCalculable<T>(Type type)
        {
            return typeof(ICalculable<T>).IsAssignableFrom(type);
        }
        static public bool IsCalculable<T>(object obj)
        {
            return typeof(ICalculable<T>).IsAssignableFrom(obj.GetType());
        }

        static public bool IsListItem(IZProperty prop)
        {
            return IsPropertyListLike(prop.AttributeNode.PropertyType) && !IsPropertyListLike(prop);
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        static internal string GetTypeName(Type parentType)
        {
            var parentName = parentType.Name;
            int index = parentName.IndexOf("`");
            if (index > 0)
                parentName = parentName.Substring(0, index);

            return parentName;
        }

        static internal Type GetPropertyType(string propertyId)
        {
            var parentName = propertyId;
            int index = parentName.IndexOf("`");
            if (index > 0)
                parentName = parentName.Substring(0, index);

            return Type.GetType(parentName);
        }
    }
}

