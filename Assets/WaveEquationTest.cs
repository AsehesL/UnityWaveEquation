using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEquationTest : MonoBehaviour
{
    public float range;
    public float force;

    public float viscosity;
    public float velocity;
    public float offset;
    public int size;

    private float m_DeltaSize;

    private Material m_ForceMat;
    private Material m_WaveMat;

    public RenderTexture[] m_RenderTextures;

	void Start ()
	{
	    Camera cam = gameObject.GetComponent<Camera>();
	    cam.SetReplacementShader(Shader.Find("Unlit/ForceR"), "RenderType");

 	    m_ForceMat = new Material(Shader.Find("Hidden/ForceTest"));
	    m_WaveMat = new Material(Shader.Find("Hidden/WaveTest"));
	    m_ForceMat.SetFloat("_Force", force);
	    m_ForceMat.SetFloat("_ForceRange", range);

        m_RenderTextures = new RenderTexture[3];

	    m_RenderTextures[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        m_RenderTextures[1] = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        m_RenderTextures[2] = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);

	    RenderTexture tmp = RenderTexture.active;
	    RenderTexture.active = m_RenderTextures[0];
	    GL.Clear(false, true, Color.black);
        RenderTexture.active = m_RenderTextures[1];
        GL.Clear(false, true, Color.black);
        RenderTexture.active = m_RenderTextures[2];
        GL.Clear(false, true, Color.black);
        RenderTexture.active = tmp;



        m_DeltaSize = 1.0f / size;
	    if (!CheckSupport())
	    {
	        return;
	    }

        float fac = velocity * velocity * 0.02f * 0.02f / (m_DeltaSize * m_DeltaSize);
        float i = viscosity * 0.02f - 2;
        float j = viscosity * 0.02f + 2;

        float k1 = (4 - 8 * fac) / (j);
        float k2 = i / j;
        float k3 = 2 * fac / j;

	    m_WaveMat.SetVector("_WaveParams", new Vector4(k1, k2, k3, offset));
	}

    void OnDestroy()
    {
        for (int i = 0; i < m_RenderTextures.Length; i++)
        {
            if (m_RenderTextures[i])
                RenderTexture.ReleaseTemporary(m_RenderTextures[i]);
            m_RenderTextures[i] = null;
        }
        m_RenderTextures = null;
    }
	
	void Update () {
	    if (Input.GetMouseButton(0))
	    {
	        float x = Input.mousePosition.x/Screen.width;
	        float y = Input.mousePosition.y/Screen.height;
	        m_ForceMat.SetVector("_ForcePos", new Vector2(x, y));

	        RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
	        Graphics.Blit(m_RenderTextures[0], rt, m_ForceMat);
	        RenderTexture.ReleaseTemporary(m_RenderTextures[0]);
            m_RenderTextures[0] = rt;
	    }
	}

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //Graphics.Blit(src, m_RenderTextures[0]);

        Graphics.Blit(m_RenderTextures[0], dst);

        m_WaveMat.SetTexture("_CurTex", m_RenderTextures[0]);
        m_WaveMat.SetTexture("_PreTex", m_RenderTextures[1]);

        Graphics.Blit(m_RenderTextures[0], m_RenderTextures[2], m_WaveMat);

        RenderTexture pre = m_RenderTextures[0];
        m_RenderTextures[0] = m_RenderTextures[2];
        m_RenderTextures[2] = m_RenderTextures[1];
        m_RenderTextures[1] = pre;
    }

    bool CheckSupport()
    {
        if (velocity < 0)
            return false;
        float maxV = m_DeltaSize / (2 * 0.02f) * Mathf.Sqrt(viscosity * 0.02f + 2);
        if (velocity >= maxV)
        {
            Debug.Log(maxV.ToString("f5"));
            Debug.LogError("波速不符合要求");
            return false;
        }
        float viscositySq = viscosity * viscosity;
        float velocitySq = velocity * velocity;
        float deltaSizeSq = m_DeltaSize * m_DeltaSize;
        float dt = Mathf.Sqrt(viscositySq + 32 * velocitySq / (deltaSizeSq));
        float dtden = 8 * velocitySq / (deltaSizeSq);
        float maxT = (viscosity + dt) / dtden;
        float maxT2 = (viscosity - dt) / dtden;
        if (maxT2 > 0 && maxT2 < maxT)
            maxT = maxT2;
        if (maxT < 0.02f)
        {
            Debug.LogError("时间间隔不符合要求");
            return false;
        }

        return true;
    }
}
