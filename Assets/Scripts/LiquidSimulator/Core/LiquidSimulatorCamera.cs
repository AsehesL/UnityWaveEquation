using UnityEngine;
using System.Collections;

public class LiquidSimulatorCamera : MonoBehaviour
{
    private Camera m_Camera;

    private Shader m_ForceRenderShader;
    public Material m_WaveEquationMat;

    public RenderTexture m_CurTexture;
    public RenderTexture m_PreTexture;

    private Vector4 m_WaveParams;

    private Material m_TargetMaterial;
    
    void Update()
    {

    }


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
        m_Camera.clearFlags = CameraClearFlags.Nothing;
        m_Camera.allowHDR = false;

        m_ForceRenderShader = Shader.Find("Hidden/ForceRender");

        m_Camera.SetReplacementShader(m_ForceRenderShader, "RenderType");

        m_CurTexture = new RenderTexture(texSize, texSize, 16);
        m_CurTexture.name = "[Cur]";
        m_PreTexture = RenderTexture.GetTemporary(texSize, texSize, 16);
        m_PreTexture.name = "[Pre]";


        RenderTexture tmp = RenderTexture.active;
        RenderTexture.active = m_CurTexture;
        GL.Clear(false, true, new Color(0, 0, 0, 0));
        RenderTexture.active = m_PreTexture;
        GL.Clear(false, true, new Color(0, 0, 0, 0));

        RenderTexture.active = tmp;

        m_Camera.targetTexture = m_CurTexture;
        
        Shader.SetGlobalFloat("internal_Force", force);

        m_WaveEquationMat = new Material(Shader.Find("Hidden/WaveEquationGen"));
        m_WaveEquationMat.SetVector("_WaveParams", m_WaveParams);

        m_TargetMaterial = material;
        m_TargetMaterial.SetTexture("_MainTex", m_Camera.targetTexture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        m_WaveEquationMat.SetTexture("_PreTex", m_PreTexture);

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
        if (m_CurTexture)
            Destroy(m_CurTexture);
        if (m_PreTexture)
            RenderTexture.ReleaseTemporary(m_PreTexture);
        m_ForceRenderShader = null;
    }
}
