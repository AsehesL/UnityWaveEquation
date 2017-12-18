using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEquationTest1D : MonoBehaviour
{

    public int size;

    public float height;

    public float c;
    public float u;
    public float force;

    private List<Vector3> m_VertexList = new List<Vector3>();
    private List<Vector3> m_LastVertexList = new List<Vector3>();
    private List<Vector3> m_NextVertexList = new List<Vector3>();
    private List<int> m_Indexes = new List<int>();

    private Material m_Material;
    private Mesh m_Mesh;

    private float m_MinY;
    private float m_MaxY;
    private float m_MinX = Mathf.Infinity;
    private float m_MaxX = -Mathf.Infinity;

    private float m_K1;
    private float m_K2;
    private float m_K3;

    private float m_D;

	void Start ()
	{
	    MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
	    MeshFilter mf = gameObject.AddComponent<MeshFilter>();

	    m_Material = new Material(Shader.Find("Diffuse"));
	    m_Mesh = new Mesh();

	    mr.sharedMaterial = m_Material;
	    mf.sharedMesh = m_Mesh;

        //m_D = Camera.main.orthographicSize * Camera.main.aspect / size;

	    for (int i = 0; i <= size; i++)
	    {
	        Vector3 p1 = new Vector3(-1 + 2.0f/size*i, -1, 0);
	        Vector3 p2 = new Vector3(-1 + 2.0f/size*i, height, 0);
	        p1 = Camera.main.projectionMatrix.inverse.MultiplyPoint(p1);
	        p1 = Camera.main.cameraToWorldMatrix.MultiplyPoint(p1);
	        
            p2 = Camera.main.projectionMatrix.inverse.MultiplyPoint(p2);
            p2 = Camera.main.cameraToWorldMatrix.MultiplyPoint(p2);

	        if (p1.y > p2.y)
	        {
	            m_MinY = p2.y;
	            m_MaxY = p1.y;
	        }
	        else
	        {
                m_MinY = p1.y;
                m_MaxY = p2.y;
            }
            if (p1.x < m_MinX)
                m_MinX = p1.x;
            if (p1.x > m_MaxX)
                m_MaxX = p1.x;

            p1 = transform.worldToLocalMatrix.MultiplyPoint(p1);
            p2 = transform.worldToLocalMatrix.MultiplyPoint(p2);
            m_VertexList.Add(p1);
            m_VertexList.Add(p2);
	        m_LastVertexList.Add(p1);
	        m_LastVertexList.Add(p2);
	        m_NextVertexList.Add(p1);
	        m_NextVertexList.Add(p2);

            if (i < size)
	        {
	            m_Indexes.Add(i*2);
                m_Indexes.Add(i * 2+1);
                m_Indexes.Add((i+1) * 2+1);

                m_Indexes.Add(i * 2);
                m_Indexes.Add((i+1) * 2+1);
                m_Indexes.Add((i+1) * 2);
            }
	    }

	    m_Mesh.SetVertices(m_VertexList);
	    m_Mesh.SetTriangles(m_Indexes, 0);

	    m_D = (m_MaxX - m_MinX)/size;

	    Debug.Log("maxY:" + m_MaxY + ",minY:" + m_MinY);
        Debug.Log("maxX:" + m_MaxX + ",minX:" + m_MinX);
	    Debug.Log("D:" + m_D);


        //m_K1 = (4 - 4*Time.fixedDeltaTime*Time.fixedDeltaTime*c*c/(m_D*m_D))/(2 + u*Time.fixedDeltaTime);
        //m_K2 = (u*Time.fixedDeltaTime - 2)/(u*Time.fixedDeltaTime + 2);
        //m_K3 = (4*Time.fixedDeltaTime*Time.fixedDeltaTime*c*c/(m_D*m_D))/(u*Time.fixedDeltaTime + 2);

	    m_K1 = (4*Time.fixedDeltaTime*Time.fixedDeltaTime/(m_D*m_D) - 4*c*c)/(u*Time.fixedDeltaTime - 2*c*c);
	    m_K2 = (2*c*c + u*Time.fixedDeltaTime)/(u*Time.fixedDeltaTime - 2*c*c);
	    m_K3 = 2*Time.fixedDeltaTime*Time.fixedDeltaTime/(m_D*m_D)/(2*c*c - u*Time.fixedDeltaTime);
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AddForce(worldPos);
        }
    }

    void FixedUpdate()
    {
        for (int i = 1; i < size; i++)
        {
            float cy = m_VertexList[i * 2 + 1].y;
            float ly = m_LastVertexList[i * 2 + 1].y;
            float cy1 = m_VertexList[(i - 1) * 2 + 1].y;
            float cy2 = m_VertexList[(i + 1) * 2 + 1].y;

            float ny = cy * m_K1 + ly * m_K2 + (cy1 + cy2) * m_K3;

            Vector3 p = m_NextVertexList[i*2 + 1];
            p.y = ny;
            m_NextVertexList[i*2 + 1] = p;
        }
        for (int i = 0; i <= size; i++)
        {
            m_LastVertexList[i*2 + 1] = m_VertexList[i*2 + 1];
            m_VertexList[i*2 + 1] = m_NextVertexList[i*2 + 1];
        }
        m_Mesh.Clear();
        m_Mesh.SetVertices(m_VertexList);
        m_Mesh.SetTriangles(m_Indexes, 0);
    }

    void OnDestroy()
    {
        if (m_Material)
            Destroy(m_Material);
        m_Material = null;
        if (m_Mesh)
            Destroy(m_Mesh);
        m_Mesh = null;
    }

    private void AddForce(Vector3 position)
    {
        int hit = Mathf.RoundToInt((position.x - m_MinX)/m_D);
        if (hit >= 0 && hit <= size)
        {
            Vector3 p = m_VertexList[hit*2 + 1];
            p.y = force;
            m_VertexList[hit * 2 + 1] = p;
            //m_Mesh.SetVertices(m_VertexList);
            //m_Mesh.SetTriangles(m_Indexes, 0);
        }
    }
}
