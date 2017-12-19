using UnityEngine;
using System.Collections;

public class LiquidSimulatorCamera : MonoBehaviour
{
    private Camera m_Camera;
    
    void Update()
    {

    }

    public void Init(LayerMask interactLayer, float size, float near, float far)
    {
        m_Camera = gameObject.AddComponent<Camera>();
        m_Camera.aspect = 1;
        m_Camera.backgroundColor = Color.black;
        m_Camera.cullingMask = interactLayer;
        m_Camera.depth = 0;
        m_Camera.farClipPlane = far;
        m_Camera.nearClipPlane = near;
        m_Camera.orthographic = true;
        m_Camera.orthographicSize = size;
        m_Camera.clearFlags = CameraClearFlags.Nothing;
    }
}
