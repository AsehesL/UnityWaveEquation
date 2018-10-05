using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 反射相机
/// </summary>
public class ReflectCamera : MonoBehaviour
{

    private RenderTexture m_ReflectTex;

    private Matrix4x4 m_ReflectionMatrix;
    private Matrix4x4 m_ClipProjection;

    private Camera m_Camera;

    private static bool m_IsRendering = false;

    void OnDisable()
    {
        Clear();
    }

    void Clear()
    {
        if (m_ReflectTex)
        {
            Destroy(m_ReflectTex);
            m_ReflectTex = null;
        }
    }

    static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }

    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(pos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(SignExt(clipPlane.x), SignExt(clipPlane.y), 1.0f, 1.0f);
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }

    static float SignExt(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }

    Camera GetReflectionCamera(Camera current, Material mat, int textureSize)
    {
        if (!m_ReflectTex)
        {
            if (m_ReflectTex) Destroy(m_ReflectTex);
            m_ReflectTex = new RenderTexture(textureSize, textureSize, 16);
            m_ReflectTex.name = "__MirrorReflection" + GetInstanceID();
            m_ReflectTex.isPowerOfTwo = true;
        }
        
        Camera cam = m_Camera;

        if (!cam)
        {

            cam = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + current.GetInstanceID()).AddComponent<Camera>();
            cam.enabled = false;

            Transform t = cam.transform;
            t.position = transform.position;
            t.rotation = transform.rotation;
            
            //mCameras[current] = cam;
            m_Camera = cam;
        }

        // Automatically update the reflection texture
        //if (mat.HasProperty("_LiquidReflectMap"))
            mat.SetTexture("_LiquidReflectMap", m_ReflectTex);
        return cam;
    }

    void CopyCamera(Camera src, Camera dest)
    {

        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
        dest.depthTextureMode = DepthTextureMode.None;
        dest.renderingPath = RenderingPath.Forward;
    }

    void OnWillRenderObject()
    {

        if (m_IsRendering) return;
        Material mat = GetComponent<Renderer>().sharedMaterial;

        Camera cam = Camera.current;
        cam.depthTextureMode = DepthTextureMode.Depth;
        if (!cam) return;

        LayerMask mask = -1;

        m_IsRendering = true;
        Camera mirror = GetReflectionCamera(cam, mat, 512);

        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        CopyCamera(cam, mirror);

        float d = -Vector3.Dot(normal, pos);
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
        Matrix4x4 reflection = Matrix4x4.zero;

        CalculateReflectionMatrix(ref reflection, reflectionPlane);

        Vector3 oldpos = cam.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldpos);
        m_ReflectionMatrix = reflection;
        mirror.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        Vector4 clipPlane = CameraSpacePlane(mirror, pos, normal, 1.0f);
        Matrix4x4 projection = cam.projectionMatrix;

        CalculateObliqueMatrix(ref projection, clipPlane);

        m_ClipProjection = projection;
        mirror.projectionMatrix = projection;
        mirror.cullingMask = ~(1 << 4) & mask.value;
        mirror.targetTexture = m_ReflectTex;

        GL.invertCulling = true;
        {
            mirror.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            mirror.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
            mirror.Render();
            mirror.transform.position = oldpos;
        }
        //mat.SetTexture("_MainTex", mTex);
        GL.invertCulling = false;
        m_IsRendering = false;
    }
}
