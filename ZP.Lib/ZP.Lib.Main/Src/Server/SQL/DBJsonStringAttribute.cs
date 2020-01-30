using System;
namespace ZP.Lib.Server.SQL
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class DBJsonStringAttribute : Attribute
    {
        //public string Name = "";
        public DBJsonStringAttribute()
        {
            //Name = name;
        }
    }
}
