using UnityEngine;
using System.Collections;

public class CausticTest : MonoBehaviour
{

    public MeshRenderer targetRenderer;
    public LiquidRenderer liquid;


    public MeshRenderer testRenderer;
    
    void Start()
    {

    }
    
    void Update()
    {
        if (testRenderer)
        {
            testRenderer.sharedMaterial.SetTexture("_BumpMap", liquid.NormalMap);
        }
        if (!targetRenderer || !liquid)
            return;
        Vector4 plane = new Vector4(liquid.transform.up.x, liquid.transform.up.y, liquid.transform.up.z,
            -Vector3.Dot(liquid.transform.up, liquid.transform.position));
        Vector4 boarder = new Vector4(liquid.transform.position.x, liquid.transform.position.z, liquid.width*0.5f,
            liquid.length*0.5f);
        targetRenderer.sharedMaterial.SetVector("_Boarder", boarder);
        targetRenderer.sharedMaterial.SetVector("_Plane", plane);
        targetRenderer.sharedMaterial.SetTexture("_HeightMap", liquid.HeightMap);
    }
}
