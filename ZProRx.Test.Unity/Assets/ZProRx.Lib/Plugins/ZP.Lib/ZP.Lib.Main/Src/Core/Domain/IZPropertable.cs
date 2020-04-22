using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZP.Lib
{
    public interface IZPropertable
    {
        string PropertyID { get; }
        ZPropertyAttributeNode AttributeNode { set; get; }
        Transform TransNode { set; get; }
    }

}

