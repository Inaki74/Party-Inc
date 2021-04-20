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
            private List<CharacterAsset>[] _classifiedOwnedAssets = new List<CharacterAsset>[24];
            private Dictionary<Enum_CharacterAssetTypes, string> _chosenAssets = new Dictionary<Enum_CharacterAssetTypes, string>();

            [SerializeField] private Data_InitialAssets _initialAssets;

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < _classifiedOwnedAssets.Length; i++)
                {
                    _classifiedOwnedAssets[i] = new List<CharacterAsset>();
                }

                foreach(KeyValuePair<Enum_CharacterAssetTypes, string> kp in _chosenAssets)
                {
                    //kp.Value
                }
            }

            public override void OnEnable()
            {
                StartCoroutine(StartCache());
            }

            public List<CharacterAsset> GetTypeList(int type)
            {
                return _classifiedOwnedAssets[type];
            }

            // Needs to get a selected assets variations
            public List<string> GetVariations(int type)
            {
                // Checks which asset is selected of the type
                // Looks for the variations of those assets
                return null;
            }

            public void ChooseAsset(string data, Enum_CharacterAssetTypes assetType)
            {
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

                    ownedAssets  = (List<Dictionary<string, object>>)Fb_FirestoreSession.Current.LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS];

                    PutInitialAssetsClassified();

                    ClassifyAssets(ownedAssets);
                }
                else
                {
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
                    List<string> variations = (List<string>)asset[Fb_Constants.FIRESTORE_KEY_ASSETS_VARIATIONS];

                    CharacterAsset toAdd = new CharacterAsset();
                    toAdd.assetType = (Enum_CharacterAssetTypes) type;
                    toAdd.id = id;
                    toAdd.variationIds = variations;

                    _classifiedOwnedAssets[type].Add(toAdd);
                }
            }

            private void PutInitialAssetsClassified()
            {
                // Using reflection is much slower.
                // This is faster, but its also less maintainable
                // MY EYES BURN

                PutFieldList(FormCharacterAssetList(_initialAssets.angryEmotes, Enum_CharacterAssetTypes.EMOTE_ANGRY), "angryEmotes");
                PutFieldList(FormCharacterAssetList(_initialAssets.beautyMarks, Enum_CharacterAssetTypes.BEAUTYMARKS), "beautyMarks");
                PutFieldList(FormCharacterAssetList(_initialAssets.brows, Enum_CharacterAssetTypes.BROWS), "brows");
                PutFieldList(FormCharacterAssetList(_initialAssets.celebratoryEmotes, Enum_CharacterAssetTypes.EMOTE_CELEBRATION), "celebratoryEmotes");
                PutFieldList(FormCharacterAssetList(_initialAssets.ears, Enum_CharacterAssetTypes.EARS), "ears");
                PutFieldList(FormCharacterAssetList(_initialAssets.eyes, Enum_CharacterAssetTypes.EYES), "eyes");
                PutFieldList(FormCharacterAssetList(_initialAssets.facialHairs, Enum_CharacterAssetTypes.FACIALHAIR), "facialHairs");
                PutFieldList(FormCharacterAssetList(_initialAssets.footwears, Enum_CharacterAssetTypes.FOOTWEAR), "footwears");
                PutFieldList(FormCharacterAssetList(_initialAssets.glasses, Enum_CharacterAssetTypes.GLASSES), "glasses");
                PutFieldList(FormCharacterAssetList(_initialAssets.hairs, Enum_CharacterAssetTypes.HAIR), "hairs");
                PutFieldList(FormCharacterAssetList(_initialAssets.happyEmotes, Enum_CharacterAssetTypes.EMOTE_HAPPY), "happyEmotes");
                PutFieldList(FormCharacterAssetList(_initialAssets.laughingEmotes, Enum_CharacterAssetTypes.EMOTE_LAUGHING), "laughingEmotes");
                PutFieldList(FormCharacterAssetList(_initialAssets.lips, Enum_CharacterAssetTypes.LIPS), "lips");
                PutFieldList(FormCharacterAssetList(_initialAssets.makeup, Enum_CharacterAssetTypes.MAKEUP), "makeup");
                PutFieldList(FormCharacterAssetList(_initialAssets.noses, Enum_CharacterAssetTypes.NOSE), "noses");
                PutFieldList(FormCharacterAssetList(_initialAssets.pants, Enum_CharacterAssetTypes.PANTS), "pants");
                PutFieldList(FormCharacterAssetList(_initialAssets.sadEmotes, Enum_CharacterAssetTypes.EMOTE_SAD), "sadEmotes");
                PutFieldList(FormCharacterAssetList(_initialAssets.shirts, Enum_CharacterAssetTypes.SHIRT), "shirts");
                PutFieldList(FormCharacterAssetList(_initialAssets.skinColors, Enum_CharacterAssetTypes.SKINCOLOR), "skinColors");
                PutFieldList(FormCharacterAssetList(_initialAssets.socks, Enum_CharacterAssetTypes.SOCKS), "socks");
                PutFieldList(FormCharacterAssetList(_initialAssets.surprisedEmotes, Enum_CharacterAssetTypes.EMOTE_SURPRISED), "surprisedEmotes");
                PutFieldList(FormCharacterAssetList(_initialAssets.tunes, Enum_CharacterAssetTypes.TUNE), "tunes");
                PutFieldList(FormCharacterAssetList(_initialAssets.wallpapers, Enum_CharacterAssetTypes.WALLPAPER), "wallpapers");
                PutFieldList(FormCharacterAssetList(_initialAssets.wrinkles, Enum_CharacterAssetTypes.WRINKLES), "wrinkles");
            }

            private List<CharacterAsset> FormCharacterAssetList(List<string> assetIds, Enum_CharacterAssetTypes assetType)
            {
                List<CharacterAsset> characterAssets = new List<CharacterAsset>();

                foreach(string id in assetIds)
                {
                    CharacterAsset newAsset = new CharacterAsset();

                    newAsset.id = id;
                    newAsset.assetType = assetType;
                    //newAsset.variationIds = ;
                }

                return characterAssets;
            }

            private void PutFieldList(List<CharacterAsset> field, string fieldName)
            {
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

        public struct CharacterAsset
        {
            public string id;
            public Enum_CharacterAssetTypes assetType;
            public List<string> variationIds;
        }
    }
}
