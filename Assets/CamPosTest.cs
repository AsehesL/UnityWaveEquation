using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPosTest : MonoBehaviour
{

    public Camera cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if (cam == null)
            return;
        Vector3 pos = cam.worldToCameraMatrix.MultiplyPoint(transform.position);

        GUILayout.Label("CamPos:" + pos);
    }
}
