using System.Collections;
using System.Collections.Generic;
using ASL.LiquidSimulator;
using UnityEngine;

/// <summary>
/// 液体渲染器
/// </summary>
public class LiquidRenderer : MonoBehaviour
{
    #region public:
    /// <summary>
    /// 网格大小
    /// </summary>
    public float cellSize;
    /// <summary>
    /// 水体宽度
    /// </summary>
    public float width;
    /// <summary>
    /// 水体长度
    /// </summary>
    public float length;
    /// <summary>
    /// 水体深度
    /// </summary>
    public float depth;
    /// <summary>
    /// 粘度系数
    /// </summary>
    public float viscosity;
    /// <summary>
    /// 波速
    /// </summary>
    public float velocity;
    /// <summary>
    /// 力度系数
    /// </summary>
    public float forceFactor;

    public float fade = 1.0f;

    public LayerMask interactLayer;

    public bool IsSupported
    {
        get { return m_IsSupported; }
    }
    #endregion

    private bool m_IsSupported;

    private LiquidSampleCamera m_Camera;
    private LiquidGeometry m_Geometry;


    void Start () {
        m_IsSupported = CheckSupport();
        if (!m_IsSupported)
        {
            Debug.LogError("初始化失败");
            return;
        }

        float fac = velocity * velocity * 0.02f * 0.02f / (cellSize * cellSize);
        float i = viscosity * 0.02f - 2;
        float j = viscosity * 0.02f + 2;

        float k1 = (4 - 8 * fac) / (j);
        float k2 = i / j;
        float k3 = 2 * fac / j;

        m_Camera = new GameObject("[SampleCamera]").AddComponent<LiquidSampleCamera>();

        m_Camera.Init(interactLayer, width, length, depth, forceFactor, fade, new Vector4(k1, k2, k3, 0.005f), 1024);
        m_Camera.transform.SetParent(transform);
        m_Camera.transform.localPosition = Vector3.zero;
        m_Camera.transform.localEulerAngles = new Vector3(90, 0, 0);

        m_Geometry = new LiquidGeometry(gameObject, cellSize, width, length);
        m_Geometry.SetLiquidHeightMap(m_Camera.HeightMap);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        if (m_Camera)
            m_Camera.Release();
        if (m_Geometry != null)
            m_Geometry.Release();
        m_Camera = null;
        m_Geometry = null;
    }

    bool CheckSupport()
    {
        if (cellSize <= 0)
        {
            return false;
        }
        if (width <= 0 || length <= 0 || depth <= 0)
        {
            return false;
        }
        if (velocity < 0)
            return false;
        float maxV = cellSize / (2 * 0.02f) * Mathf.Sqrt(viscosity * 0.02f + 2);
        if (velocity >= maxV)
        {
            Debug.Log(maxV.ToString("f5"));
            Debug.LogError("波速不符合要求");
            return false;
        }
        float viscositySq = viscosity * viscosity;
        float velocitySq = velocity * velocity;
        float deltaSizeSq = cellSize * cellSize;
        float dt = Mathf.Sqrt(viscositySq + 32 * velocitySq / (deltaSizeSq));
        float dtden = 8 * velocitySq / (deltaSizeSq);
        float maxT = (viscosity + dt) / dtden;
        float maxT2 = (viscosity - dt) / dtden;
        if (maxT2 > 0 && maxT2 < maxT)
            maxT = maxT2;
        if (maxT < 0.02f)
        {
            Debug.LogError("时间间隔不符合要求");
            return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        //LiquidUtils.DrawWirePlane(transform.position, transform.eulerAngles.y, width, length, Color.gray);

        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, width, length, -depth, 0, Color.green);
    }
}
