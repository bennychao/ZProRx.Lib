using System;
namespace ZP.Lib.Server.SQL
{
    //must be uint type
    [AttributeUsage(AttributeTargets.Field
        | AttributeTargets.Property
        | AttributeTargets.Class, Inherited = true)]
    public class DBIndexAttribute : Attribute
    {
        bool bPrimary = true;

        public bool IsPrimary => bPrimary;

        public DBIndexAttribute(bool bPrimary = false)
        {
            this.bPrimary = bPrimary;
        }

        public string IndexName { get; set; }
        public DBIndexAttribute(string indexName)
        {
            this.bPrimary = false;
            IndexName = indexName;
        }
    }

}
