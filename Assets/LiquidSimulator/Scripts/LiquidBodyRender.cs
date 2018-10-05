using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidBodyRender : MonoBehaviour {

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

    [SerializeField] private Material m_LiquidBodyMaterial;

    private Mesh m_LiquidBodyMesh;
    private MeshFilter m_LiquidBodyMeshFilter;
    private MeshRenderer m_LiquidBodyMeshRenderer;
    
    void Start () {
        m_LiquidBodyMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (m_LiquidBodyMeshRenderer == null)
            m_LiquidBodyMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        m_LiquidBodyMeshFilter = gameObject.GetComponent<MeshFilter>();
        if (m_LiquidBodyMeshFilter == null)
            m_LiquidBodyMeshFilter = gameObject.AddComponent<MeshFilter>();

        m_LiquidBodyMesh = LiquidUtils.GenerateLiquidBodyMesh(liquidWidth, liquidLength, liquidDepth, geometryCellSize);
        m_LiquidBodyMeshFilter.sharedMesh = m_LiquidBodyMesh;
        m_LiquidBodyMeshRenderer.sharedMaterial = m_LiquidBodyMaterial;

        Vector3 boundsMin = new Vector3(transform.position.x - liquidWidth * 0.5f, transform.position.y - liquidDepth,
            transform.position.z - liquidLength * 0.5f);
        Vector3 boundsMax = new Vector3(transform.position.x + liquidWidth * 0.5f, transform.position.y,
            transform.position.z + liquidLength * 0.5f);

        Vector4 plane = new Vector4(0, 1, 0, Vector3.Dot(new Vector3(0, 1, 0), transform.position));
        m_LiquidBodyMaterial.SetVector("_BoundsMin", boundsMin);
        m_LiquidBodyMaterial.SetVector("_BoundsMax", boundsMax);
        m_LiquidBodyMaterial.SetVector("_WaterPlane", plane);
    }
	
	

    void OnDrawGizmosSelected()
    {
        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, liquidWidth, liquidLength, -liquidDepth, 0, Color.yellow);
    }
}
