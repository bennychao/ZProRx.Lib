using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class ZServerBehaviour
#if ZP_UNITY_SERVER
    : MonoBehaviour
#endif
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if !ZP_UNITY_SERVER

#endif
}
