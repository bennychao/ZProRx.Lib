using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Common;

public class LoginBuilder :Singleton<LoginBuilder>
{
    public Transform LoginCanvas;

    public Transform RegisterCanvas;

    // Start is called before the first frame update
    void Start()
    {
        //var loginConfig = RoomMatrixBehaviour.Instance.GetComponent<ILoginItem>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BindLogin(object system)
    {
        ZViewBuildTools.BindObject(system, LoginCanvas);
    }

    public void BindRegister(object system)
    {
        ZViewBuildTools.BindObject(system, RegisterCanvas);
    }
}
