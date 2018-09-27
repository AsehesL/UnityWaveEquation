using UnityEngine;
using System.Collections;
using ASL.LiquidSimulator;

/// <summary>
/// 液体模拟器
/// </summary>
public class LiquidSimulator : MonoBehaviour
{
    #region public
    /// <summary>
    /// 网格单元格大小
    /// </summary>
    public float geometryCellSize;

    public float causticCellSize;
    /// <summary>
    /// 液面宽度
    /// </summary>
    public float liquidWidth;
    /// <summary>
    /// 液面长度
    /// </summary>
    public float liquidLength;
    /// <summary>
    /// 液体深度
    /// </summary>
    public float liquidDepth;

    public int heightMapSize;

    /// <summary>
    /// 粘度系数
    /// </summary>
    public float Viscosity
    {
        get { return m_Viscosity; }
        set
        {
            m_Viscosity = value;
            if (m_Viscosity < 0.0001f)
                m_Viscosity = 0.0001f;
        }
    }

    /// <summary>
    /// 波速
    /// </summary>
    public float Velocity
    {
        get { return m_Velocity; }
        set
        {
            
        }
    }

    /// <summary>
    /// 力度系数
    /// </summary>
    public float ForceFactor
    {
        get { return m_ForceFactor; }
        set
        {

        }
    }

    public LayerMask InteractLayer
    {
        get { return m_InteractLayer; }
        set
        {

        }
    }

    public Material LiquidMaterial
    {
        get { return m_LiquidMaterial; }
        set
        {
            m_LiquidMaterial = value;
            if (m_Renderer)
                m_Renderer.SetLiquidMaterial(m_LiquidMaterial);
        }
    }
    
    #endregion

    [SerializeField] private float m_Viscosity;
    [SerializeField] private float m_Velocity;
    [SerializeField] private float m_ForceFactor;
    //[SerializeField] private float m_SampleSpacing;
    [SerializeField] private LayerMask m_InteractLayer;
    [SerializeField] private Material m_LiquidMaterial;
    [SerializeField] private Material m_LiquidCausticMaterial;

    private bool m_IsSupported;

    private LiquidRenderer m_Renderer;
    private LiquidSampleCamera m_SampleCamera;

    private LiquidCausticRenderer m_CausticRenderer;

    private Vector4 m_LiquidParams;

    private float m_SampleSpacing;

    void Start()
    {
        m_SampleSpacing = 1.0f / heightMapSize;

        m_IsSupported = CheckSupport();
        if (!m_IsSupported)
            return;

        m_SampleCamera = new GameObject("[LiquidSampleCamera]").AddComponent<LiquidSampleCamera>();
        m_SampleCamera.transform.SetParent(transform);
        m_SampleCamera.transform.localPosition = Vector3.zero;
        m_SampleCamera.transform.localEulerAngles = new Vector3(90,0,0);
        m_SampleCamera.Init(m_InteractLayer, liquidWidth, liquidLength, liquidDepth, m_ForceFactor,
            new Vector4(transform.up.x, transform.up.y, transform.up.z,
                -Vector3.Dot(transform.up, transform.position)), m_LiquidParams, heightMapSize);

        m_Renderer = new GameObject("[LiquidRenderer]").AddComponent<LiquidRenderer>();
        m_Renderer.transform.SetParent(transform);
        m_Renderer.transform.localPosition = Vector3.zero;
        m_Renderer.transform.localEulerAngles = Vector3.zero;
        m_Renderer.Init(geometryCellSize, liquidWidth, liquidLength);
        m_Renderer.SetLiquidMaterial(m_LiquidMaterial);
        m_Renderer.SetLiquidHeightMap(m_SampleCamera.HeightMap);
        m_Renderer.SetLiquidNormalMap(m_SampleCamera.NormalMap);
        m_Renderer.SetLiquidReflectMap(m_SampleCamera.ReflectMap);



        m_CausticRenderer = new GameObject(("[LiquidCausticRenderer]")).AddComponent<LiquidCausticRenderer>();
        m_CausticRenderer.transform.SetParent(transform);
        m_CausticRenderer.transform.localPosition = Vector3.zero;
        m_CausticRenderer.transform.localEulerAngles = new Vector3(90, 0, 0);
        m_CausticRenderer.Init(causticCellSize, liquidWidth, liquidLength);
        m_CausticRenderer.SetLiquidMaterial(m_LiquidCausticMaterial);
        m_CausticRenderer.SetLiquidHeightMap(m_SampleCamera.HeightMap);
        m_CausticRenderer.SetLiquidNormalMap(m_SampleCamera.NormalMap);
    }
    
    void Update()
    {

    }

    bool CheckSupport()
    {
        if (geometryCellSize <= 0)
        {
            Debug.LogError("网格单元格大小不允许小于等于0！");
            return false;
        }
        if (liquidWidth <= 0 || liquidLength <= 0)
        {
            Debug.LogError("液体长宽不允许小于等于0！");
            return false;
        }
        if (liquidDepth <= 0)
        {
            Debug.LogError("液体深度不允许小于等于0！");
            return false;
        }


        if (!RefreshLiquidParams(m_Velocity, m_Viscosity))
            return false;

        return true;
    }

    private bool RefreshLiquidParams(float speed, float viscosity)
    {
        if (speed <= 0)
        {
            Debug.LogError("波速不允许小于等于0！");
            return false;
        }
        if (viscosity <= 0)
        {
            Debug.LogError("粘度系数不允许小于等于0！");
            return false;
        }
        float maxvelocity = m_SampleSpacing / (2 * Time.fixedDeltaTime) * Mathf.Sqrt(viscosity * Time.fixedDeltaTime + 2);
        float velocity = maxvelocity * speed;
        float viscositySq = viscosity * viscosity;
        float velocitySq = velocity * velocity;
        float deltaSizeSq = m_SampleSpacing * m_SampleSpacing;
        float dt = Mathf.Sqrt(viscositySq + 32 * velocitySq / (deltaSizeSq));
        float dtden = 8 * velocitySq / (deltaSizeSq);
        float maxT = (viscosity + dt) / dtden;
        float maxT2 = (viscosity - dt) / dtden;
        if (maxT2 > 0 && maxT2 < maxT)
            maxT = maxT2;
        if (maxT < Time.fixedDeltaTime)
        {
            Debug.LogError("粘度系数不符合要求");
            return false;
        }

        float fac = velocitySq * Time.fixedDeltaTime * Time.fixedDeltaTime / deltaSizeSq;
        float i = viscosity * Time.fixedDeltaTime - 2;
        float j = viscosity * Time.fixedDeltaTime + 2;

        float k1 = (4 - 8 * fac) / (j);
        float k2 = i / j;
        float k3 = 2 * fac / j;

        m_LiquidParams = new Vector4(k1, k2, k3, m_SampleSpacing);

        Debug.Log(m_LiquidParams.ToString("f7"));
        m_Velocity = speed;
        m_Viscosity = viscosity;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, liquidWidth, liquidLength, -liquidDepth, 0, Color.green);
    }
}
