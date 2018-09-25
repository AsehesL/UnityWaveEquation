using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.LiquidSimulator
{
    /// <summary>
    /// 液体焦散渲染器
    /// </summary>
    public class LiquidCausticRenderer : MonoBehaviour
    {

        private MeshRenderer m_MeshRenderer;
        private MeshFilter m_MeshFilter;

        private Mesh m_Mesh;

        public void SetLiquidHeightMap(RenderTexture heightMap)
        {
            if (m_MeshRenderer && m_MeshRenderer.sharedMaterial)
                m_MeshRenderer.sharedMaterial.SetTexture("_LiquidHeightMap", heightMap);
        }

        public void SetLiquidNormalMap(RenderTexture normalMap)
        {
            if (m_MeshRenderer && m_MeshRenderer.sharedMaterial)
                m_MeshRenderer.sharedMaterial.SetTexture("_LiquidNormalMap", normalMap);
        }

        public void SetLiquidMaterial(Material liquidMaterial)
        {
            if (m_MeshRenderer)
                m_MeshRenderer.sharedMaterial = liquidMaterial;
        }

        public void Init(float cellSize, float width, float length)
        {
            m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (m_MeshRenderer == null)
                m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_MeshFilter = gameObject.GetComponent<MeshFilter>();
            if (m_MeshFilter == null)
                m_MeshFilter = gameObject.AddComponent<MeshFilter>();

            m_Mesh = LiquidUtils.GenerateLiquidMesh(width, length, cellSize);

            m_MeshFilter.sharedMesh = m_Mesh;
        }
    }
}