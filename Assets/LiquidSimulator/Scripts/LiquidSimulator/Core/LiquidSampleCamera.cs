using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace ASL.LiquidSimulator
{
    public class LiquidSampleCamera : MonoBehaviour
    {
        public RenderTexture HeightMap
        {
            get
            {
                return m_HeightMap;
            }
        }
        public RenderTexture NormalMap
        {
            get
            {
                return m_NormalMap;
            }
        }

        public RenderTexture ReflectMap
        {
            get
            {
                return m_ReflectMap;
            }
        }

        private Camera m_Camera;
        private Camera m_ReflectCamera;

        private RenderTexture m_CurTexture;
        private RenderTexture m_PreTexture;
        private RenderTexture m_HeightMap;
        private RenderTexture m_NormalMap;
        private RenderTexture m_ReflectMap;

        private Shader m_ForceRenderShader;

        private Material m_WaveEquationMat;

        private Vector4 m_WaveParams;
        private Vector4 m_Plane;

        //void OnGUI()
        //{
        //    if (m_CurTexture)
        //        GUI.DrawTexture(new Rect(0, 0, 100, 100), m_HeightMap);
        //}

        void OnRenderObject()
        {
            if (m_ReflectCamera)
            {
                m_ReflectCamera.CopyFrom(Camera.main);
                m_ReflectCamera.targetTexture = m_ReflectMap;
                m_ReflectCamera.worldToCameraMatrix = m_ReflectCamera.worldToCameraMatrix * ReflectMatrix(m_Plane);
                m_ReflectCamera.projectionMatrix = ObliqueMatrix(m_Plane, Camera.main);
            }
        }

        public void Init(LayerMask interactLayer, float width, float height, float depth, float force, Vector4 plane, Vector4 waveParams, int texSize)
        {
            m_WaveParams = waveParams;
            m_Plane = plane;

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
            m_Camera.clearFlags = CameraClearFlags.Depth;
            m_Camera.allowHDR = false;

            m_ReflectCamera = new GameObject("[ReflectCamera]").AddComponent<Camera>();
            //m_ReflectCamera.hideFlags = HideFlags.HideInHierarchy;
            //m_ReflectCamera.CopyFrom(Camera.main);
            m_ReflectCamera.enabled = false;

            m_ForceRenderShader = Shader.Find("Hidden/LiquidSimulator/Force");

            m_CurTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_CurTexture.name = "[Cur]";
            m_CurTexture.format = RenderTextureFormat.ARGB32;
            m_PreTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_PreTexture.name = "[Pre]";
            m_PreTexture.format = RenderTextureFormat.ARGB32;
            m_HeightMap = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_HeightMap.name = "[HeightMap]";
            m_HeightMap.format = RenderTextureFormat.ARGB32;
            m_NormalMap = RenderTexture.GetTemporary(texSize, texSize, 16);
            m_NormalMap.format = RenderTextureFormat.ARGB32;
            m_NormalMap.anisoLevel = 1;
            m_NormalMap.name = "[NormalMap]";
            m_ReflectMap = RenderTexture.GetTemporary(1024, 1024, 16);
            m_ReflectMap.name = "[HeightMap]";

            RenderTexture tmp = RenderTexture.active;
            RenderTexture.active = m_CurTexture;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            RenderTexture.active = m_PreTexture;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            RenderTexture.active = m_HeightMap;
            GL.Clear(false, true, new Color(0, 0, 0, 0));

            RenderTexture.active = tmp;

            m_Camera.targetTexture = m_CurTexture;
            m_Camera.SetReplacementShader(m_ForceRenderShader, "RenderType");
            m_ReflectCamera.targetTexture = m_ReflectMap;

            Shader.SetGlobalFloat("internal_Force", force);

            m_WaveEquationMat = new Material(Shader.Find("Hidden/WaveEquationGen"));
            m_WaveEquationMat.SetVector("_WaveParams", m_WaveParams);
            //m_WaveEquationMat.SetFloat("_Fade", fade);
            //m_WaveEquationMat.SetFloat("_Offset", 0.02f);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {

            m_WaveEquationMat.SetTexture("_PreTex", m_PreTexture);

            Graphics.Blit(src, dst, m_WaveEquationMat, 0);

            Graphics.Blit(dst, m_HeightMap);
            //Blur(dst, m_HeightMap, 0.01f);

            RenderNormalMap(m_HeightMap, m_NormalMap);


            Graphics.Blit(src, m_PreTexture);

            GL.invertCulling = true;

            m_ReflectCamera.Render();

            GL.invertCulling = false;
        }

        private Matrix4x4 ReflectMatrix(Vector4 plane)
        {
            Matrix4x4 m = default(Matrix4x4);
            m.m00 = -2 * plane.x * plane.x + 1;
            m.m01 = -2 * plane.x * plane.y;
            m.m02 = -2 * plane.x * plane.z;
            m.m03 = -2 * plane.x * plane.w;

            m.m10 = -2 * plane.x * plane.y;
            m.m11 = -2 * plane.y * plane.y + 1;
            m.m12 = -2 * plane.y * plane.z;
            m.m13 = -2 * plane.y * plane.w;

            m.m20 = -2 * plane.z * plane.x;
            m.m21 = -2 * plane.z * plane.y;
            m.m22 = -2 * plane.z * plane.z + 1;
            m.m23 = -2 * plane.z * plane.w;

            m.m30 = 0; m.m31 = 0;
            m.m32 = 0; m.m33 = 1;
            return m;
        }

        private void RenderNormalMap(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, m_WaveEquationMat, 1);
            //RenderTexture tmp = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
            //RenderTexture tmp2 = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
            //Graphics.Blit(src, tmp, m_WaveEquationMat, 1);
            //m_WaveEquationMat.SetVector("_BlurOffset", new Vector4(0.01f, 0));
            //Graphics.Blit(tmp, tmp2, m_WaveEquationMat, 2);
            //m_WaveEquationMat.SetVector("_BlurOffset", new Vector4(0, 0.01f));
            //Graphics.Blit(tmp2, tmp, m_WaveEquationMat, 2);
            //Graphics.Blit(tmp, dst);
            //RenderTexture.ReleaseTemporary(tmp);
            //RenderTexture.ReleaseTemporary(tmp2);
        }

        private void Blur(RenderTexture src, RenderTexture dst, float offset)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
            RenderTexture tmp2 = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
   
            m_WaveEquationMat.SetVector("_BlurOffset", new Vector4(offset, 0));
            Graphics.Blit(src, tmp, m_WaveEquationMat, 2);
            m_WaveEquationMat.SetVector("_BlurOffset", new Vector4(0, offset));
            Graphics.Blit(tmp, tmp2, m_WaveEquationMat, 2);

            Graphics.Blit(tmp2, dst);

            RenderTexture.ReleaseTemporary(tmp);
            RenderTexture.ReleaseTemporary(tmp2);
        }

        private Matrix4x4 ObliqueMatrix(Vector4 plane, Camera reflectCamera)
        {
            //世界空间的平面先变换到相机空间
            plane = (reflectCamera.worldToCameraMatrix.inverse).transpose * plane;


            Matrix4x4 proj = reflectCamera.projectionMatrix;

            //计算近裁面的最远角点Q
            Vector4 q = default(Vector4);
            q.x = (Mathf.Sign(plane.x) + proj.m02) / proj.m00;
            q.y = (Mathf.Sign(plane.y) + proj.m12) / proj.m11;
            q.z = -1.0f;
            q.w = (1.0f + proj.m22) / proj.m23;
            Vector4 c = plane * (2.0f / Vector4.Dot(plane, q));

            //计算M3'
            proj.m20 = c.x;
            proj.m21 = c.y;
            proj.m22 = c.z + 1.0f;
            proj.m23 = c.w;
            return proj;
        }

        public void Release()
        {
            if (m_CurTexture)
                RenderTexture.ReleaseTemporary(m_CurTexture);
            if (m_PreTexture)
                RenderTexture.ReleaseTemporary(m_PreTexture);
            if (m_HeightMap)
                RenderTexture.ReleaseTemporary(m_HeightMap);
            if (m_NormalMap)
                RenderTexture.ReleaseTemporary(m_NormalMap);
            if (m_ReflectCamera)
                Destroy(m_ReflectCamera.gameObject);
            m_ForceRenderShader = null;
        }
    }
}