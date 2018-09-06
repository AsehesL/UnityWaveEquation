using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.LiquidSimulator
{
    public class LiquidGeometry
    {

        private Mesh m_LiquidMesh;
        private Material m_LiquidMaterial;
        private MeshFilter m_LiquidMeshFilter;
        private MeshRenderer m_LiquidMeshRenderer;

        public LiquidGeometry(GameObject gameObject, float cellSize, float width, float length)
        {
            m_LiquidMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (m_LiquidMeshRenderer == null)
                m_LiquidMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_LiquidMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (m_LiquidMeshFilter == null)
                m_LiquidMeshFilter = gameObject.AddComponent<MeshFilter>();

            m_LiquidMesh = GenerateLiquidMesh(width, length, cellSize);
            m_LiquidMaterial = new Material(Shader.Find("Unlit/Water"));

            m_LiquidMeshRenderer.sharedMaterial = m_LiquidMaterial;
            m_LiquidMeshFilter.sharedMesh = m_LiquidMesh;
        }

        public void SetLiquidHeightMap(RenderTexture heightMap)
        {
            if (m_LiquidMaterial)
                m_LiquidMaterial.SetTexture("_LiquidHeightMap", heightMap);
        }

        private static Mesh GenerateLiquidMesh(float width, float length, float cellSize)
        {
            int xsize = Mathf.RoundToInt(width/cellSize);
            int ysize = Mathf.RoundToInt(length / cellSize);

            Mesh mesh = new Mesh();

            List<Vector3> vertexList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<Vector3> normalList = new List<Vector3>();
            List<int> indexList = new List<int>();
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
                        indexList.Add(i * (xsize + 1) + j);
                        indexList.Add((i + 1) * (xsize + 1) + j);
                        indexList.Add((i + 1) * (xsize + 1) + j + 1);

                        indexList.Add(i * (xsize + 1) + j);
                        indexList.Add((i + 1) * (xsize + 1) + j + 1);
                        indexList.Add(i * (xsize + 1) + j + 1);
                    }
                }
            }

            mesh.SetVertices(vertexList);
            mesh.SetUVs(0, uvList);
            mesh.SetNormals(normalList);
            mesh.SetTriangles(indexList, 0);
            return mesh;
        }

        public void Release()
        {
            if (m_LiquidMaterial)
                Object.Destroy(m_LiquidMaterial);
            if (m_LiquidMesh)
                Object.Destroy(m_LiquidMesh);
            m_LiquidMaterial = null;
            m_LiquidMesh = null;
        }
    }
}