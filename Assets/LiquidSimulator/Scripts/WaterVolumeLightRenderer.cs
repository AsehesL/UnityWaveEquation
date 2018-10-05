using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 液体体积光渲染器
/// 该渲染器的功能只是生成水体（不含水面）的Mesh并为材质传入包围盒和水面平面信息，实际的功能实现全部在shader中
/// </summary>
public class WaterVolumeLightRenderer : MonoBehaviour {

    /// <summary>
    /// 网格单元格大小
    /// </summary>
    public float geometryCellSize;
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

    public Material material;

    private Mesh m_waterBodyMesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;
    
    void Start () {
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (m_MeshRenderer == null)
            m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        m_MeshFilter = gameObject.GetComponent<MeshFilter>();
        if (m_MeshFilter == null)
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();

        m_waterBodyMesh = Utils.GenerateLiquidBodyMesh(width, length, depth, geometryCellSize);
        m_MeshFilter.sharedMesh = m_waterBodyMesh;
        m_MeshRenderer.sharedMaterial = material;

        //传入水体包围盒信息，用于计算水底光线追踪的范围
        Vector3 boundsMin = new Vector3(transform.position.x - width * 0.5f, transform.position.y - depth,
            transform.position.z - length * 0.5f);
        Vector3 boundsMax = new Vector3(transform.position.x + width * 0.5f, transform.position.y,
            transform.position.z + length * 0.5f);

        //传入水体平面用于获取水体表面法线，以计算折射光线
        Vector4 plane = new Vector4(0, 1, 0, Vector3.Dot(new Vector3(0, 1, 0), transform.position));

        material.SetVector("_BoundsMin", boundsMin);
        material.SetVector("_BoundsMax", boundsMax);
        material.SetVector("_WaterPlane", plane);
    }

    void OnDrawGizmosSelected()
    {
        Utils.DrawWireCube(transform.position, transform.eulerAngles.y, width, length, -depth, 0, Color.yellow);
    }
}
