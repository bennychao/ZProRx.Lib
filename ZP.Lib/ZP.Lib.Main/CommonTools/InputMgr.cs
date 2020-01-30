using UnityEngine;
using System.Collections;

public class InputMgr : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static InputMgr GetInstance()
    {
        return Camera.main.GetComponent<InputMgr>();
    }

    public Vector3 GetPosition()
    {
        return Input.mousePosition;
    }

    public bool IsClickDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool IsDowning()
    {
        return Input.GetButton("Fire1");
    }
}
