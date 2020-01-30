using System;
namespace ZP.Lib.Server.SQL
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class DBExcludeAttribute : Attribute
    {
        //public string Name = "";
        public DBExcludeAttribute()
        {
            //Name = name;
        }
    }

}
