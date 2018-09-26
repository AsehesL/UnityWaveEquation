using UnityEngine;
using System.Collections;

public class MatrixTest : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log(transform.localToWorldMatrix);
        Debug.Log(transform.right.ToString("f4") + "," + transform.up.ToString("f4") + "," + transform.forward.ToString("f4"));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
