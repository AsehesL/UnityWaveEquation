using UnityEngine;
using System.Collections.Generic;

public static class LiquidUtils
{
    public static Mesh GenerateMesh(float size, int subdivision)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<Vector3> normalList = new List<Vector3>();
        List<int> indexList = new List<int>();
        float vecd = size/subdivision;
        float uvd = 1.0f/subdivision;
        for (int i = 0; i <= subdivision; i++)
        {
            for (int j = 0; j <= subdivision; j++)
            {
                vertexList.Add(new Vector3(-size/2 + j*vecd, 0, -size/2 + i*vecd));
                uvList.Add(new Vector2(j*uvd, i*uvd));
                normalList.Add(Vector3.up);

                if (i < subdivision && j < subdivision)
                {
                    indexList.Add(i*(subdivision + 1) + j);
                    indexList.Add((i + 1)*(subdivision + 1) + j);
                    indexList.Add((i + 1)*(subdivision + 1) + j + 1);

                    indexList.Add(i*(subdivision + 1) + j);
                    indexList.Add((i + 1)*(subdivision + 1) + j + 1);
                    indexList.Add(i*(subdivision + 1) + j + 1);
                }
            }
        }

        mesh.SetVertices(vertexList);
        mesh.SetUVs(0, uvList);
        mesh.SetNormals(normalList);
        mesh.SetTriangles(indexList, 0);
        return mesh;
    }

    public static void DrawWirePlane(Vector3 position, float angle, float size, Color color)
    {
        Vector3 p1 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-size/2, 0, -size/2);
        Vector3 p2 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-size/2, 0, size/2);
        Vector3 p3 = position + Quaternion.Euler(0, angle, 0)*new Vector3(size/2, 0, size/2);
        Vector3 p4 = position + Quaternion.Euler(0, angle, 0)*new Vector3(size/2, 0, -size/2);

        Gizmos.color = color;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

    public static void DrawWireCube(Vector3 position, float angle, float size, float minHeight, float maxHeight, Color color)
    {
        Vector3 p1 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-size/2, minHeight, -size/2);
        Vector3 p2 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-size/2, minHeight, size/2);
        Vector3 p3 = position + Quaternion.Euler(0, angle, 0)*new Vector3(size/2, minHeight, size/2);
        Vector3 p4 = position + Quaternion.Euler(0, angle, 0)*new Vector3(size/2, minHeight, -size/2);

        Vector3 p5 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-size/2, maxHeight, -size/2);
        Vector3 p6 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-size/2, maxHeight, size/2);
        Vector3 p7 = position + Quaternion.Euler(0, angle, 0)*new Vector3(size/2, maxHeight, size/2);
        Vector3 p8 = position + Quaternion.Euler(0, angle, 0)*new Vector3(size/2, maxHeight, -size/2);

        Gizmos.color = color;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p6, p7);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p8, p5);

        Gizmos.DrawLine(p1, p5);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p3, p7);
        Gizmos.DrawLine(p4, p8);
    }
}
