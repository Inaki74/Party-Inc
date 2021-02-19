using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlayInc
{
    public static class DebugExtensions
    {
        public static void PrintArray<T>(T[] array) where T : struct
        {
            for(int i = 0; i < array.Length; i++)
            {
                Debug.Log(array[i].ToString());
            }
        }

        public static void PrintList<T>(List<T> list) where T : struct
        {
            PrintArray(list.ToArray());
        }

        public static List<T> VectorToList<T>(T[] theList)
        {
            List<T> ret = new List<T>();

            for (int i = 0; i < theList.Length; i++)
            {
                ret.Add(theList[i]);
            }

            return ret;
        }

        public static void DrawPlane(Vector3 position, Vector3 normal)
        {
            Vector3 v3;

            if (normal.normalized != Vector3.forward)
            {
                v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
            }
            else
            {
                v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
            }

            var corner0 = position + v3;
            var corner2 = position - v3;
            var q = Quaternion.AngleAxis((float)90.0, normal);
            v3 = q * v3;
            var corner1 = position + v3;
            var corner3 = position - v3;

            Debug.DrawLine(corner0, corner2, Color.green, 5f);
            Debug.DrawLine(corner1, corner3, Color.green, 5f);
            Debug.DrawLine(corner0, corner1, Color.green, 5f);
            Debug.DrawLine(corner1, corner2, Color.green, 5f);
            Debug.DrawLine(corner2, corner3, Color.green, 5f);
            Debug.DrawLine(corner3, corner0, Color.green, 5f);
            Debug.DrawRay(position, normal, Color.red, 5f);
        }
    }
}


