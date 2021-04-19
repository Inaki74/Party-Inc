using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle))]
        public class Mono_AssetButtonPlayableHandler : MonoBehaviour
        {
            private string _assetId;
            private Toggle _theToggle;
            private Enum_AssetTypes _assetType;

            //DATA

            private void Awake()
            {
                _theToggle = GetComponent<Toggle>();
            }

            public void InitializeButton(string assetId, ToggleGroup theTG, Enum_AssetTypes assetTypes)
            {
                _assetId = assetId;

                _theToggle.group = theTG;

                _assetType = assetTypes;
            }
        }
    }
}