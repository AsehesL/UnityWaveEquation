using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机控制器
/// </summary>
public class CameraController : MonoBehaviour
{

    public Vector3 cameraTarget;
    public Bounds targetRange;
    public Vector2 distanceRange;
    public Vector2 eulerXRange;
    public Vector2 eulerYRange;

    public float moveLerpSpeed;
    public float rotateLerpSpeed;

    public float speedMove = 50;
    public float speedRotate = 500;
    public float speedScalling = 100;

    [SerializeField]private float m_Distance;
    private Vector3 m_Target;
    private Quaternion m_Rotation;

	void Start ()
	{
	    m_Target = cameraTarget;
	    m_Distance = Vector3.Distance(transform.position, m_Target);
	    m_Rotation = transform.rotation;

	    transform.position = m_Target - transform.forward * m_Distance;
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

        Vector3 camPos = m_Target - transform.forward * m_Distance;

        transform.rotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * rotateLerpSpeed);
        transform.position = Vector3.Lerp(transform.position, camPos, Time.deltaTime * moveLerpSpeed);
    }

    private void CameraMove()
    {
        m_Target += (-Input.GetAxis("Mouse X") * transform.right - Input.GetAxis("Mouse Y") * transform.up) * Time.deltaTime * speedMove;

        m_Target.x = Mathf.Clamp(m_Target.x, targetRange.min.x, targetRange.max.x);
        m_Target.y = Mathf.Clamp(m_Target.y, targetRange.min.y, targetRange.max.y);
        m_Target.z = Mathf.Clamp(m_Target.z, targetRange.min.z, targetRange.max.z);
    }

    private void CameraScalling(float axis)
    {
        if (Mathf.Abs(axis) > 0.01f)
        {
            m_Distance -= Time.deltaTime* axis * speedScalling;
            m_Distance = Mathf.Clamp(m_Distance, distanceRange.x, distanceRange.y);
        }
    }

    private void CameraRotate()
    {
        Vector3 _rotation = transform.rotation.eulerAngles;
        _rotation += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speedRotate;
        
        _rotation.x = Mathf.Clamp(_rotation.x, eulerXRange.x, eulerXRange.y);
        _rotation.y = Mathf.Clamp(_rotation.y, eulerYRange.x, eulerYRange.y);

        _rotation.z = 0;

        m_Rotation = Quaternion.Euler(_rotation);
    }
}
