using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{

    public GameObject dragTarget;
    public LiquidSimulator simulator;
    public Mesh swipeMesh;
    public float swipeSize;
    public float height;

    private Camera m_Camera;
    
    private bool m_IsBeginDrag;
    private float m_DragPlane;
    private Vector4 m_DragWorldPlane;
    private Vector4 m_SwipePlane;

    private Vector3 m_Offset;

    void Start ()
    {
        m_Camera = gameObject.GetComponent<Camera>();
        m_SwipePlane = new Vector4(0, 1, 0, Vector3.Dot(Vector3.up, new Vector3(0, height, 0)));
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!m_IsBeginDrag)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == dragTarget)
                    {
                        if (!m_IsBeginDrag)
                        {
                            m_IsBeginDrag = true;
                            Vector3 viewPos =
                                m_Camera.transform.worldToLocalMatrix.MultiplyPoint(hit.collider.transform.position);
                            m_DragPlane = viewPos.z;
                            m_Offset = hit.point - hit.collider.transform.position;
                            m_DragWorldPlane = new Vector4(0, 1, 0, Vector3.Dot(Vector3.up, hit.point));
                        }
                    }
                    else if (hit.collider.gameObject == simulator.gameObject)
                    {
                        if (!m_IsBeginDrag)
                        {
                            float t = (m_SwipePlane.w -
                                       Vector3.Dot(ray.origin,
                                           new Vector3(m_SwipePlane.x, m_SwipePlane.y, m_SwipePlane.z)))/
                                      Vector3.Dot(ray.direction,
                                          new Vector3(m_SwipePlane.x, m_SwipePlane.y, m_SwipePlane.z));
                            Vector3 hitpos = ray.origin + ray.direction*t;
                            Matrix4x4 matrix = Matrix4x4.TRS(hitpos, Quaternion.identity, Vector3.one*swipeSize);
                            LiquidSimulator.DrawMesh(swipeMesh, matrix);
                        }
                    }
                }
            }
            else
            {
                if (m_Camera.transform.eulerAngles.x > 45)
                {
                    float t = (m_DragWorldPlane.w -
                               Vector3.Dot(ray.origin,
                                   new Vector3(m_DragWorldPlane.x, m_DragWorldPlane.y, m_DragWorldPlane.z))) /
                              Vector3.Dot(ray.direction,
                                  new Vector3(m_DragWorldPlane.x, m_DragWorldPlane.y, m_DragWorldPlane.z));
                    Vector3 hitpos = ray.origin + ray.direction * t;

                    dragTarget.transform.position = hitpos - m_Offset;
                }
                else
                {
                    float tan = Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    float height = m_DragPlane * tan;
                    float width = height * m_Camera.aspect;

                    float x = (Input.mousePosition.x / Screen.width) * 2 - 1;
                    float y = (Input.mousePosition.y / Screen.height) * 2 - 1;
                    Vector3 viewPos = new Vector3(x * width, y * height, m_DragPlane);
                    Vector3 pos = m_Camera.transform.localToWorldMatrix.MultiplyPoint(viewPos);
                    dragTarget.transform.position = pos - m_Offset;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_IsBeginDrag = false;
        }
    }
    
}
