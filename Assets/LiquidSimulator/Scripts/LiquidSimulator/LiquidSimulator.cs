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

    /// <summary>
    /// 粘度系数
    /// </summary>
    public float Viscosity
    {
        get
        {
            
        }
        set
        {
            
        }
    }

    /// <summary>
    /// 波速
    /// </summary>
    public float Velocity
    {
        get
        {

        }
        set
        {

        }
    }

    /// <summary>
    /// 力度系数
    /// </summary>
    public float ForceFactor
    {
        get
        {

        }
        set
        {

        }
    }

    /// <summary>
    /// 采样间距
    /// </summary>
    public float SampleSpacing
    {
        get
        {

        }
        set
        {

        }
    }

    public LayerMask InteractLayer
    {
        get
        {

        }
        set
        {

        }
    }
    
    #endregion

    [SerializeField] private float m_Viscosity;
    [SerializeField] private float m_Velocity;
    [SerializeField] private float m_ForceFactor;
    [SerializeField] private float m_SampleSpacing;
    [SerializeField] private LayerMask m_InteractLayer;

    private bool m_IsSupported;

    private LiquidRenderer m_Renderer;
    private LiquidSampleCamera m_SampleCamera;

    void Start()
    {

    }
    
    void Update()
    {

    }
}
