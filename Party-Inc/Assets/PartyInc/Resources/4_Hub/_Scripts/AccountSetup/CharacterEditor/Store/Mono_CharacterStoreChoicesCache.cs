using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CharacterStoreChoicesCache : MonoBehaviour
        {
            private List<AssetsStoreData> _chosenAssets = new List<AssetsStoreData>();
            public List<AssetsStoreData> ChosenAssets
            {
                get
                {
                    return _chosenAssets;
                }
                set
                {
                    _chosenAssets = value;
                }
            }

            public delegate void ActionChosenAssetListModified(int length);
            public static event ActionChosenAssetListModified onChosenAssetListModified;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void AddAsset(AssetsStoreData assetData)
            {
                if (_chosenAssets.Any(asset => asset.assetid == assetData.assetid))
                {
                    return;
                }

                _chosenAssets.Add(assetData);

                onChosenAssetListModified?.Invoke(_chosenAssets.Count);
            }

            public void RemoveAsset(AssetsStoreData assetData)
            {
                if (!_chosenAssets.Any(asset => asset.assetid == assetData.assetid))
                {
                    return;
                }

                _chosenAssets.Remove(assetData);

                onChosenAssetListModified?.Invoke(_chosenAssets.Count);
            }
        }
    }
}


