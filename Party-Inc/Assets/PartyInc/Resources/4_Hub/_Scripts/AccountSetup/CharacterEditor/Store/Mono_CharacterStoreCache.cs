using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        using System;
        using PartyFirebase.Firestore;

        public struct AssetsStoreData
        {
            public Enum_CharacterAssetTypes assettype; 
            public int baseprice;
            public int premiumprice;
            public string storename;
            public string assetid;
            public Enum_AssetRarities rarity;


            public override bool Equals(object obj)
            {
                AssetsStoreData outsider = (AssetsStoreData)obj;
                return this.assetid == outsider.assetid;
            }
        }

        public enum Enum_AssetRarities
        {
            COMMON = 0,
            UNCOMMON = 1,
            RARE = 2,
            UNIQUE = 3
        }

        public class Mono_CharacterStoreCache : MonoBehaviour
        {
            public bool GotData { get; private set; } = false;

            private List<AssetsStoreData>[] _allAssetsStoreData = new List<AssetsStoreData>[24];

            private bool _ownedItemsIdentified = false;
            private List<AssetsStoreData>[] _displayAssetsStoreData = new List<AssetsStoreData>[24];
            public List<AssetsStoreData>[] DisplayAssetsStoreData
            {
                get
                {
                    return _displayAssetsStoreData;
                }
                set
                {
                    _displayAssetsStoreData = value;
                }
            }

            private void Update()
            {
                if (GotData && !_ownedItemsIdentified)
                {
                    IdentifyUnownedItems();
                }
            }

            public void GetAllAssetsStoreData()
            {
                InitializeStoreCache();

                Fb_FirestoreManager.Current.QueryAllDocumentsInCollection(Fb_FirestoreManager.Current.Assets, res =>
                {
                    if (res.success)
                    {
                        Dictionary<string, object> allAssets = res.data;

                        foreach(KeyValuePair<string, object> assetPair in allAssets)
                        {
                            Dictionary<string, object> theAsset = (Dictionary<string, object>)assetPair.Value;

                            if (theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_ACHIEVEMENT] == null)
                            {
                                AssetsStoreData newData = new AssetsStoreData();
                                newData.baseprice = (int)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_BASEPRICE], typeof(int));
                                newData.assetid = (string)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_ASSETID], typeof(string));
                                newData.premiumprice = (int)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_PREMIUMPRICE], typeof(int));
                                newData.storename = (string)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_NAME], typeof(string));

                                int assettype = (int)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_TYPE], typeof(int));
                                int rarity = (int)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_RARITY], typeof(int));

                                newData.assettype = (Enum_CharacterAssetTypes)assettype;
                                newData.rarity = (Enum_AssetRarities)rarity;

                                int type = (int)Convert.ChangeType(theAsset[Fb_Constants.FIRESTORE_KEY_ASSETS_TYPE], typeof(int));

                                print("Asset of ID: " + assetPair.Key +
                                      ", with Name: " + newData.storename +
                                      ", with price: " + newData.baseprice +
                                      ", with gem price: " + newData.premiumprice +
                                      ", with assetid: " + newData.assetid);

                                _allAssetsStoreData[type].Add(newData);
                            }
                        }

                        GotData = true;
                    }
                    else{
                        Debug.Log(res.exceptions[0].Message);
                    }
                });
            }

            public AssetsStoreData GetAssetData(string assetid, int assettype)
            {
                return _displayAssetsStoreData[assettype].First(ass => ass.assetid == assetid);
            }

            private void IdentifyUnownedItems()
            {
                Debug.Log("IdOwnedItems");

                _ownedItemsIdentified = true;

                // Might need a big copy
                _displayAssetsStoreData = _allAssetsStoreData;

                List<Dictionary<string, object>> ownedAssets = new List<Dictionary<string, object>>();
                object playerAssetsObject = Fb_FirestoreSession.Current.LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS];
                List<object> playerAssetsListObject = (List<object>)Convert.ChangeType(playerAssetsObject, typeof(List<object>));

               
                if (playerAssetsListObject.Count > 0)
                {
                    foreach (object listObject in playerAssetsListObject)
                    {
                        ownedAssets.Add((Dictionary<string, object>)Convert.ChangeType(listObject, typeof(Dictionary<string, object>)));
                    }

                    IdentifyItemsNotIntersected(ownedAssets);
                }
                print(_displayAssetsStoreData[0].Count);
            }

            private void IdentifyItemsNotIntersected(List<Dictionary<string, object>> ownedAssets)
            {
                foreach (Dictionary<string, object> asset in ownedAssets)
                {
                    string id = (string)Convert.ChangeType(asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID], typeof(string));
                    int type = (int)Convert.ChangeType(asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_TYPE], typeof(int));

                    if (_allAssetsStoreData[type].Any(data => data.assetid == id))
                    {
                        _displayAssetsStoreData[type].Remove(_allAssetsStoreData[type].Find(data => data.assetid == id));
                    }
                }
            }

            private void InitializeStoreCache()
            {
                for (int i = 0; i < _allAssetsStoreData.Length; i++)
                {
                    _allAssetsStoreData[i] = new List<AssetsStoreData>();
                }

                for(int i = 0; i < _displayAssetsStoreData.Length; i++)
                {
                    _displayAssetsStoreData[i] = new List<AssetsStoreData>();
                }
            }
        }
    }
}


