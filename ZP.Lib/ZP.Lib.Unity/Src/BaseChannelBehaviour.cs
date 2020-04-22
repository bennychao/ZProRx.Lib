using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib.Matrix;

public class BaseChannelBehaviour : MonoBehaviour
{
    //ZP.Lib.Matrix.
    //ChannelListener channelListener = null;

    public string ChannelName
    {
        get
        {
            var typeName = this.GetType().ToString();
            var chindex = typeName.IndexOf("ChannelBehaviour");
            return chindex > 0 ? typeName.Substring(0, typeName.Length - 16) : typeName;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
