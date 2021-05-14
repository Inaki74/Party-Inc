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
            private AssetsStoreData[] _chosenAssets = new AssetsStoreData[16];
            public AssetsStoreData[] ChosenAssets
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
                for(int i = 0; i < _chosenAssets.Length; i++)
                {
                    _chosenAssets[i] = default;
                }
            }

            // Update is called once per frame
            void Update()
            {

            }

            public void AddAsset(AssetsStoreData assetData)
            {
                _chosenAssets[(int)assetData.assettype] = assetData;

                int nonDefaultItems = CountNonDefaultItems(_chosenAssets);

                onChosenAssetListModified?.Invoke(nonDefaultItems);
            }

            public void RemoveAsset(AssetsStoreData assetData)
            {
                _chosenAssets[(int) assetData.assettype] = default;

                int nonDefaultItems = CountNonDefaultItems(_chosenAssets);

                onChosenAssetListModified?.Invoke(nonDefaultItems);
            }

            private int CountNonDefaultItems(AssetsStoreData[] assetData)
            {
                int count = 0;

                foreach(AssetsStoreData asset in assetData)
                {
                    if (!asset.Equals(default(AssetsStoreData)))
                    {
                        count++;
                    }
                }

                return count;
            }
        }
    }
}


