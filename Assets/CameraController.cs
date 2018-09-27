using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机控制器
/// </summary>
public class CameraController : MonoBehaviour {

    public float speedMove = 50;
    public float speedRotate = 500;
    public float speedScalling = 100;
    public DepthTextureMode depthTextureMode;

    private static Transform instance;

	void Start () {
        instance = transform;
	    Camera.main.depthTextureMode = depthTextureMode;
	}

    void Update()
    {
        float _scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetMouseButton(1))
        {
            CameraRotate();
        }
        if (Input.GetMouseButton(2))
        {
            CameraMove();
        }
        CameraScalling(_scrollWheelValue);
    }

    public static void ResetCamera()
    {
        instance.position = new Vector3(4.1f, 7.88f, -5.2f);
        instance.rotation = Quaternion.Euler(40, 0, 0);
    }

    private void CameraMove()
    {
        transform.position += (-Input.GetAxis("Mouse X") * transform.right - Input.GetAxis("Mouse Y") * transform.up) * Time.deltaTime * speedMove;
    }

    private void CameraScalling(float axis)
    {
        if (Mathf.Abs(axis) > 0.01f)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * axis * speedRotate);
        }
    }

    private void CameraRotate()
    {
        Vector3 _rotation = transform.rotation.eulerAngles;
        _rotation += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speedScalling;
        transform.rotation = Quaternion.Euler(_rotation);
    }
}
