using System.Collections;
using System.Collections.Generic;
using ASL.LiquidSimulator;
using UnityEngine;

namespace ASL.LiquidSimulator
{
    public class LiquidRenderer : MonoBehaviour
    {
        private Mesh m_LiquidMesh;

        private MeshFilter m_LiquidMeshFilter;
        private MeshRenderer m_LiquidMeshRenderer;

        public void SetLiquidHeightMap(RenderTexture heightMap)
        {
            if (m_LiquidMeshRenderer && m_LiquidMeshRenderer.sharedMaterial)
                m_LiquidMeshRenderer.sharedMaterial.SetTexture("_LiquidHeightMap", heightMap);
        }

        public void SetLiquidNormalMap(RenderTexture normalMap)
        {
            if (m_LiquidMeshRenderer && m_LiquidMeshRenderer.sharedMaterial)
                m_LiquidMeshRenderer.sharedMaterial.SetTexture("_LiquidNormalMap", normalMap);
        }

        public void SetLiquidReflectMap(RenderTexture reflectMap)
        {
            if (m_LiquidMeshRenderer && m_LiquidMeshRenderer.sharedMaterial)
                m_LiquidMeshRenderer.sharedMaterial.SetTexture("_LiquidReflectMap", reflectMap);
        }

        public void SetLiquidMaterial(Material liquidMaterial)
        {
            if (m_LiquidMeshRenderer)
                m_LiquidMeshRenderer.sharedMaterial = liquidMaterial;
        }

        public void Init(float cellSize, float width, float length)
        {
            m_LiquidMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (m_LiquidMeshRenderer == null)
                m_LiquidMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_LiquidMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (m_LiquidMeshFilter == null)
                m_LiquidMeshFilter = gameObject.AddComponent<MeshFilter>();

            m_LiquidMesh = GenerateLiquidMesh(width, length, cellSize);

            //m_LiquidMeshRenderer.sharedMaterial = liquidMaterial;
            m_LiquidMeshFilter.sharedMesh = m_LiquidMesh;
        }

