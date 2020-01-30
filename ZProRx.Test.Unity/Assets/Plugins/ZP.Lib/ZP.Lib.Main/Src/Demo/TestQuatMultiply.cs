using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;

public class TestQuatMultiply : MonoBehaviour
{
    public Transform Target;   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #if ZP_SERVER
        var q = ZTransformEx.GetGlobalRotation(transform);
        var eu = Quat2Euler.QuaternionToEuler(q);
        //Debug.Log("eu is " + eu.ToString());


        var pos = ZTransformEx.GetGlobalPosition(transform);

        Target.position = pos;
        Target.eulerAngles = eu;

#endif
    }
}
