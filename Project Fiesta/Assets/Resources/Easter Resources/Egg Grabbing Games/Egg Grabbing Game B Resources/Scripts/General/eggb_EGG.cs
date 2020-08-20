using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The present game session where we store the spawning routines and info of the game.
/// EGG, Egg Grabbing Game
/// </summary>
[System.Serializable]
public class eggb_EGG
{
    public static eggb_EGG current;
    public eggb_EggMaps[] eggMaps = new eggb_EggMaps[10];
    public int HighestScore { get; private set; }

    /// <summary>
    /// TODO: Loads a randomized set of Egg Maps a places them in the eggMaps array.
    /// </summary>
    public void InitializeEggMaps()
    {

    }
}
