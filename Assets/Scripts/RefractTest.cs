using UnityEngine;
using System.Collections;

public class RefractTest : MonoBehaviour
{
    public Transform normal;
    public Transform lightDir;
    public Transform refractDir;

    public float refract;
    
    void Update()
    {
        if (!normal || !lightDir || !refractDir)
            return;

        Vector3 refractD = Refract(-lightDir.forward, normal.forward, refract);
        refractDir.forward = refractD;
    }

    void OnGUI()
    {
        if (!normal || !lightDir || !refractDir)
            return;
        GUI.color = Color.black;

        GUILayout.Label("Normal:" + normal.forward.ToString("f4"));
        GUILayout.Label("LDir:" + lightDir.forward.ToString("f4"));
        GUILayout.Label("RDir:" + refractDir.forward.ToString("f4"));
    }

    Vector3 Refract(Vector3 i, Vector3 n, float eta)
    {
        float cosi = Vector3.Dot(-i, n);
        float cost2 = 1.0f - eta * eta * (1.0f - cosi * cosi);
        Vector3 t = eta * i + ((eta * cosi - Mathf.Sqrt(Mathf.Abs(cost2))) * n);
        float v = cost2 > 0 ? 1.0f : 0.0f;
        return t * v;
    }
}
