using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;

public class TestEuler : MonoBehaviour
{

    public Transform Target;

    public float a;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var i = Math.Asin(a) * 180f / Mathf.PI;

        var eu = Quat2Euler.QuaternionToEuler(Target.rotation);
            Debug.Log("eu is " + i.ToString());

        transform.eulerAngles = eu;
    }
}
