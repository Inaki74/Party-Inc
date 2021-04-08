using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetScrollViewHandler : MonoBehaviour
        {
            [SerializeField] private GameObject _elementToSpawnPrefab;
            [SerializeField] private GameObject _spawnerContainer;

            public void InitializeScrollview(string[] data)
            {
                foreach(string unit in data)
                {
                    GameObject element = Instantiate(_elementToSpawnPrefab, _spawnerContainer.transform); // Prefab pool?
                    element.tag = "Carousel";

                    element.GetComponent<Mono_AssetButtonHandler>().InitializeButton();// (unit)
                }
            }
        }
    }
}