        private static Mesh GenerateLiquidMesh(float width, float length, float cellSize)
        {
            int xsize = Mathf.RoundToInt(width / cellSize);
            int ysize = Mathf.RoundToInt(length / cellSize);

            Mesh mesh = new Mesh();

            List<Vector3> vertexList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<Vector3> normalList = new List<Vector3>();
            List<int> sub0indexList = new List<int>();
            float xcellsize = width / xsize;
            float uvxcellsize = 1.0f / xsize;
            float ycellsize = length / ysize;
            float uvycellsize = 1.0f / ysize;

            for (int i = 0; i <= ysize; i++)
            {
                for (int j = 0; j <= xsize; j++)
                {
                    vertexList.Add(new Vector3(-width * 0.5f + j * xcellsize, 0, -length * 0.5f + i * ycellsize));
                    uvList.Add(new Vector2(j * uvxcellsize, i * uvycellsize));
                    normalList.Add(Vector3.up);

                    if (i < ysize && j < xsize)
                    {
                        sub0indexList.Add(i * (xsize + 1) + j);
                        sub0indexList.Add((i + 1) * (xsize + 1) + j);
                        sub0indexList.Add((i + 1) * (xsize + 1) + j + 1);

                        sub0indexList.Add(i * (xsize + 1) + j);
                        sub0indexList.Add((i + 1) * (xsize + 1) + j + 1);
                        sub0indexList.Add(i * (xsize + 1) + j + 1);
                    }
                }
            }

            mesh.SetVertices(vertexList);
            mesh.SetUVs(0, uvList);
            mesh.SetNormals(normalList);
            mesh.SetTriangles(sub0indexList, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }

        void OnDestroy()
        {
            if (m_LiquidMesh)
                Object.Destroy(m_LiquidMesh);
            
            m_LiquidMesh = null;
        }
    }
}

///// <summary>
///// 液体渲染器
///// </summary>
//public class LiquidRenderer : MonoBehaviour
//{
//    #region public:
//    /// <summary>
//    /// 网格大小
//    /// </summary>
//    public float cellSize;

//    //public float offset;

//    public float waveOffset;
//    /// <summary>
//    /// 水体宽度
//    /// </summary>
//    public float width;
//    /// <summary>
//    /// 水体长度
//    /// </summary>
//    public float length;
//    /// <summary>
//    /// 水体深度
//    /// </summary>
//    public float depth;
//    /// <summary>
//    /// 粘度系数
//    /// </summary>
//    public float viscosity;
//    /// <summary>
//    /// 波速
//    /// </summary>
//    public float velocity;
//    /// <summary>
//    /// 力度系数
//    /// </summary>
//    public float forceFactor;

//    public float fade = 1.0f;

//    public float specular;

//    public float gloss;

//    public float refract;

//    public float height;

//    public Vector2 range;

//    public Color shallowColor;
//    public Color deepColor;

//    public LayerMask interactLayer;

//    public bool IsSupported
//    {
//        get { return m_IsSupported; }
//    }

//    public RenderTexture HeightMap
//    {
//        get { return m_Camera ? m_Camera.HeightMap : null; }
//    }

//    public RenderTexture NormalMap
//    {
//        get { return m_Camera ? m_Camera.NormalMap : null; }
//    }
//    #endregion

//    private bool m_IsSupported;

//    private LiquidSampleCamera m_Camera;
//    private LiquidGeometry m_Geometry;


//    void Start ()
//    {
//        float offset = waveOffset;
//        m_IsSupported = CheckSupport(offset);
//        if (!m_IsSupported)
//        {
//            Debug.LogError("初始化失败");
//            return;
//        }

//        //waveOffset = 1.0f/waveOffset;

//        float fac = velocity * velocity * 0.02f * 0.02f / (offset * offset);
//        float i = viscosity * 0.02f - 2;
//        float j = viscosity * 0.02f + 2;

//        float k1 = (4 - 8 * fac) / (j);
//        float k2 = i / j;
//        float k3 = 2 * fac / j;

//        m_Camera = new GameObject("[SampleCamera]").AddComponent<LiquidSampleCamera>();

//        m_Camera.Init(interactLayer, width, length, depth, forceFactor, fade,
//            new Vector4(transform.up.x, transform.up.y, transform.up.z,
//                -Vector3.Dot(transform.up, transform.position)), new Vector4(k1, k2, k3, offset), 512);
//        m_Camera.transform.SetParent(transform);
//        m_Camera.transform.localPosition = Vector3.zero;
//        m_Camera.transform.localEulerAngles = new Vector3(90, 0, 0);

//        m_Geometry = new LiquidGeometry(gameObject, cellSize, width, length, depth);
//        m_Geometry.SetLiquidHeightMap(m_Camera.HeightMap);
//        m_Geometry.SetLiquidNormalMap(m_Camera.NormalMap);
//        m_Geometry.SetLiquidReflectMap(m_Camera.ReflectMap);

//        m_Geometry.LiquidMaterial.SetFloat("_Specular", specular);
//        m_Geometry.LiquidMaterial.SetFloat("_Gloss", gloss);
//        m_Geometry.LiquidMaterial.SetFloat("_Refract", refract);
//        m_Geometry.LiquidMaterial.SetFloat("_Height", height);

//        m_Geometry.LiquidMaterial.SetVector("_Range", range);

//        m_Geometry.LiquidMaterial.SetColor("_ShallowColor", shallowColor);
//        m_Geometry.LiquidMaterial.SetColor("_DeepColor", deepColor);
//    }

//    void Update()
//    {

//    }

//    void OnDestroy()
//    {
//        if (m_Camera)
//            m_Camera.Release();
//        if (m_Geometry != null)
//            m_Geometry.Release();
//        m_Camera = null;
//        m_Geometry = null;
//    }

//    bool CheckSupport(float offset)
//    {
//        if (offset <= 0)
//        {
//            return false;
//        }
//        if (width <= 0 || length <= 0 || depth <= 0)
//        {
//            return false;
//        }
//        if (velocity < 0)
//            return false;
//        float maxV = offset / (2 * 0.02f) * Mathf.Sqrt(viscosity * 0.02f + 2);
//        if (velocity >= maxV)
//        {
//            Debug.Log(maxV.ToString("f5"));
//            Debug.LogError("波速不符合要求");
//            return false;
//        }
//        float viscositySq = viscosity * viscosity;
//        float velocitySq = velocity * velocity;
//        float deltaSizeSq = offset * offset;
//        float dt = Mathf.Sqrt(viscositySq + 32 * velocitySq / (deltaSizeSq));
//        float dtden = 8 * velocitySq / (deltaSizeSq);
//        float maxT = (viscosity + dt) / dtden;
//        float maxT2 = (viscosity - dt) / dtden;
//        if (maxT2 > 0 && maxT2 < maxT)
//            maxT = maxT2;
//        if (maxT < 0.02f)
//        {
//            Debug.LogError("时间间隔不符合要求");
//            return false;
//        }

//        return true;
//    }

//    void OnDrawGizmosSelected()
//    {
//        //LiquidUtils.DrawWirePlane(transform.position, transform.eulerAngles.y, width, length, Color.gray);

//        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, width, length, -depth, 0, Color.green);
//    }
//}
