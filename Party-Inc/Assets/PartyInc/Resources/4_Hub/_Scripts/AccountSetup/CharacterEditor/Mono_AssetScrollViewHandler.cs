using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetScrollViewHandler : MonoBehaviour
        {
            [SerializeField] private GameObject _elementToSpawnPrefab;
            [SerializeField] private GameObject _spawnerContainer;

            public void InitializeScrollview(string[] data, ToggleGroup buttonsToggle, Enum_AssetTypes assetType)
            {
                foreach(string unit in data)
                {
                    if (unit == null) continue;

                    GameObject element = Instantiate(_elementToSpawnPrefab, _spawnerContainer.transform); // Prefab pool?
                    element.tag = "Carousel";

                    Mono_AssetButtonHandler buttonHandler = element.GetComponent<Mono_AssetButtonHandler>();
                    if(buttonHandler != null)
                    {
                        element.GetComponent<Mono_AssetButtonHandler>().InitializeButton(unit, buttonsToggle, assetType);// (unit)
                    }
                    else
                    {
                        element.GetComponent<Mono_AssetButtonPlayableHandler>().InitializeButton(unit, buttonsToggle, assetType);// (unit)
                    }
                    
                }
            }
        }
    }
}


