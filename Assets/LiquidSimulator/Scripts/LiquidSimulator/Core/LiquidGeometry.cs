using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.LiquidSimulator
{
    public class LiquidGeometry
    {
        public Material LiquidMaterial
        {
            get { return m_LiquidMaterial; }
        }

        private Mesh m_LiquidMesh;
        private Material m_LiquidMaterial;
        //private Material m_LiquidBodyMaterial;
        private MeshFilter m_LiquidMeshFilter;
        private MeshRenderer m_LiquidMeshRenderer;

        public LiquidGeometry(GameObject gameObject, float cellSize, float width, float length, float depth)
        {
            m_LiquidMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (m_LiquidMeshRenderer == null)
                m_LiquidMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_LiquidMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (m_LiquidMeshFilter == null)
                m_LiquidMeshFilter = gameObject.AddComponent<MeshFilter>();

            m_LiquidMesh = GenerateLiquidMesh(width, length, depth, cellSize);
            m_LiquidMaterial = new Material(Shader.Find("Unlit/Water"));
            //m_LiquidBodyMaterial = new Material(Shader.Find("Unlit/WaterBody"));

            m_LiquidMeshRenderer.sharedMaterial = m_LiquidMaterial;
            //m_LiquidMeshRenderer.sharedMaterials = new Material[] {m_LiquidMaterial, m_LiquidBodyMaterial};
            m_LiquidMeshFilter.sharedMesh = m_LiquidMesh;

        }

        public void SetLiquidHeightMap(RenderTexture heightMap)
        {
            if (m_LiquidMaterial)
                m_LiquidMaterial.SetTexture("_LiquidHeightMap", heightMap);
        }

        public void SetLiquidNormalMap(RenderTexture normalMap)
        {
            if (m_LiquidMaterial)
                m_LiquidMaterial.SetTexture("_LiquidNormalMap", normalMap);
        }

        public void SetLiquidReflectMap(RenderTexture reflectMap)
        {
            if (m_LiquidMaterial)
                m_LiquidMaterial.SetTexture("_LiquidReflectMap", reflectMap);
        }

        private static Mesh GenerateLiquidMesh(float width, float length, float depth, float cellSize)
        {
            int xsize = Mathf.RoundToInt(width/cellSize);
            int ysize = Mathf.RoundToInt(length / cellSize);

            Mesh mesh = new Mesh();

            List<Vector3> vertexList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<Vector3> normalList = new List<Vector3>();
            List<int> sub0indexList = new List<int>();
            //List<int> sub1indexList = new List<int>();
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

            //int vcount = vertexList.Count;
            //for (int i = 0; i <= ysize; i++)
            //{
            //    vertexList.Add(new Vector3(-width*0.5f, -depth, -length*0.5f + i*ycellsize));
            //    vertexList.Add(new Vector3(-width*0.5f, 0, -length * 0.5f + i * ycellsize));

            //    vertexList.Add(new Vector3(width * 0.5f, -depth, -length * 0.5f + i * ycellsize));
            //    vertexList.Add(new Vector3(width * 0.5f, 0, -length * 0.5f + i * ycellsize));

            //    uvList.Add(new Vector2(i * uvycellsize, 0));
            //    uvList.Add(new Vector2(i * uvycellsize, 1));

            //    uvList.Add(new Vector2(i * uvycellsize, 0));
            //    uvList.Add(new Vector2(i * uvycellsize, 1));

            //    normalList.Add(Vector3.left);
            //    normalList.Add(Vector3.left);

            //    normalList.Add(Vector3.right);
            //    normalList.Add(Vector3.right);

            //    if (i < ysize)
            //    {
            //        sub1indexList.Add(vcount + i * 4 + 0);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 1);
            //        sub1indexList.Add(vcount + i * 4 + 1);

            //        sub1indexList.Add(vcount + i * 4 + 0);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 0);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 1);

            //        sub1indexList.Add(vcount + i * 4 + 2);
            //        sub1indexList.Add(vcount + i * 4 + 3);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 3);

            //        sub1indexList.Add(vcount + i * 4 + 2);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 3);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 2);
            //    }
            //}

            //vcount = vertexList.Count;
            //for (int i = 0; i <= xsize; i++)
            //{
            //    vertexList.Add(new Vector3(-width * 0.5f + i * xcellsize, -depth, -length * 0.5f));
            //    vertexList.Add(new Vector3(-width * 0.5f + i * xcellsize, 0, -length * 0.5f));

            //    vertexList.Add(new Vector3(-width * 0.5f + i * xcellsize, -depth, length * 0.5f));
            //    vertexList.Add(new Vector3(-width * 0.5f + i * xcellsize, 0, length * 0.5f));

            //    uvList.Add(new Vector2(i * uvxcellsize, 0));
            //    uvList.Add(new Vector2(i * uvxcellsize, 0));

            //    uvList.Add(new Vector2(i * uvxcellsize, 0));
            //    uvList.Add(new Vector2(i * uvxcellsize, 0));

            //    normalList.Add(Vector3.back);
            //    normalList.Add(Vector3.back);

            //    normalList.Add(Vector3.forward);
            //    normalList.Add(Vector3.forward);

            //    if (i < xsize)
            //    {
            //        sub1indexList.Add(vcount + i * 4 + 0);
            //        sub1indexList.Add(vcount + i * 4 + 1);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 1);

            //        sub1indexList.Add(vcount + i * 4 + 0);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 1);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 0);

            //        sub1indexList.Add(vcount + i * 4 + 2);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 3);
            //        sub1indexList.Add(vcount + i * 4 + 3);

            //        sub1indexList.Add(vcount + i * 4 + 2);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 2);
            //        sub1indexList.Add(vcount + (i + 1) * 4 + 3);
            //    }
            //}

            //mesh.subMeshCount = 2;
            mesh.SetVertices(vertexList);
            mesh.SetUVs(0, uvList);
            mesh.SetNormals(normalList);
            mesh.SetTriangles(sub0indexList, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            //mesh.SetTriangles(sub1indexList, 1);
            
            return mesh;
        }

        public void Release()
        {
            if (m_LiquidMaterial)
                Object.Destroy(m_LiquidMaterial);
            //if (m_LiquidBodyMaterial)
            //    Object.Destroy(m_LiquidBodyMaterial);
            if (m_LiquidMesh)
                Object.Destroy(m_LiquidMesh);
            
            m_LiquidMaterial = null;
            m_LiquidMesh = null;
        }
    }
}