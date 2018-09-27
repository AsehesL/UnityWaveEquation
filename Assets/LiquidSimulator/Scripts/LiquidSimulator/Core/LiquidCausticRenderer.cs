using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ASL.LiquidSimulator
{
    /// <summary>
    /// 液体焦散渲染器
    /// </summary>
    public class LiquidCausticRenderer : MonoBehaviour
    {

        //private MeshRenderer m_MeshRenderer;
        //private MeshFilter m_MeshFilter;

        private Mesh m_Mesh;
        private Material m_Material;

        private Camera m_Camera;

        private RenderTexture m_RenderTexture;
        private CommandBuffer m_CommandBuffer;

        private float m_Width;
        private float m_Height;

        public void SetLiquidHeightMap(RenderTexture heightMap)
        {
            if (m_Material)
                m_Material.SetTexture("_LiquidHeightMap", heightMap);
        }

        public void SetLiquidNormalMap(RenderTexture normalMap)
        {
            if (m_Material)
                m_Material.SetTexture("_LiquidNormalMap", normalMap);
        }

        public void SetLiquidMaterial(Material liquidMaterial)
        {
            m_Material = liquidMaterial;
        }

        public void Init(float cellSize, float width, float length)
        {
            m_Camera = gameObject.AddComponent<Camera>();
            m_Camera.aspect = width / length;
            m_Camera.backgroundColor = Color.black;
            //m_Camera.enabled = false;
            m_Camera.depth = 0;
            m_Camera.farClipPlane = 10;
            m_Camera.nearClipPlane = -10;
            m_Camera.orthographic = true;
            m_Camera.orthographicSize = length * 0.5f * 1.2f;
            //m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.allowHDR = false;
            m_Camera.backgroundColor = Color.black;
            m_Camera.cullingMask = 0;

            m_Width = width * 0.5f * 1.2f;
            m_Height = length * 0.5f * 1.2f;

            m_RenderTexture = RenderTexture.GetTemporary(512, 512, 16);
            m_RenderTexture.name = "[Caustic]";
            m_Camera.targetTexture = m_RenderTexture;

            m_CommandBuffer = new CommandBuffer();
            m_CommandBuffer.name = "[Caustic CB]";
            m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_CommandBuffer);

           //m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
           //if (m_MeshRenderer == null)
           //    m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
           //m_MeshFilter = gameObject.GetComponent<MeshFilter>();
           //if (m_MeshFilter == null)
           //    m_MeshFilter = gameObject.AddComponent<MeshFilter>();

           m_Mesh = LiquidUtils.GenerateLiquidMesh(width, length, cellSize);


            //m_MeshFilter.sharedMesh = m_Mesh;
        }

        void OnGUI()
        {
            if (m_RenderTexture)
                GUI.DrawTexture(new Rect(0, 0, 100, 100), m_RenderTexture);
        }

        void OnPostRender()
        {
            Matrix4x4 trs = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            m_CommandBuffer.Clear();
            m_CommandBuffer.ClearRenderTarget(true, true, Color.black);

            m_CommandBuffer.SetRenderTarget(m_RenderTexture);

            m_CommandBuffer.DrawMesh(m_Mesh, trs, m_Material);

            Vector4 plane = new Vector4(0, -1, 0, Vector3.Dot(new Vector3(0, -1, 0), transform.position));
            Vector4 range = new Vector4(transform.position.x, transform.position.z, m_Width, m_Height);

            Shader.SetGlobalVector("_CausticPlane", plane);
            Shader.SetGlobalVector("_CausticRange", range);
            Shader.SetGlobalTexture("_CausticMap", m_RenderTexture);
        }

        void OnDestroy()
        {
            if(m_RenderTexture)
                Destroy(m_RenderTexture);
            if(m_Mesh)
                Destroy(m_Mesh);
            if (m_CommandBuffer != null)
            {
                m_CommandBuffer.Release();
                m_CommandBuffer = null;
            }
        }
    }
}