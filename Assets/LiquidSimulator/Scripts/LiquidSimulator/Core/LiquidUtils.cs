using UnityEngine;
using System.Collections;

namespace ASL.LiquidSimulator
{
    public static class LiquidUtils
    {

        public static void DrawWirePlane(Vector3 position, float angle, float width, float length, Color color)
        {
            Vector3 p1 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-width*0.5f, 0, -length*0.5f);
            Vector3 p2 = position + Quaternion.Euler(0, angle, 0)*new Vector3(-width*0.5f, 0, length*0.5f);
            Vector3 p3 = position + Quaternion.Euler(0, angle, 0)*new Vector3(width*0.5f, 0, length*0.5f);
            Vector3 p4 = position + Quaternion.Euler(0, angle, 0)*new Vector3(width*0.5f, 0, -length*0.5f);

            Gizmos.color = color;

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }

        public static void DrawWireCube(Vector3 position, float angle, float width, float length, float minHeight, float maxHeight, Color color)
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

}