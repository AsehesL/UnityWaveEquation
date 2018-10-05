using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 焦散渲染器
/// </summary>
public class CausticRenderer : MonoBehaviour
{
    /// <summary>
    /// 网格单元格大小
    /// </summary>
    public float geometryCellSize;
    /// <summary>
    /// 焦散宽度
    /// </summary>
    public float width;
    /// <summary>
    /// 焦散长度
    /// </summary>
    public float length;
    /// <summary>
    /// 焦散强度
    /// </summary>
    public float causticIntensity = 1.0f;
    /// <summary>
    /// 深度范围（该参数目前实现比较简单，只是简单的传入世界空间的最小高度和有效高度范围，以计算焦散的有效高度范围（线性插值），暂时没有实现复杂的范围计算效果）
    /// </summary>
    public Vector2 causticDepthRange;

    public Material material;

    private Mesh m_Mesh;

    private Camera m_Camera;

    private RenderTexture m_RenderTexture;
    private CommandBuffer m_CommandBuffer;

    void Start()
    {
        m_Camera = gameObject.AddComponent<Camera>();
        m_Camera.aspect = width / length;
        m_Camera.backgroundColor = Color.black;
        //m_Camera.enabled = false;
        m_Camera.depth = 0;
        m_Camera.farClipPlane = 5;
        m_Camera.nearClipPlane = -5;
        m_Camera.orthographic = true;
        m_Camera.orthographicSize = length * 0.5f;
        //m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.allowHDR = false;
        m_Camera.backgroundColor = Color.black;
        m_Camera.cullingMask = 0;

        m_RenderTexture = RenderTexture.GetTemporary(512, 512, 16);
        m_RenderTexture.name = "[Caustic]";
        m_Camera.targetTexture = m_RenderTexture;

        m_CommandBuffer = new CommandBuffer();
        m_CommandBuffer.name = "[Caustic CB]";
        m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_CommandBuffer);

        m_Mesh = Utils.GenerateLiquidMesh(width, length, geometryCellSize);

    }

    void OnPostRender()
    {
        //绘制焦散mesh
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        m_CommandBuffer.Clear();
        m_CommandBuffer.ClearRenderTarget(true, true, Color.black);

        m_CommandBuffer.SetRenderTarget(m_RenderTexture);

        m_CommandBuffer.DrawMesh(m_Mesh, trs, material);

        Vector4 plane = new Vector4(0, 1, 0, Vector3.Dot(new Vector3(0, 1, 0), transform.position));
        Vector4 range = new Vector4(transform.position.x, transform.position.z, width * 0.5f, length * 0.5f);

        Shader.SetGlobalVector("_CausticPlane", plane);
        Shader.SetGlobalVector("_CausticRange", range);
        Shader.SetGlobalTexture("_CausticMap", m_RenderTexture);
        Shader.SetGlobalVector("_CausticDepthRange", causticDepthRange);
        Shader.SetGlobalFloat("_CausticIntensity", causticIntensity);
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
        Utils.DrawWireCube(transform.position, transform.eulerAngles.y, width, length, 0, 0,
            Color.blue);
    }
}
