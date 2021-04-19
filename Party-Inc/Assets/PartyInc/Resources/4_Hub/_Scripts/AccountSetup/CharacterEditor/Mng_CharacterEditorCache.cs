using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            private List<string>[] _classifiedOwnedAssets = new List<string>[24];
            private Dictionary<Enum_AssetTypes, string> _chosenAssets = new Dictionary<Enum_AssetTypes, string>();

            [SerializeField] private Data_InitialAssets _initialAssets;

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < _classifiedOwnedAssets.Length; i++)
                {
                    _classifiedOwnedAssets[i] = new List<string>();
                }

                foreach(KeyValuePair<Enum_AssetTypes, string> kp in _chosenAssets)
                {
                    //kp.Value
                }
            }

            public override void OnEnable()
            {
                StartCoroutine(StartCache());
            }

            public List<string> GetTypeList(int type)
            {
                print(type);
                return _classifiedOwnedAssets[type];
            }

            public void ChooseAsset(string data, Enum_AssetTypes assetType)
            {
                _chosenAssets.Add(assetType, data);
            }

            private IEnumerator StartCache()
            {
                // Get the owned assets list (already in memory)
                // Async load the resources
                // Build up lists for each type of asset (24)
                // Save them in an Array of List of strings

                print("Checking auth initialized");

                yield return new WaitUntil(() => Fb_FirebaseAuthenticateManager.Current.AuthInitialized);

                List<Dictionary<string, object>> ownedAssets;

                if (Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
                {
                    print("Classifying assets with user");
                    yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                    ownedAssets  = (List<Dictionary<string, object>>)Fb_FirestoreSession.Current.LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS];

                    ClassifyAssets(ownedAssets);
                }
                else
                {
                    print("Classifying assets");
                    PutInitialAssetsClassified();
                }

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

            private void ClassifyAssets(List<Dictionary<string, object>> assets)
            {

                foreach (Dictionary<string, object> asset in assets)
                {
                    string id = (string)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID];
                    int type = (int)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_TYPE];

                    _classifiedOwnedAssets[type].Add(id);
                }
            }

            private void PutInitialAssetsClassified()
            {
                // Using reflection is much slower.
                // This is faster, but its also less maintainable

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
                print("Asset classified");

                var fieldInfo = _initialAssets.GetType().GetField(fieldName);
                var type = ((AssetsType)Attribute.GetCustomAttribute(fieldInfo, typeof(AssetsType))).Type;

                _classifiedOwnedAssets[(int)type] = field;
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
