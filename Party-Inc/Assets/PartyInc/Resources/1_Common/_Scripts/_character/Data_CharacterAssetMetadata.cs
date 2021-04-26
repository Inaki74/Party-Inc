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
        [SerializeField] private List<Data_CharacterAssetMetadata> _variationAssets;
        [SerializeField] private bool _isVariation;
        [SerializeField] private Sprite _buttonImage;

        public string AssetId
        {
            get
            {
                return _assetId;
            }
            set
            {
                _assetId = value;
            }
        }

        public Enum_CharacterAssetTypes AssetType
        {
            get
            {
                return _assetType;
            }
            set
            {
                _assetType = value;
            }
        }

        public List<Data_CharacterAssetMetadata> VariationAssets
        {
            get
            {
                return _variationAssets;
            }
            set
            {
                _variationAssets = value;
            }
        }

        public bool IsVariation
        {
            get
            {
                return _isVariation;
            }
            set
            {
                _isVariation = value;
            }
        }

        public Sprite ButtonImage
        {
            get
            {
                return _buttonImage;
            }
            set
            {
                _buttonImage = value;
            }
        }
    }
}


