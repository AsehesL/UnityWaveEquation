using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class Utils
{
    public static Mesh GenerateLiquidMesh(float width, float length, float cellSize)
    {
        int xsize = Mathf.RoundToInt(width / cellSize);
        int ysize = Mathf.RoundToInt(length / cellSize);

        Mesh mesh = new Mesh();

        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<Vector3> normalList = new List<Vector3>();
        List<int> indexList = new List<int>();
        float xcellsize = width / xsize;
        float uvxcellsize = 1.0f / xsize;
        float ycellsize = length / ysize;
        float uvycellsize = 1.0f / ysize;

        for (int i = 0; i <= ysize; i++)
        {
            for (int j = 0; j <= xsize; j++)
            {
                vertexList.Add(new Vector3(-width * 0.5f + j * xcellsize, 0, -length * 0.5f + i * ycellsize));
                uvList.Add(new Vector2(j * uvxcellsize, i * uvycellsize));
                normalList.Add(Vector3.up);

                if (i < ysize && j < xsize)
                {
                    indexList.Add(i * (xsize + 1) + j);
                    indexList.Add((i + 1) * (xsize + 1) + j);
                    indexList.Add((i + 1) * (xsize + 1) + j + 1);

                    indexList.Add(i * (xsize + 1) + j);
                    indexList.Add((i + 1) * (xsize + 1) + j + 1);
                    indexList.Add(i * (xsize + 1) + j + 1);
                }
            }
        }

        mesh.SetVertices(vertexList);
        mesh.SetUVs(0, uvList);
        mesh.SetNormals(normalList);
        mesh.SetTriangles(indexList, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public static Mesh GenerateLiquidBodyMesh(float width, float length, float depth, float cellSize)
    {
        int xsize = Mathf.RoundToInt(width / cellSize);
        int ysize = Mathf.RoundToInt(length / cellSize);

        Mesh mesh = new Mesh();

        List<Vector3> vertexList = new List<Vector3>();
        List<Vector3> normalList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<Color> colorList = new List<Color>();
        List<int> indexList = new List<int>();
        float xcellsize = width / xsize;
        float uvxcellsize = 1.0f / xsize;
        float ycellsize = length / ysize;
        float uvycellsize = 1.0f / ysize;

        for (int i = 0; i <= ysize; i++)
        {
            vertexList.Add(new Vector3(-width * 0.5f, -depth, -length * 0.5f + i * ycellsize));
            vertexList.Add(new Vector3(-width * 0.5f, 0, -length * 0.5f + i * ycellsize));
            vertexList.Add(new Vector3(width * 0.5f, -depth, -length * 0.5f + i * ycellsize));
            vertexList.Add(new Vector3(width * 0.5f, 0, -length * 0.5f + i * ycellsize));
            normalList.Add(Vector3.left);
            normalList.Add(Vector3.left);
            normalList.Add(Vector3.right);
            normalList.Add(Vector3.right);
            colorList.Add(Color.white);
            colorList.Add(new Color(1, 1, 1, 0));
            colorList.Add(Color.white);
            colorList.Add(new Color(1, 1, 1, 0));
            uvList.Add(new Vector2(0, i * uvycellsize));
            uvList.Add(new Vector2(0, i * uvycellsize));
            uvList.Add(new Vector2(1, i * uvycellsize));
            uvList.Add(new Vector2(1, i * uvycellsize));

            if (i < ysize)
            {
                indexList.Add(i * 4);
                indexList.Add((i + 1) * 4 + 1);
                indexList.Add(i * 4 + 1);
                indexList.Add(i * 4);
                indexList.Add((i + 1) * 4);
                indexList.Add((i + 1) * 4 + 1);

                indexList.Add((i + 1) * 4 + 2);
                indexList.Add(i * 4 + 3);
                indexList.Add((i + 1) * 4 + 3);
                indexList.Add((i + 1) * 4 + 2);
                indexList.Add(i * 4 + 2);
                indexList.Add(i * 4 + 3);
            }
        }

        for (int j = 0; j <= xsize; j++)
        {
            vertexList.Add(new Vector3(-width * 0.5f + j * xcellsize, -depth, -length * 0.5f));
            vertexList.Add(new Vector3(-width * 0.5f + j * xcellsize, 0, -length * 0.5f));
            vertexList.Add(new Vector3(-width * 0.5f + j * xcellsize, -depth, length * 0.5f));
            vertexList.Add(new Vector3(-width * 0.5f + j * xcellsize, 0, length * 0.5f));
            normalList.Add(Vector3.back);
            normalList.Add(Vector3.back);
            normalList.Add(Vector3.forward);
            normalList.Add(Vector3.forward);
            colorList.Add(Color.white);
            colorList.Add(new Color(1, 1, 1, 0));
            colorList.Add(Color.white);
            colorList.Add(new Color(1, 1, 1, 0));
            uvList.Add(new Vector2(j * uvxcellsize, 0));
            uvList.Add(new Vector2(j * uvxcellsize, 0));
            uvList.Add(new Vector2(j * uvxcellsize, 1));
            uvList.Add(new Vector2(j * uvxcellsize, 1));

            if (j < xsize)
            {
                indexList.Add((ysize + 1) * 4 + j * 4);
                indexList.Add((ysize + 1) * 4 + j * 4 + 1);
                indexList.Add((ysize + 1) * 4 + (j + 1) * 4 + 1);
                indexList.Add((ysize + 1) * 4 + j * 4);
                indexList.Add((ysize + 1) * 4 + (j + 1) * 4 + 1);
                indexList.Add((ysize + 1) * 4 + (j + 1) * 4);

                indexList.Add((ysize + 1) * 4 + (j + 1) * 4 + 2);
                indexList.Add((ysize + 1) * 4 + (j + 1) * 4 + 3);
                indexList.Add((ysize + 1) * 4 + j * 4 + 3);
                indexList.Add((ysize + 1) * 4 + (j + 1) * 4 + 2);
                indexList.Add((ysize + 1) * 4 + j * 4 + 3);
                indexList.Add((ysize + 1) * 4 + j * 4 + 2);
            }
        }

        mesh.SetVertices(vertexList);
        mesh.SetNormals(normalList);
        mesh.SetColors(colorList);
        mesh.SetUVs(0, uvList);
        mesh.SetTriangles(indexList, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public static void DrawWirePlane(Vector3 position, float angle, float width, float length, Color color)
    {
        Vector3 p1 = position + Quaternion.Euler(0, angle, 0) * new Vector3(-width * 0.5f, 0, -length * 0.5f);
        Vector3 p2 = position + Quaternion.Euler(0, angle, 0) * new Vector3(-width * 0.5f, 0, length * 0.5f);
        Vector3 p3 = position + Quaternion.Euler(0, angle, 0) * new Vector3(width * 0.5f, 0, length * 0.5f);
        Vector3 p4 = position + Quaternion.Euler(0, angle, 0) * new Vector3(width * 0.5f, 0, -length * 0.5f);

        Gizmos.color = color;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

    public static void DrawWireCube(Vector3 position, float angle, float width, float length, float minHeight,
        float maxHeight, Color color)
    {
        Vector3 p1 = position + Quaternion.Euler(0, angle, 0) * new Vector3(-width * 0.5f, minHeight, -length * 0.5f);
        Vector3 p2 = position + Quaternion.Euler(0, angle, 0) * new Vector3(-width * 0.5f, minHeight, length * 0.5f);
        Vector3 p3 = position + Quaternion.Euler(0, angle, 0) * new Vector3(width * 0.5f, minHeight, length * 0.5f);
        Vector3 p4 = position + Quaternion.Euler(0, angle, 0) * new Vector3(width * 0.5f, minHeight, -length * 0.5f);

        Vector3 p5 = position + Quaternion.Euler(0, angle, 0) * new Vector3(-width * 0.5f, maxHeight, -length * 0.5f);
        Vector3 p6 = position + Quaternion.Euler(0, angle, 0) * new Vector3(-width * 0.5f, maxHeight, length * 0.5f);
        Vector3 p7 = position + Quaternion.Euler(0, angle, 0) * new Vector3(width * 0.5f, maxHeight, length * 0.5f);
        Vector3 p8 = position + Quaternion.Euler(0, angle, 0) * new Vector3(width * 0.5f, maxHeight, -length * 0.5f);

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