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

        public class Mng_CharacterEditorCache : MonoSingleton<Mng_CharacterEditorCache>
        {
            // An array of strings which each array position maps to a list of assets owned by the player of a certain type:
            // Outfit assets populate numbers 0 - 7
            // Wallpaper assets populate the number 8
            // Emote assets populate numbers 9 - 14
            // Tune assets populate the number 15
            // Face assets populate numbers 16 - 23 (last since they are not buyable)
            private Dictionary<Enum_CharacterAssetTypes, List<Data_CharacterAssetMetadata>> _ownedAssets = new Dictionary<Enum_CharacterAssetTypes, List<Data_CharacterAssetMetadata>>();
            private List<string>[] _ownedAssetsIds = new List<string>[24];
            private Dictionary<Enum_CharacterAssetTypes, string> _chosenAssets = new Dictionary<Enum_CharacterAssetTypes, string>();
            private Dictionary<Enum_CharacterAssetTypes, List<Data_CharacterAssetMetadata>> _allAssetsMetadata = new Dictionary<Enum_CharacterAssetTypes, List<Data_CharacterAssetMetadata>>();

            private const string ASSETS_METADATA_PATH = "/1_Common/_Prefabs/_characterAssetsMetadata";
            private const char ASSET_NAME_SEPARATOR = '_';

            [SerializeField] private Data_InitialAssets _initialAssets;

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < _ownedAssetsIds.Length; i++)
                {
                    _ownedAssetsIds[i] = new List<string>();
                }

                foreach(KeyValuePair<Enum_CharacterAssetTypes, string> kp in _chosenAssets)
                {
                    //kp.Value
                }

                InitializeAllAssetsMetadata();
            }

            private void InitializeAllAssetsMetadata()
            {
                Data_CharacterAssetMetadata[] allAssets = Resources.LoadAll(ASSETS_METADATA_PATH, typeof(Data_CharacterAssetMetadata)).Cast<Data_CharacterAssetMetadata>().ToArray();

                foreach(Data_CharacterAssetMetadata metadata in allAssets)
                {
                    _allAssetsMetadata[metadata.AssetType].Add(metadata);
                }
            }

            public override void OnEnable()
            {
                StartCoroutine(StartCache());
            }

            public List<Data_CharacterAssetMetadata> GetVariationsOfSelectedAsset(Enum_CharacterAssetTypes type)
            {
                List<Data_CharacterAssetMetadata> variationsMetadata = new List<Data_CharacterAssetMetadata>();
                string selectedAssetForType = _chosenAssets[type];
                Data_CharacterAssetMetadata theSelectedAsset = _ownedAssets[type].First(m => m.AssetId == selectedAssetForType);

                if (!theSelectedAsset.IsVariation)
                {
                    List<string> variationsList = theSelectedAsset.VariationAssetsIds;

                    foreach (string variationId in variationsList)
                    {
                        variationsMetadata.Add(_ownedAssets[type].First(m => m.AssetId == variationId));
                    }
                }
                else
                {
                    // Its a variation, look for parent and then get its variations
                    string[] split = theSelectedAsset.AssetId.Split(ASSET_NAME_SEPARATOR);

                    string parentAssetForType = split[0];
                    Data_CharacterAssetMetadata theSelectedAssetsParent = _ownedAssets[type].First(m => m.AssetId == parentAssetForType);

                    List<string> variationsList = theSelectedAssetsParent.VariationAssetsIds;

                    foreach (string variationId in variationsList)
                    {
                        variationsMetadata.Add(_ownedAssets[type].First(m => m.AssetId == variationId));
                    }
                }

                return variationsMetadata;
            }

            public List<Data_CharacterAssetMetadata> GetMetadataListOfAssetType(int type)
            {
                return _ownedAssets[(Enum_CharacterAssetTypes)type];
            }

            public void ChooseAsset(string data, Enum_CharacterAssetTypes assetType)
            {
                _chosenAssets.Remove(assetType);
                _chosenAssets.Add(assetType, data);
            }

            private IEnumerator StartCache()
            {
                // Get the owned assets list (already in memory)
                // Async load the resources
                // Build up lists for each type of asset (24)
                // Save them in an Array of List of strings

                yield return new WaitUntil(() => Fb_FirebaseAuthenticateManager.Current.AuthInitialized);

                List<Dictionary<string, object>> ownedAssets;

                if (Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
                {
                    yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                    ownedAssets = (List<Dictionary<string, object>>)Fb_FirestoreSession.Current.LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS];

                    PutBasicAssets();

                    PutOwnedAssets(ownedAssets);
                }
                else
                {
                    PutBasicAssets();
                }

                LoadOwnedAssetsMetadata();

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

            private void LoadOwnedAssetsMetadata()
            {
                for(int i = 0; i < _ownedAssetsIds.Length; i++)
                {
                    Enum_CharacterAssetTypes currentType = (Enum_CharacterAssetTypes)i;
                    List<Data_CharacterAssetMetadata> ownedAssetMetadataListForType = new List<Data_CharacterAssetMetadata>();

                    foreach(string id in _ownedAssetsIds[i])
                    {
                        if(_allAssetsMetadata[currentType].Any(m => m.AssetId == id))
                        {
                            Data_CharacterAssetMetadata theMetadata = _allAssetsMetadata[currentType].First(m => m.AssetId == id);

                            ownedAssetMetadataListForType.Add(theMetadata);

                            if (theMetadata.VariationAssetsIds.Count() > 0)
                            {
                                // Add the variations of the asset.
                                // PRE: All variations are within the all assets list and are unique.
                                foreach(string variationId in theMetadata.VariationAssetsIds)
                                {
                                    _allAssetsMetadata[currentType].First(m => m.AssetId == variationId);
                                }
                            }
                        }
                        else
                        {
                            // Something went wrong (DB impairment with local metadata files probably).
                        }
                    }

                    _ownedAssets[currentType] = ownedAssetMetadataListForType;
                }
            }

            private void PutOwnedAssets(List<Dictionary<string, object>> assets)
            {
                foreach (Dictionary<string, object> asset in assets)
                {
                    string id = (string)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID];
                    int type = (int)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_TYPE];

                    _ownedAssetsIds[type].Add(id);
                }
            }

            private void PutBasicAssets()
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

            private void PutFieldList(List<string> field, string fieldName)
            {
                var fieldInfo = _initialAssets.GetType().GetField(fieldName);
                var type = ((AssetsType)Attribute.GetCustomAttribute(fieldInfo, typeof(AssetsType))).Type;

                _ownedAssetsIds[(int)type] = field;
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
