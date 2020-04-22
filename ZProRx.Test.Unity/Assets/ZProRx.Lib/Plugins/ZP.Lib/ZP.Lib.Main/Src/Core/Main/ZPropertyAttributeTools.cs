using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZP.Lib
{
	/// <summary>
	/// Property tools.
	/// </summary>
	public static class ZPropertyAttributeTools{

		static public string GetName(MemberInfo info){
			//var type = typeof(PropertyNameAttribute);

			var attribute = info.GetCustomAttributes(typeof(PropertyDescriptionAttribute), true)?.FirstOrDefault();

			if (attribute == null)
			{
				return null;
			}

			return ((PropertyDescriptionAttribute)attribute).name;
		}


		static public string GetDescription(MemberInfo info){
			//var type = typeof(PropertyDescriptionAttribute);

			var attribute = info.GetCustomAttributes(typeof(PropertyDescriptionAttribute), true)?.FirstOrDefault();

			if (attribute == null)
			{
				return null;
			}

			return ((PropertyDescriptionAttribute)attribute).description;
		}

		/// <summary>
		/// Gets the type attribute.
		/// </summary>
		/// <returns>The type attribute.</returns>
		/// <param name="type">Type.</param>
		/// <typeparam name="AttributeType">The 1st type parameter.</typeparam>
		static public AttributeType GetTypeAttribute<AttributeType>(Type type) where AttributeType : Attribute
		{
            if (!type.IsDefined(typeof(AttributeType)))
                return null;

			var attribute =  type.GetCustomAttributes(typeof(AttributeType), false)?.FirstOrDefault();

			if (attribute == null)
			{
				return null;
			}

			return (attribute as AttributeType);
		}

        static public List<AttributeType> GetTypeAttributes<AttributeType>(Type type) where AttributeType : Attribute
        {
            if (!type.IsDefined(typeof(AttributeType)))
                return null;

            var attributes = type.GetCustomAttributes(typeof(AttributeType), false).Select(o => o as AttributeType).ToList<AttributeType>();

            if (attributes == null)
            {
                return null;
            }

            return (attributes);
        }

        static public AttributeType GetAttribute<AttributeType>(MemberInfo info) where AttributeType : Attribute{
            //var type = typeof(PropertyDescriptionAttribute);

            if (!info.IsDefined(typeof(AttributeType)))
                return null;

			var attribute = info.GetCustomAttributes(typeof(AttributeType), true)?.FirstOrDefault();

			if (attribute == null)
			{
				return null;
			}

			return (attribute as AttributeType);
		}

        static public List<AttributeType> GetAttributes<AttributeType>(MemberInfo info) where AttributeType : Attribute
        {
            //var type = typeof(PropertyDescriptionAttribute);

            if (!info.IsDefined(typeof(AttributeType)))
                return null;

            var attributes = info.GetCustomAttributes(typeof(AttributeType), true).Select(o => o as AttributeType).ToList<AttributeType>();

            if (attributes == null)
            {
                return null;
            }

            return attributes;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        static public AttributeType GetAttribute<AttributeType>(IZProperty property) where AttributeType : Attribute{
			//var type = typeof(PropertyDescriptionAttribute);

			var attr = property.AttributeNode.GetAttribute<AttributeType> ();
			if (attr != null) {
				
				return attr;
			}

            //support ZProperty's Attribute define
			var propType = property.GetDefineType ();
			if (propType != null) {
				attr = ZPropertyAttributeTools.GetTypeAttribute<AttributeType> (propType);
			}

			if (attr != null) {

				return attr;
			}

			if (!propType.IsValueType && property.Value != null) {
				var valueType = property.Value.GetType ();
				attr = ZPropertyAttributeTools.GetTypeAttribute<AttributeType> (valueType);
			}

			return attr;
		}


        static public List<AttributeType> GetAttributes<AttributeType>(IZProperty property) where AttributeType : Attribute
        {
            //var type = typeof(PropertyDescriptionAttribute);

            var attr = property.AttributeNode.GetAttributes<AttributeType>();
            if (attr != null)
            {
                return attr;
            }

            var propType = property.GetDefineType();
            if (propType != null)
            {
                attr = ZPropertyAttributeTools.GetTypeAttributes<AttributeType>(propType);
            }

            if (attr != null)
            {

                return attr;
            }

            if (!propType.IsValueType && property.Value != null)
            {
                var valueType = property.Value.GetType();
                attr = ZPropertyAttributeTools.GetTypeAttributes<AttributeType>(valueType);
            }

            return attr;
        }

        static internal List<AttributeType> GetAttributesExcludeListClass<AttributeType>(IZProperty property) where AttributeType : Attribute
        {
            //var type = typeof(PropertyDescriptionAttribute);
            List<AttributeType> attr = null;
            if (ZPropertyMesh.IsListItem(property))
            {
                attr = property.AttributeNode.GetPropertyAttributes<AttributeType>();
                if (attr != null)
                {
                    return attr;
                }
            }
            else
            {
                //get property and  attributes
                attr = property.AttributeNode.GetAttributes<AttributeType>();
                if (attr != null)
                {
                    return attr;
                }
            }


            var propType = property.GetDefineType();
            if (propType != null)
            {
                attr = ZPropertyAttributeTools.GetTypeAttributes<AttributeType>(propType);
            }

            if (attr != null)
            {

                return attr;
            }

            if (!propType.IsValueType && property.Value != null)
            {
                var valueType = property.Value.GetType();
                attr = ZPropertyAttributeTools.GetTypeAttributes<AttributeType>(valueType);
            }

            return attr;
        }

        /// <summary>
        /// Determines if is default to value the specified prop.
        /// </summary>
        public static bool IsDefaultToValue(IZProperty prop){
			var attr = ZPropertyAttributeTools.GetAttribute<PropertyDefaultToValueAttribute> (prop as IZProperty);
			return prop as IRuntimable != null && attr != null;
		}

		//		static public ReturnType GetDescription<AttributeType, ReturnType>(bool bInherited){
		//			var type = typeof(AttributeType);
		//
		//			var attribute = type.GetCustomAttributes(typeof(PropertyDescriptionAttribute), bInherited).FirstOrDefault();
		//
		//			if (attribute == null)
		//			{
		//				return null;
		//			}
		//
		//			return ((PropertyDescriptionAttribute)attribute).description;
		//		}
	}
}

