using UnityEngine;
using System.Collections;

public class LiquidSimulatorCamera : MonoBehaviour
{
    private Camera m_Camera;

    private Shader m_ForceRenderShader;
    public Material m_WaveEquationMat;

    //private RenderTexture m_targetTexture;
    //private RenderTexture m_cacheTexture;

    public RenderTexture m_CurTexture;
    public RenderTexture m_PreTexture;
    //public RenderTexture m_NextTexture;

    public float fade;

    private Vector4 m_WaveParams;

    private Material m_TargetMaterial;

    //private Material m_ForceAddMaterial;
    
    void Update()
    {

    }

    //void OnGUI()
    //{
    //    if (m_targetTexture)
    //        GUI.DrawTexture(new Rect(0, 0, 100, 100), m_targetTexture);
    //}


    public void Init(LayerMask interactLayer, float size, float near, float far, float force, Vector4 waveParams, int texSize, Material material)
    {
        Debug.Log(waveParams);

        m_WaveParams = waveParams;

        m_Camera = gameObject.AddComponent<Camera>();
        m_Camera.aspect = 1;
        m_Camera.backgroundColor = Color.black;
        m_Camera.cullingMask = interactLayer;
        m_Camera.depth = 0;
        m_Camera.farClipPlane = far;
        m_Camera.nearClipPlane = near;
        m_Camera.orthographic = true;
        m_Camera.orthographicSize = size;
        //m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.clearFlags = CameraClearFlags.Nothing;
        m_Camera.allowHDR = false;

        m_ForceRenderShader = Shader.Find("Hidden/ForceRender");

        m_Camera.SetReplacementShader(m_ForceRenderShader, "RenderType");

        m_CurTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
        m_CurTexture.name = "[Cur]";
        m_PreTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
        m_PreTexture.name = "[Pre]";

        //m_targetTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
        //m_targetTexture.name = "[Tar]";
        //m_cacheTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
        //m_cacheTexture.name = "[Ca]";
        //m_NextTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
        //m_NextTexture.name = "[Ca]";


        RenderTexture tmp = RenderTexture.active;
        RenderTexture.active = m_CurTexture;
        GL.Clear(false, true, new Color(0, 0, 0, 0));
        RenderTexture.active = m_PreTexture;
        GL.Clear(false, true, new Color(0, 0, 0, 0));
        //RenderTexture.active = m_targetTexture;
        //GL.Clear(false, true, new Color(0, 0, 0, 0));
        //RenderTexture.active = m_cacheTexture;
        //GL.Clear(false, true, new Color(0, 0, 0, 0));
        //RenderTexture.active = m_NextTexture;
        //GL.Clear(false, true, new Color(0, 0, 0, 0));

        RenderTexture.active = tmp;

        //m_Camera.targetTexture = m_targetTexture;
        m_Camera.targetTexture = m_CurTexture;
        
        Shader.SetGlobalFloat("internal_Force", force);

        m_WaveEquationMat = new Material(Shader.Find("Hidden/WaveEquationGen"));
        m_WaveEquationMat.SetVector("_WaveParams", m_WaveParams);

        //m_ForceAddMaterial = new Material(Shader.Find("Hidden/ForceDet"));

        m_TargetMaterial = material;
        //m_TargetMaterial.SetTexture("_MainTex", m_CurTexture);
        m_TargetMaterial.SetTexture("_MainTex", m_Camera.targetTexture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //m_ForceAddMaterial.SetTexture("_CacheTex", m_cacheTexture);
        //m_ForceAddMaterial.SetFloat("_Fade", fade);

        //Graphics.Blit(src, dst, m_ForceAddMaterial);


        //Graphics.Blit(src, m_cacheTexture);

        //m_WaveEquationMat.SetTexture("_PreTex", m_PreTexture);
        //m_WaveEquationMat.SetTexture("_ForceTex", dst);

        //Graphics.Blit(m_CurTexture, m_NextTexture, m_WaveEquationMat);


        //RenderTexture tmp = m_PreTexture;
        //m_PreTexture = m_CurTexture;
        //m_CurTexture = m_NextTexture;
        //m_NextTexture = tmp;

        //m_TargetMaterial.SetTexture("_MainTex", m_CurTexture);

		m_WaveEquationMat.SetTexture("_PreTex", m_PreTexture);
		m_WaveEquationMat.SetFloat("_Fade", fade);

        Graphics.Blit(src, dst, m_WaveEquationMat);
        

        Graphics.Blit(src, m_PreTexture);
    }

    //void OnPostRender()
    //{
    //    m_WaveEquationMat.SetTexture("_CurTex", m_ForceTextures[1]);
    //    m_WaveEquationMat.SetTexture("_PreTex", m_ForceTextures[0]);

    //    m_ForceTextures[2].DiscardContents();
    //    Graphics.Blit(m_ForceTextures[0], m_ForceTextures[2], m_WaveEquationMat);

    //    SwapRenderTexture();
    //}

    //void SwapRenderTexture()
    //{

    //    RenderTexture pre = m_ForceTextures[0];
    //    m_ForceTextures[0] = m_ForceTextures[1];
    //    m_ForceTextures[1] = m_ForceTextures[2];
    //    m_ForceTextures[2] = pre;
    //    m_Camera.targetTexture = m_ForceTextures[1];


    //    m_TargetMaterial.SetTexture("_MainTex", m_Camera.targetTexture);
    //}

    public void Release()
    {
        //if (m_ForceTextures!=null)
        //{
        //    for (int i = 0; i < m_ForceTextures.Length; i++)
        //    {
        //        if (m_ForceTextures[i])
        //            Destroy(m_ForceTextures[i]);
        //        m_ForceTextures[i] = null;
        //    }
        //}
        //m_ForceTextures = null;
        //if (m_targetTexture)
        //    RenderTexture.ReleaseTemporary(m_targetTexture);
        //if (m_cacheTexture)
        //    RenderTexture.ReleaseTemporary(m_cacheTexture);
        if (m_CurTexture)
            RenderTexture.ReleaseTemporary(m_CurTexture);
        if (m_PreTexture)
            RenderTexture.ReleaseTemporary(m_PreTexture);
        //if (m_NextTexture)
        //    RenderTexture.ReleaseTemporary(m_NextTexture);
        m_ForceRenderShader = null;
    }
}
