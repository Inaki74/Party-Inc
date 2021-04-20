using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle))]
        public class Mono_AssetButtonHandler : MonoBehaviour
        {
            private string _assetId;
            private Toggle _theToggle;
            private Enum_CharacterAssetTypes _assetType;

            //DATA

            private void Awake()
            {
                _theToggle = GetComponent<Toggle>();
            }

            public void InitializeButton(CharacterAsset assetId, ToggleGroup theTG, Enum_CharacterAssetTypes assetType)
            {
                _theToggle.group = theTG;

                _assetType = assetType;
            }
        }
    }
}


