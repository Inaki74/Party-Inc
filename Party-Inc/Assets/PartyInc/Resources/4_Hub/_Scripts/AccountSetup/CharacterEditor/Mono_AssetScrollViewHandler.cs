using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetScrollViewHandler : MonoBehaviour
        {
            private List<Mono_AssetButtonHandler> _toggles = new List<Mono_AssetButtonHandler>();
            [SerializeField] private GameObject _elementToSpawnPrefab;
            [SerializeField] private GameObject _spawnerContainer;

            public void InitializeScrollview(Data_CharacterAssetMetadata[] data, ToggleGroup buttonsToggle, Action<Data_CharacterAssetMetadata> onToggle)
            {
                foreach(Data_CharacterAssetMetadata unit in data)
                {
                    if (unit == null) continue;

                    GameObject element = Instantiate(_elementToSpawnPrefab, _spawnerContainer.transform); // Prefab pool?
                    element.tag = "Carousel";

                    Mono_AssetButtonHandler buttonHandler = element.GetComponent<Mono_AssetButtonHandler>();
                    buttonHandler.InitializeButton(unit, buttonsToggle, onToggle);// (unit)

                    _toggles.Add(buttonHandler);
                }
            }
        }
    }
}


