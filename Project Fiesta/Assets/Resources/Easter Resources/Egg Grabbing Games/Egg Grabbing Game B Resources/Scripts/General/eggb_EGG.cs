using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

/// <summary>
/// The present game session where we store the info of the game.
/// </summary>
[System.Serializable]
public class eggb_EGG
{
    private int startingEggCount = 0;
    [SerializeField] public Array2DInt[] eggMaps = new Array2DInt[10];
    public int HighestScore { get; private set; }
    
} 
