using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actual predefined sequence of falling eggs.
/// 
/// </summary>
[System.Serializable]
public class eggb_EggMaps
{
    public class EggMapRow
    {
        string[] row = new string[3];

        public EggMapRow(string first, string second, string third)
        {
            row[0] = first;
            row[1] = second;
            row[2] = third;
        }
    }
    public List<EggMapRow> eggMap = new List<EggMapRow>();

    public void AddRow(EggMapRow row)
    {
        eggMap.Add(row);
    }

    public void AddRows(EggMapRow[] rows)
    {
        for(int i = 0; i < rows.Length; i++)
        {
            eggMap.Add(rows[i]);
        }
    }
}
