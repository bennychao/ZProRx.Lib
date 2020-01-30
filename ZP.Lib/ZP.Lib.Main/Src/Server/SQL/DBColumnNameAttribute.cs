using System;
namespace ZP.Lib.Server.SQL
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class DBColumnNameAttribute : Attribute
    {
        public string Name = "";
        public DBColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
