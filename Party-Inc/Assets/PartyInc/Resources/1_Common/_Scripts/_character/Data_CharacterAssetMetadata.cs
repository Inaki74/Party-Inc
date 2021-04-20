using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    [CreateAssetMenu(fileName = "Data", menuName = "AssetMetadata")]
    public class Data_CharacterAssetMetadata : ScriptableObject
    {
        // Make a system to make sure its unique?
        // An asset registry DB?
        [Header("The asset ID. Please be sure that its unique within its type.")]
        [SerializeField] private string _assetId;
        [SerializeField] private Enum_CharacterAssetTypes _assetType;
        [SerializeField] private List<string> _variationAssetsIds;
        [SerializeField] private bool _isVariation;
    }
}


