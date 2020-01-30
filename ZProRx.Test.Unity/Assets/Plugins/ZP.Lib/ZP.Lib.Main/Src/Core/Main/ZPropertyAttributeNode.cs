#define Deasdf

using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace ZP.Lib
{

    /// <summary>
    /// save the type property's info
    /// </summary>
    public class ZPropertyAttributeNode
	{
        public string PropertyID { private set; get;}
		public string Name{ private set; get;}
		public string Description{ private set; get;}

		public Type PropertyType{ private set; get;}

        public string SimplePropertyID => "." + info.Name;

        //public Type UIItemType{ set; get;}

        private readonly FieldInfo info;
		private readonly Type parentType;

		public ZPropertyAttributeNode (FieldInfo info, Type parentType)
		{
			this.info = info;
			this.parentType = parentType;
			ParseAttribute ();
		}

		private void ParseAttribute(){
			var des = ZPropertyAttributeTools.GetDescription (info);
			var name = ZPropertyAttributeTools.GetName (info);
            if (name != null)
			    Name = name;

            if (des != null)
			    Description = des;

			var parentName = ZPropertyMesh.GetTypeName (parentType);

			PropertyID = parentName + "." + info.Name;
			PropertyType = info.FieldType;

            //SimplePropertyID = "." + info.Name;

        }

		/// <summary>
		/// Gets the attribute.
		/// </summary>
		/// <returns>The attribute.</returns>
		/// <typeparam name="AttributeType">The 1st type parameter.</typeparam>
		public AttributeType GetAttribute<AttributeType>() where AttributeType : Attribute
		{
			var a = ZPropertyAttributeTools.GetAttribute<AttributeType> (info);
			if (a == null) {
				a = ZPropertyAttributeTools.GetTypeAttribute<AttributeType> (PropertyType);
			}
			return a;
		}


        public List<AttributeType> GetAttributes<AttributeType>() where AttributeType : Attribute
        {
            var a = ZPropertyAttributeTools.GetAttributes<AttributeType>(info);
            if (a == null)
            {
                a = ZPropertyAttributeTools.GetTypeAttributes<AttributeType>(PropertyType);
            }
            return a;
        }

        public List<AttributeType> GetPropertyAttributes<AttributeType>() where AttributeType : Attribute
        {
            var a = ZPropertyAttributeTools.GetAttributes<AttributeType>(info);
            return a;
        }

        public bool IsDefined<AttributeType>() where AttributeType : Attribute
        {
            return info.IsDefined(typeof(AttributeType));
        }

        public bool IsValueAnchorSupportl()
        {
            return !PropertyType.IsValueType;
        }

        /// <summary>
        /// Gets the type attribute.
        /// </summary>
        /// <returns>The type attribute.</returns>
        /// <typeparam name="AttributeType">The 1st type parameter.</typeparam>
        public AttributeType GetTypeAttribute<AttributeType>() where AttributeType : Attribute
		{
			return ZPropertyAttributeTools.GetTypeAttribute<AttributeType> (PropertyType);
		}
	}
}

