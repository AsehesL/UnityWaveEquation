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

            m_LiquidMesh = LiquidUtils.GenerateLiquidMesh(width, length, cellSize);

            //m_LiquidMeshRenderer.sharedMaterial = liquidMaterial;
            m_LiquidMeshFilter.sharedMesh = m_LiquidMesh;
        }

        void OnDestroy()
        {
            if (m_LiquidMesh)
                Object.Destroy(m_LiquidMesh);
            
            m_LiquidMesh = null;
        }
    }
}
