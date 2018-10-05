using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 平行光-用于渲染和投射阴影
/// </summary>
public class SunLight : MonoBehaviour {

    public int shadowMapSize;

    public float bias;

    public float near;
    public float far;

    public float size;
    public float aspect;
    public Color color;
    public float intensity;

    public Shader shadowMapRenderShader;

    private Camera m_Camera;
    private RenderTexture m_ShadowMap;

    private Mesh m_BGMesh;
    private Material m_BGMaterial;

    void Start () {
        InitRenderTarget();

        m_BGMesh = new Mesh();
        m_BGMesh.vertices = new Vector3[]
        {
            new Vector3(-aspect * size, -size, far - 0.01f), new Vector3(-aspect * size, size, far - 0.01f),
            new Vector3(aspect * size, size, far - 0.01f), new Vector3(aspect * size, -size, far - 0.01f)
        };
        m_BGMesh.triangles = new int[]
        {
            0, 1, 2, 0, 2, 3
        };
        m_BGMaterial = new Material(shadowMapRenderShader);

        //MeshRenderer mr = new GameObject("t").AddComponent<MeshRenderer>();
        //MeshFilter mf = mr.gameObject.AddComponent<MeshFilter>();
        //mr.transform.SetParent(transform);
        //mr.transform.localPosition = Vector3.zero;
        //mr.transform.localEulerAngles = Vector3.zero;
        //mr.sharedMaterial = m_BGMaterial;
        //mf.sharedMesh = m_BGMesh;
    }

    void OnDestroy()
    {
        if (m_ShadowMap)
            Destroy(m_ShadowMap);
        m_ShadowMap = null;
        if (m_BGMaterial)
            Destroy(m_BGMaterial);
        if(m_BGMesh)
            Destroy(m_BGMesh);
    }

    //void OnPostRender()
    //{
    //    m_BGMaterial.SetPass(0);
    //    Graphics.DrawMeshNow(m_BGMesh, transform.localToWorldMatrix);
    //}

    void OnPreRender()
    {
        //m_BGMaterial.SetPass(0);
        //Graphics.DrawMeshNow(m_BGMesh, transform.localToWorldMatrix);

        Matrix4x4 wtl = m_Camera.worldToCameraMatrix;

        Shader.SetGlobalVector("internalWorldLightPos",
            new Vector4(transform.forward.x, transform.forward.y, transform.forward.z, 0));
        Shader.SetGlobalMatrix("internalWorldLightMV", wtl);

        Shader.SetGlobalMatrix("internalWorldLightVP", m_Camera.projectionMatrix);
        Shader.SetGlobalVector("internalProjectionParams",
            new Vector4(0.01f, m_Camera.nearClipPlane, m_Camera.farClipPlane, 1 / m_Camera.farClipPlane));
        Shader.SetGlobalColor("internalWorldLightColor",
            new Color(color.r*intensity, color.g * intensity, color.b * intensity, color.a));
        Shader.SetGlobalFloat("internalBias", bias);
    }

    void InitRenderTarget()
    {
        if (m_Camera == null)
        {
            m_Camera = GetComponent<Camera>();
            if (m_Camera == null)
                m_Camera = gameObject.AddComponent<Camera>();
            //m_Camera.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            //m_Camera = new GameObject("[RenderCamera]").AddComponent<Camera>();
            //m_Camera.transform.SetParent(transform);
            //m_Camera.transform.localPosition = Vector3.zero;
            //m_Camera.transform.localRotation = Quaternion.identity;
            m_Camera.aspect = aspect;
            m_Camera.backgroundColor = new Color(1,1,1,0);//new Color(0, 0, 0, 0);
            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.depth = 0;
            m_Camera.farClipPlane = far;
            m_Camera.nearClipPlane = near;
            m_Camera.orthographic = true;
            m_Camera.orthographicSize = size;
            m_Camera.SetReplacementShader(shadowMapRenderShader, "RenderType");
        }
        if (m_ShadowMap == null)
        {
            m_ShadowMap = new RenderTexture(shadowMapSize, shadowMapSize, 16);
            m_Camera.targetTexture = m_ShadowMap;
            Shader.SetGlobalTexture("internalShadowMap", m_ShadowMap);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0.3f, 1f, 0.6f);

        Vector3 vp1 = transform.position + transform.rotation * new Vector3(-size * aspect, -size, near);
        Vector3 vp2 = transform.position + transform.rotation * new Vector3(-size * aspect, size, near);
        Vector3 vp3 = transform.position + transform.rotation * new Vector3(size * aspect, size, near);
        Vector3 vp4 = transform.position + transform.rotation * new Vector3(size * aspect, -size, near);

        Vector3 vp5 = transform.position + transform.rotation * new Vector3(-size * aspect, -size, far);
        Vector3 vp6 = transform.position + transform.rotation * new Vector3(-size * aspect, size, far);
        Vector3 vp7 = transform.position + transform.rotation * new Vector3(size * aspect, size, far);
        Vector3 vp8 = transform.position + transform.rotation * new Vector3(size * aspect, -size, far);

        Gizmos.DrawLine(vp1, vp2);
        Gizmos.DrawLine(vp2, vp3);
        Gizmos.DrawLine(vp3, vp4);
        Gizmos.DrawLine(vp4, vp1);

        Gizmos.DrawLine(vp5, vp6);
        Gizmos.DrawLine(vp6, vp7);
        Gizmos.DrawLine(vp7, vp8);
        Gizmos.DrawLine(vp8, vp5);

        Gizmos.DrawLine(vp1, vp5);
        Gizmos.DrawLine(vp2, vp6);
        Gizmos.DrawLine(vp3, vp7);
        Gizmos.DrawLine(vp4, vp8);
    }
}
