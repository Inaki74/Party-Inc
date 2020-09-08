using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializationHelperClass
{
    /// <summary>
    /// Serialized a Two Dimensional Array into a jagged array of two. Because photon takes arrays of arrays, but not MDArrays.
    /// </summary>
    /// <param name="twoDimArray"></param>
    /// <returns></returns>
    public static int[][] SerializeTDArray(int[,] twoDimArray)
    {
        int[][] aux = new int[twoDimArray.GetLength(0)][];



        for(int i = 0; i < twoDimArray.GetLength(0); i++)
        {
            int[] result = new int[twoDimArray.GetLength(1)];

            for(int j = 0; j < twoDimArray.GetLength(1); j++)
            {
                result[j] = twoDimArray[i, j];
            }

            aux[i] = result;
        }

        return aux;
    }

    public static int[,] DeserializeTDArray(int [][] toDeserialize)
    {
        int[,] aux = new int[toDeserialize.Length, toDeserialize[0].Length];

        for(int i = 0; i < toDeserialize.Length; i++)
        {
            for(int j = 0; j < toDeserialize[0].Length; j++)
            {
                aux[i, j] = toDeserialize[i][j];
            }
        }

        return aux;
    }
    
}
