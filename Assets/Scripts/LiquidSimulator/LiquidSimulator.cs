using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSimulator : MonoBehaviour
{

    public float size;

    public float minHeight;
    public float maxHeight;

    public int subdivision;

    public float viscosity;
    public float velocity;
    public float force;

    public LayerMask interactLayer;

    private LiquidRenderer m_Renderer;
    private LiquidSimulatorCamera m_Camera;

    private float m_DeltaSize;

    private bool m_IsSupported;

    void Start ()
    {
        m_DeltaSize = 1.0f/subdivision;

        m_IsSupported = CheckSupport();
        if (!m_IsSupported)
        {
            Debug.LogError("XX");
            return;
        }

        float fac = velocity*velocity*0.02f*0.02f/(m_DeltaSize*m_DeltaSize);
        float i = viscosity*0.02f - 2;
        float j = viscosity*0.02f + 2;

        float k1 = (4 - 8*fac)/(j);
        float k2 = i / j;
        float k3 = 2 * fac / j;

	    m_Renderer = new LiquidRenderer(gameObject, size, subdivision);
        m_Camera = new GameObject("[LS Camera]").AddComponent<LiquidSimulatorCamera>();
        m_Camera.Init(interactLayer, size/2, -maxHeight, -minHeight, force, new Vector4(k1,k2,k3,0.05f), 1024, m_Renderer.material);
        m_Camera.transform.SetParent(transform);
        m_Camera.transform.localPosition = Vector3.zero;
        m_Camera.transform.localEulerAngles = new Vector3(90, 0, 0);
	}

    void OnDestroy()
    {
        if(m_Renderer!=null)
        m_Renderer.Release();
        if(m_Camera!=null)
        m_Camera.Release();
        m_Renderer = null;
        m_Camera = null;
    }

    bool CheckSupport()
    {
        if (velocity < 0)
            return false;
        float maxV = m_DeltaSize/(2*0.02f)*Mathf.Sqrt(viscosity*0.02f + 2);
        if (velocity >= maxV)
        {
            Debug.Log(maxV.ToString("f5"));
            Debug.LogError("波速不符合要求");
            return false;
        }
        float viscositySq = viscosity*viscosity;
        float velocitySq = velocity*velocity;
        float deltaSizeSq = m_DeltaSize*m_DeltaSize;
        float dt = Mathf.Sqrt(viscositySq + 32* velocitySq / (deltaSizeSq));
        float dtden = 8* velocitySq / (deltaSizeSq);
        float maxT = (viscosity + dt) / dtden;
        float maxT2 = (viscosity - dt)/dtden;
        if (maxT2 > 0 && maxT2 < maxT)
            maxT = maxT2;
        if (maxT < 0.02f)
        {
            Debug.LogError("时间间隔不符合要求");
            return false;
        }

        return true;
    }
	
	void Update () {
		
	}

    void OnDrawGizmosSelected()
    {
        LiquidUtils.DrawWirePlane(transform.position, transform.eulerAngles.y, size, Color.gray);

        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, size, minHeight, maxHeight, Color.green);
    }
}
