using UnityEngine;
using System.Collections;

public class LiquidSampleObject : MonoBehaviour
{
    private Renderer m_Renderer;
    private Matrix4x4 m_LocalMatrix;

    void Start()
    {
        m_Renderer = gameObject.GetComponent<Renderer>();
        m_LocalMatrix = transform.localToWorldMatrix;
    }

    void OnRenderObject()
    {
        if (m_Renderer && m_LocalMatrix != transform.localToWorldMatrix)
        {
            m_LocalMatrix = transform.localToWorldMatrix;
            LiquidSimulator.DrawObject(m_Renderer);
        }
    }
}
