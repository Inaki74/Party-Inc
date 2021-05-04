using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        using PartyFirebase;
        using PartyFirebase.Firestore;
        using PartyFirebase.Auth;

        public struct PositionData
        {
            public float height;
            public float separation;
            public float squash;
            public float scale;
            public float rotation;
        }

        public class Mng_CharacterEditorCache : MonoSingleton<Mng_CharacterEditorCache>
        {
            // An array of strings which each array position maps to a list of assets owned by the player of a certain type:
            // Loadout assets populate numbers 0 - 7
            // Wallpaper assets populate the number 8
            // Emote assets populate numbers 9 - 14
            // Tune assets populate the number 15
            // Face assets populate numbers 16 - 23 (last since they are not buyable)
            [SerializeField] private Mono_CharacterStoreCache _storeCache;
            private List<Data_CharacterAssetMetadata>[] _storeDisplayAssetsMetadata = new List<Data_CharacterAssetMetadata>[24];

            private List<Data_CharacterAssetMetadata>[] _ownedAssets = new List<Data_CharacterAssetMetadata>[24];
            private List<string>[] _ownedAssetsIds = new List<string>[24];
            private List<Data_CharacterAssetMetadata>[] _allAssetsMetadata = new List<Data_CharacterAssetMetadata>[24];

            private const string ASSETS_METADATA_PATH = "1_Common/_Prefabs/_characterAssetsMetadata/";
            public const char ASSET_NAME_SEPARATOR = '_';

            [SerializeField] private Data_InitialAssets _initialAssets;

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < _ownedAssetsIds.Length; i++)
                {
                    _ownedAssetsIds[i] = new List<string>();
                }

                for (int i = 0; i < _ownedAssets.Length; i++)
                {
                    _ownedAssets[i] = new List<Data_CharacterAssetMetadata>();
                }

                for (int i = 0; i < _allAssetsMetadata.Length; i++)
                {
                    _allAssetsMetadata[i] = new List<Data_CharacterAssetMetadata>();
                }

                if(_storeCache != null)
                {
                    for (int i = 0; i < _storeDisplayAssetsMetadata.Length; i++)
                    {
                        _storeDisplayAssetsMetadata[i] = new List<Data_CharacterAssetMetadata>();
                    }
                }
            }

            private void InitializeAllAssetsMetadata()
            {
                Data_CharacterAssetMetadata[] allAssets = Resources.LoadAll(ASSETS_METADATA_PATH, typeof(Data_CharacterAssetMetadata)).Cast<Data_CharacterAssetMetadata>().ToArray();

                foreach(Data_CharacterAssetMetadata metadata in allAssets)
                {
                    _allAssetsMetadata[(int)metadata.AssetType].Add(metadata);
                }
            }

            private void OnDestroy()
            {
                Resources.UnloadUnusedAssets();
            }

            public override void OnEnable()
            {
                StartCoroutine(StartCache());
            }

            public List<Data_CharacterAssetMetadata> GetVariationsOfSelectedAsset(Enum_CharacterAssetTypes type)
            {
                List<Data_CharacterAssetMetadata> variationsMetadata = new List<Data_CharacterAssetMetadata>();
                string selectedAssetForType = Mng_CharacterEditorChoicesCache.Current.GetChosenAssetId(type);
                Data_CharacterAssetMetadata theSelectedAsset = _allAssetsMetadata[(int)type].First(m => m.AssetId == selectedAssetForType);

                if (!theSelectedAsset.IsVariation)
                {
                    // The parent is, in itself, a variation
                    variationsMetadata.Add(theSelectedAsset);

                    List<Data_CharacterAssetMetadata> variationsList = theSelectedAsset.VariationAssets;

                    foreach (Data_CharacterAssetMetadata variation in variationsList)
                    {
                        print(variation);
                        variationsMetadata.Add(variation);
                    }
                }
                else
                {
                    // Its a variation, look for parent and then get its variations
                    string[] split = theSelectedAsset.AssetId.Split(ASSET_NAME_SEPARATOR);

                    string parentAssetForType = split[0];
                    Data_CharacterAssetMetadata theSelectedAssetsParent = _allAssetsMetadata[(int)type].First(m => m.AssetId == parentAssetForType);

                    variationsMetadata.Add(theSelectedAssetsParent);

                    List<Data_CharacterAssetMetadata> variationsList = theSelectedAssetsParent.VariationAssets;

                    foreach (Data_CharacterAssetMetadata variation in variationsList)
                    {
                        variationsMetadata.Add(variation);
                    }
                }

                return variationsMetadata;
            }

            public List<Data_CharacterAssetMetadata> GetVariationsOfSelectedDisplayAsset(Enum_CharacterAssetTypes type)
            {
                List<Data_CharacterAssetMetadata> variationsMetadata = new List<Data_CharacterAssetMetadata>();
                string selectedAssetForType = Mng_CharacterEditorChoicesCache.Current.GetChosenStoreAssetId(type);
                Data_CharacterAssetMetadata theSelectedAsset = _storeDisplayAssetsMetadata[(int)type].First(m => m.AssetId == selectedAssetForType);

                if (!theSelectedAsset.IsVariation)
                {
                    // The parent is, in itself, a variation
                    variationsMetadata.Add(theSelectedAsset);

                    List<Data_CharacterAssetMetadata> variationsList = theSelectedAsset.VariationAssets;

                    foreach (Data_CharacterAssetMetadata variation in variationsList)
                    {
                        print(variation);
                        variationsMetadata.Add(variation);
                    }
                }
                else
                {
                    // Its a variation, look for parent and then get its variations
                    string[] split = theSelectedAsset.AssetId.Split(ASSET_NAME_SEPARATOR);

                    string parentAssetForType = split[0];
                    Data_CharacterAssetMetadata theSelectedAssetsParent = _ownedAssets[(int)type].First(m => m.AssetId == parentAssetForType);

                    variationsMetadata.Add(theSelectedAssetsParent);

                    List<Data_CharacterAssetMetadata> variationsList = theSelectedAssetsParent.VariationAssets;

                    foreach (Data_CharacterAssetMetadata variation in variationsList)
                    {
                        variationsMetadata.Add(variation);
                    }
                }

                return variationsMetadata;
            }

            public List<Data_CharacterAssetMetadata> GetParentsMetadataListOfAssetType(int type)
            {
                List<Data_CharacterAssetMetadata> listWithoutVariations = new List<Data_CharacterAssetMetadata>();

                foreach(Data_CharacterAssetMetadata metadata in _ownedAssets[type])
                {
                    if (!metadata.IsVariation)
                    {
                        listWithoutVariations.Add(metadata);
                    }
                }

                return listWithoutVariations;
            }

            public void SetDisplayAssetMetadataList(List<AssetsStoreData> list, int type)
            {
                foreach(AssetsStoreData assetData in list)
                {
                    Data_CharacterAssetMetadata theMetadata = _allAssetsMetadata[type].Find(data => data.AssetId == assetData.assetid);

                    _storeDisplayAssetsMetadata[type].Add(theMetadata);

                    foreach (Data_CharacterAssetMetadata variation in theMetadata.VariationAssets)
                    {
                        _storeDisplayAssetsMetadata[type].Add(variation);
                    }
                }
            }

            public List<Data_CharacterAssetMetadata> GetParentsDisplayAssetsMetadata(int type)
            {

                List<Data_CharacterAssetMetadata> listWithoutVariations = new List<Data_CharacterAssetMetadata>();

                foreach (Data_CharacterAssetMetadata metadata in _storeDisplayAssetsMetadata[type])
                {
                    if (!metadata.IsVariation)
                    {
                        listWithoutVariations.Add(metadata);
                    }
                }

                return listWithoutVariations;
            }

            public AssetsStoreData GetAssetStoreData(string assetId, int assetType)
            {
                return _storeCache.GetAssetData(assetId, assetType);
            }

            public bool GetAssetStoreReady()
            {
                return _storeCache.GotData;
            }

            private IEnumerator StartCache()
            {
                // Get the owned assets list (already in memory)
                // Async load the resources
                // Build up lists for each type of asset (24)

                yield return new WaitUntil(() => Fb_FirebaseAuthenticateManager.Current.AuthInitialized);

                if (Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
                {
                    InitializeAllAssetsMetadata();

                    yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                    LoadBasicAssetsMetadata();

                    SetupOwnedAssets();

                    print("About to get asset data");
                    _storeCache.GetAllAssetsStoreData();
                    print("About to check get data");
                    yield return new WaitUntil(() => _storeCache.GotData);
                    print("Got data");
                    BuildDisplayList();
                }
                else
                {
                    LoadBasicAssetsMetadata();
                }

                SetChosenAssetsToDefault();

                // Loading assets all at once
                // Or load assets in the moment?
                // If this stays, load from classified list
                //foreach (Dictionary<string, object> asset in ownedAssets)
                //{
                //    string assetId = (string)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID];

                //    LoadAssetImages(assetId);
                //    LoadAssetModels(assetId);
                //}
            }

            private void BuildDisplayList()
            {
                print("Build list");
                List<AssetsStoreData>[] displayStoreList = _storeCache.DisplayAssetsStoreData;

                int i = 0;
                foreach(List<AssetsStoreData> assetList in displayStoreList)
                {
                    foreach(AssetsStoreData asset in assetList)
                    {
                        _storeDisplayAssetsMetadata[i].Add(_allAssetsMetadata[i].Find(assetMeta => assetMeta.AssetId == asset.assetid));
                    }

                    i++;
                }
                //_storeDisplayAssetsMetadata;
            }

            private void SetupOwnedAssets()
            {
                List<Dictionary<string, object>> ownedAssets = new List<Dictionary<string, object>>();
                object playerAssetsObject = Fb_FirestoreSession.Current.LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS];
                List<object> playerAssetsListObject = (List<object>)Convert.ChangeType(playerAssetsObject, typeof(List<object>));

                if (playerAssetsListObject.Count > 0)
                {
                    foreach (object listObject in playerAssetsListObject)
                    {
                        ownedAssets.Add((Dictionary<string, object>)Convert.ChangeType(listObject, typeof(Dictionary<string, object>)));
                    }

                    PutOwnedAssets(ownedAssets);
                    LoadOwnedAssetsMetadata();
                }
            }

            private void SetChosenAssetsToDefault()
            {
                if (FindObjectOfType<Mono_StoreClosetResolver>().EnteredStore)
                {
                    SetChosenAssetsStore();
                }
                else
                {
                    SetChosenAssetsCloset();
                }
            }

            public void SetChosenAssetsCloset()
            {
                for (int i = 0; i < _ownedAssets.Length; i++)
                {
                    if (_ownedAssets[i].Count > 0)
                    {
                        Mng_CharacterEditorChoicesCache.Current.ChooseAsset(_ownedAssets[i].First().AssetId, (Enum_CharacterAssetTypes)i);
                    }
                }
            }

            public void SetChosenAssetsStore()
            {
                for (int i = 0; i < _storeDisplayAssetsMetadata.Length; i++)
                {
                    if (_storeDisplayAssetsMetadata[i].Count > 0)
                    {
                        Mng_CharacterEditorChoicesCache.Current.ChooseAsset(_storeDisplayAssetsMetadata[i].First().AssetId, (Enum_CharacterAssetTypes)i);
                    }
                }
            }

            private void LoadOwnedAssetsMetadata()
            {
                for(int i = 0; i < _ownedAssetsIds.Length; i++)
                {
                    Enum_CharacterAssetTypes currentType = (Enum_CharacterAssetTypes)i;
                    List<Data_CharacterAssetMetadata> ownedAssetMetadataListForType = new List<Data_CharacterAssetMetadata>();

                    foreach(string id in _ownedAssetsIds[i])
                    {
                        if(_allAssetsMetadata[(int)currentType].Any(m => m.AssetId == id))
                        {
                            Data_CharacterAssetMetadata theMetadata = _allAssetsMetadata[(int)currentType].First(m => m.AssetId == id);

                            ownedAssetMetadataListForType.Add(theMetadata);

                            if (theMetadata.VariationAssets.Count() > 0)
                            {
                                // Add the variations of the asset.
                                // PRE: All variations are within the all assets list and are unique.
                                foreach(Data_CharacterAssetMetadata variation in theMetadata.VariationAssets)
                                {
                                    print("Asset of type: " + currentType + " and id: " + variation.AssetId);

                                    ownedAssetMetadataListForType.Add(variation);
                                }
                            }
                        }
                        else
                        {
                            // Something went wrong (DB impairment with local metadata files probably).
                        }
                    }

                    _ownedAssets[(int)currentType] = ownedAssetMetadataListForType;
                }
            }

            private void PutOwnedAssets(List<Dictionary<string, object>> assets)
            {
                foreach (Dictionary<string, object> asset in assets)
                {
                    string id = (string)Convert.ChangeType(asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID], typeof(string));
                    int type = (int)Convert.ChangeType(asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_TYPE], typeof(int));

                    _ownedAssetsIds[type].Add(id);
                }
            }

            private void LoadBasicAssetsMetadata()
            {
                // Using reflection is much slower.
                // This is faster, but its also less maintainable
                // MY EYES BURN

                PutFieldList(_initialAssets.angryEmotes, "angryEmotes");
                PutFieldList(_initialAssets.beautyMarks, "beautyMarks");
                PutFieldList(_initialAssets.brows, "brows");
                PutFieldList(_initialAssets.celebratoryEmotes, "celebratoryEmotes");
                PutFieldList(_initialAssets.ears, "ears");
                PutFieldList(_initialAssets.eyes, "eyes");
                PutFieldList(_initialAssets.facialHairs, "facialHairs");
                PutFieldList(_initialAssets.footwears, "footwears");
                PutFieldList(_initialAssets.glasses, "glasses");
                PutFieldList(_initialAssets.hairs, "hairs");
                PutFieldList(_initialAssets.happyEmotes, "happyEmotes");
                PutFieldList(_initialAssets.laughingEmotes, "laughingEmotes");
                PutFieldList(_initialAssets.lips, "lips");
                PutFieldList(_initialAssets.makeup, "makeup");
                PutFieldList(_initialAssets.noses, "noses");
                PutFieldList(_initialAssets.pants, "pants");
                PutFieldList(_initialAssets.sadEmotes, "sadEmotes");
                PutFieldList(_initialAssets.shirts, "shirts");
                PutFieldList(_initialAssets.skinColors, "skinColors");
                PutFieldList(_initialAssets.socks, "socks");
                PutFieldList(_initialAssets.surprisedEmotes, "surprisedEmotes");
                PutFieldList(_initialAssets.tunes, "tunes");
                PutFieldList(_initialAssets.wallpapers, "wallpapers");
                PutFieldList(_initialAssets.wrinkles, "wrinkles");
            }

            private void PutFieldList(List<Data_CharacterAssetMetadata> field, string fieldName)
            {
                var fieldInfo = _initialAssets.GetType().GetField(fieldName);
                var type = ((AssetsType)Attribute.GetCustomAttribute(fieldInfo, typeof(AssetsType))).Type;

                _ownedAssets[(int)type] = new List<Data_CharacterAssetMetadata>();

                foreach(Data_CharacterAssetMetadata metadata in field)
                {
                    _ownedAssets[(int)type].Add(metadata);
                    foreach(Data_CharacterAssetMetadata variation in metadata.VariationAssets)
                    {
                        _ownedAssets[(int)type].Add(variation);
                    }
                }
            }

            //TODO
            private void LoadAssetModels(string assetId)
            {
                print("Here goes loading of models, BEEP BOOP BOOP");
            }

            //TODO
            private void LoadAssetImages(string assetId)
            {
                print("Here goes loading of images, BEEP BOOP BOOP");
            } 
        }
    }
}
