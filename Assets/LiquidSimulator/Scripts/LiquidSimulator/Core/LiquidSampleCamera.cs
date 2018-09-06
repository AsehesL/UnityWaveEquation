using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace ASL.LiquidSimulator
{
    public class LiquidSampleCamera : MonoBehaviour
    {
        //private CommandBuffer m_CommandBuffer;
        public RenderTexture HeightMap
        {
            get
            {
                return m_HeightMap;
            }
        }

        private Camera m_Camera;

        private RenderTexture m_CurTexture;
        private RenderTexture m_PreTexture;
        private RenderTexture m_HeightMap;

        private Shader m_ForceRenderShader;

        private Material m_WaveEquationMat;
        //private Material m_TargetMaterial;

        private Vector4 m_WaveParams;

        void OnGUI()
        {
            if (m_CurTexture)
                GUI.DrawTexture(new Rect(0, 0, 100, 100), m_CurTexture);
        }

        public void Init(LayerMask interactLayer, float width, float height, float depth, float force, float fade, Vector4 waveParams, int texSize)
        {
            m_WaveParams = waveParams;

            m_Camera = gameObject.AddComponent<Camera>();
            m_Camera.aspect = width/height;
            m_Camera.backgroundColor = Color.black;
            m_Camera.cullingMask = interactLayer;
            m_Camera.depth = 0;
            m_Camera.farClipPlane = depth;
            m_Camera.nearClipPlane = 0;
            m_Camera.orthographic = true;
            m_Camera.orthographicSize = height * 0.5f;
            //m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.clearFlags = CameraClearFlags.Nothing;
            m_Camera.allowHDR = false;

            m_ForceRenderShader = Shader.Find("Hidden/LiquidSimulator/Force");

            m_CurTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_CurTexture.name = "[Cur]";
            m_PreTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_PreTexture.name = "[Pre]";
            m_HeightMap = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_HeightMap.name = "[HeightMap]";

            RenderTexture tmp = RenderTexture.active;
            RenderTexture.active = m_CurTexture;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            RenderTexture.active = m_PreTexture;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            RenderTexture.active = m_HeightMap;
            GL.Clear(false, true, new Color(0, 0, 0, 0));

            RenderTexture.active = tmp;

            m_Camera.targetTexture = m_CurTexture;

            Shader.SetGlobalFloat("internal_Force", force);

            m_WaveEquationMat = new Material(Shader.Find("Hidden/WaveEquationGen"));
            m_WaveEquationMat.SetVector("_WaveParams", m_WaveParams);
            m_WaveEquationMat.SetFloat("_Fade", fade);
            m_WaveEquationMat.SetFloat("_Offset", 0.01f);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {

            m_WaveEquationMat.SetTexture("_PreTex", m_PreTexture);

            Graphics.Blit(src, dst, m_WaveEquationMat, 0);
            Graphics.Blit(dst, m_HeightMap, m_WaveEquationMat, 1);


            Graphics.Blit(src, m_PreTexture);
        }

        public void Release()
        {
            if (m_CurTexture)
                RenderTexture.ReleaseTemporary(m_CurTexture);
            if (m_PreTexture)
                RenderTexture.ReleaseTemporary(m_PreTexture);
            if (m_HeightMap)
                RenderTexture.ReleaseTemporary(m_HeightMap);
            //if (m_CommandBuffer != null)
            //{
            //    if (m_Camera)
            //        m_Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_CommandBuffer);
            //    m_CommandBuffer.Release();
            //    m_CommandBuffer = null;
            //}
            m_ForceRenderShader = null;
        }
    }
}