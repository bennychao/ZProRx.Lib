using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;

public class TestServerEx : MonoBehaviour
{
    public Transform Target;
    public bool bServer = false;
    public float angle = 30;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = ZTransformEx.LookAt(Target.forward, Target.up);
#if ZP_SERVER
        //var f =  Mathf.PI;
        //var r1 = ZTransformEx.AngleAxisEx(angle, Target.up);

        var r1 = ZTransformEx.LookAt(Target.forward, Target.up);

        //var r2 = Quaternion.AngleAxis(angle, Target.up);
        //Vector3.zero
        var r2 = Quaternion.LookRotation(Target.forward, Target.up);

        transform.rotation = bServer? r1 : r2;
#endif
    }
}
