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
            private AssetsStoreData[] _cart = new AssetsStoreData[16];
            public AssetsStoreData[] Cart
            {
                get
                {
                    return _cart;
                }
                set
                {
                    _cart = value;
                }
            }

            private string[] _chosenAssets = new string[16];
            public string[] ChosenAssets
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
                for(int i = 0; i < _cart.Length; i++)
                {
                    _cart[i] = default;
                }

                for (int i = 0; i < _chosenAssets.Length; i++)
                {
                    _chosenAssets[i] = null;
                }
            }

            // Update is called once per frame
            void Update()
            {

            }

            public List<AssetsStoreData> GetCartWithoutDefaults()
            {
                List<AssetsStoreData> result = new List<AssetsStoreData>();

                foreach (AssetsStoreData asset in _cart)
                {
                    if (!asset.Equals(default(AssetsStoreData)))
                    {
                        result.Add(asset);
                    }
                }

                return result;
            }

            public void SetAsset(AssetsStoreData assetData)
            {
                print(assetData);

                _cart[(int)assetData.assettype] = assetData;
                _chosenAssets[(int)assetData.assettype] = assetData.assetid;

                int nonDefaultItems = CountNonDefaultItems(_cart);

                onChosenAssetListModified?.Invoke(nonDefaultItems);
            }

            public void RemoveAsset(AssetsStoreData assetData)
            {
                _cart[(int) assetData.assettype] = default;
                _chosenAssets[(int)assetData.assettype] = Mng_CharacterEditorChoicesCache.Current.GetChosenAssetId(assetData.assettype);

                int nonDefaultItems = CountNonDefaultItems(_cart);

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


