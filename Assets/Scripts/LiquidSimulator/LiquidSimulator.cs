using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSimulator : MonoBehaviour
{

    public float size;

    public float minHeight;
    public float maxHeight;

    public int subdivision;

    public LayerMask interactLayer;

    private LiquidRenderer m_Renderer;
    private LiquidSimulatorCamera m_Camera;

    void Start ()
	{
	    m_Renderer = new LiquidRenderer(gameObject, size, subdivision);
        m_Camera = new GameObject("[LS Camera]").AddComponent<LiquidSimulatorCamera>();
        m_Camera.Init(interactLayer, size/2, -maxHeight, -minHeight);
        m_Camera.transform.SetParent(transform);
        m_Camera.transform.localPosition = Vector3.zero;
        m_Camera.transform.localEulerAngles = new Vector3(90, 0, 0);
	}

    void OnDestroy()
    {
        m_Renderer.Release();
    }
	
	void Update () {
		
	}

    void OnDrawGizmosSelected()
    {
        LiquidUtils.DrawWirePlane(transform.position, transform.eulerAngles.y, size, Color.gray);

        LiquidUtils.DrawWireCube(transform.position, transform.eulerAngles.y, size, minHeight, maxHeight, Color.green);
    }
}
