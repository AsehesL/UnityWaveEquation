using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 液体焦散渲染器
/// </summary>
public class LiquidCausticRenderer : MonoBehaviour
{
    /// <summary>
    /// 网格单元格大小
    /// </summary>
    public float geometryCellSize;

    /// <summary>
    /// 焦散宽度
    /// </summary>
    public float causticWidth;

    /// <summary>
    /// 焦散长度
    /// </summary>
    public float causticLength;

    public Vector2 causticDepthRange;

    [SerializeField] private Material m_CausticMaterial;

    private Mesh m_Mesh;

    private Camera m_Camera;

    private RenderTexture m_RenderTexture;
    private CommandBuffer m_CommandBuffer;

    private float m_Width;
    private float m_Height;

    void Start()
    {
        m_Camera = gameObject.AddComponent<Camera>();
        m_Camera.aspect = causticWidth / causticLength;
        m_Camera.backgroundColor = Color.black;
        //m_Camera.enabled = false;
        m_Camera.depth = 0;
        m_Camera.farClipPlane = 5;
        m_Camera.nearClipPlane = -5;
        m_Camera.orthographic = true;
        m_Camera.orthographicSize = causticLength * 0.5f * 1.2f;
        //m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.allowHDR = false;
        m_Camera.backgroundColor = Color.black;
        m_Camera.cullingMask = 0;

        m_Width = causticWidth * 0.5f * 1.2f;
        m_Height = causticLength * 0.5f * 1.2f;

        m_RenderTexture = RenderTexture.GetTemporary(512, 512, 16);
        m_RenderTexture.name = "[Caustic]";
        m_Camera.targetTexture = m_RenderTexture;

        m_CommandBuffer = new CommandBuffer();
        m_CommandBuffer.name = "[Caustic CB]";
        m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_CommandBuffer);

        m_Mesh = LiquidUtils.GenerateLiquidMesh(causticWidth, causticLength, geometryCellSize);

    }

    void OnGUI()
    {
        if (m_RenderTexture)
            GUI.DrawTexture(new Rect(0, 0, 100, 100), m_RenderTexture);
    }

    void OnPostRender()
    {
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        m_CommandBuffer.Clear();
        m_CommandBuffer.ClearRenderTarget(true, true, Color.black);

        m_CommandBuffer.SetRenderTarget(m_RenderTexture);

        m_CommandBuffer.DrawMesh(m_Mesh, trs, m_CausticMaterial);

        Vector4 plane = new Vector4(0, 1, 0, Vector3.Dot(new Vector3(0, 1, 0), transform.position));
        Vector4 range = new Vector4(transform.position.x, transform.position.z, m_Width * 1.2f, m_Height * 1.2f);

        Shader.SetGlobalVector("_CausticPlane", plane);
        Shader.SetGlobalVector("_CausticRange", range);
        Shader.SetGlobalTexture("_CausticMap", m_RenderTexture);
        Shader.SetGlobalVector("_CausticDepthRange", causticDepthRange);
    }

    void OnDestroy()
    {
        if (m_RenderTexture)
            Destroy(m_RenderTexture);
        if (m_Mesh)
            Destroy(m_Mesh);
        if (m_CommandBuffer != null)
        {
            m_CommandBuffer.Release();
            m_CommandBuffer = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, causticWidth, causticLength, 0, 0,
            Color.blue);
    }
}
