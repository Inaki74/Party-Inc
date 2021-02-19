using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FiestaTime
{
    public static class LogExtensions
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
    }
}


