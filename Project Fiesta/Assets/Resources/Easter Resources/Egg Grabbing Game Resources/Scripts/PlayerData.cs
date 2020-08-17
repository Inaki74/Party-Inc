using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Data")]
    public float movementSpeed = 7f;
    public float maxRangeOfMovement = 10f;
}
