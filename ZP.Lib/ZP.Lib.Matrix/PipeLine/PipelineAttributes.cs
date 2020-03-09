using System;
namespace ZP.Lib.Matrix
{

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class AgentTypeAttribute : Attribute
    {
        public Type AgentType { get; }
        public AgentTypeAttribute(Type type)
        {
            AgentType = type;
        }
    }
}
