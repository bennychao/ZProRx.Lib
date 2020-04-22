using System;
namespace ZP.Lib.Server.SQL
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class DBIncludeAttribute : Attribute
    {
        //public string Name = "";
        public DBIncludeAttribute()
        {
            //Name = name;
        }
    }
}
