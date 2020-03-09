using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Domain
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PipelineDefaultServerChannelTypeAttribute : Attribute
    {
        public Type DefaultType { get; }
        public PipelineDefaultServerChannelTypeAttribute(Type type)
        {
            DefaultType = type;
        }
    }
}
