using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using ZProRx.Lib.Test;

public class TestWebBuilder : MonoBehaviour
{
    public string Url = "http://localhost:6008";
    // Start is called before the first frame update
    void Start()
    {
        var system = ZPropertyMesh.CreateObjectWithParam<TestWebSystem>(Url);

        ZViewBuildTools.BindObject(system, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
