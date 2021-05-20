using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        using PartyFirebase;
        using PartyFirebase.Firestore;
        using PartyFirebase.Auth;

        public class Mng_CharacterEditorChoicesCache : MonoSingleton<Mng_CharacterEditorChoicesCache>
        {
            [SerializeField] private Mono_CharacterStoreChoicesCache _storeCache;
            private string[] _chosenAssets = new string[24];
            private PositionData[] _chosenPositions = new PositionData[24];

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < _chosenPositions.Length; i++)
                {
                    _chosenPositions[i] = new PositionData();
                }
            }

            /// <summary>
            /// STORE
            /// </summary>
            /// <returns></returns>

            public List<AssetsStoreData> GetCart()
            {
                return _storeCache.GetCartWithoutDefaults();
            }

            public string GetChosenStoreAssetId(Enum_CharacterAssetTypes assetType)
            {
                return _storeCache.ChosenAssets[(int)assetType];
            }

            public void SetChosenStoreAssetId(string id, Enum_CharacterAssetTypes assetType)
            {
                _storeCache.ChosenAssets[(int)assetType] = id;
            }

            public void AddStoreAssetToCart(AssetsStoreData assetData)
            {
                _storeCache.SetAsset(assetData);
            }

            public void RemoveStoreAssetFromCart(AssetsStoreData assetData)
            {
                _storeCache.RemoveAsset(assetData);
            }

            /////////////

            public string GetChosenAssetId(Enum_CharacterAssetTypes assetType)
            {
                return _chosenAssets[(int)assetType];
            }

            public PositionData GetChosenAssetPosition(Enum_CharacterAssetTypes assetType)
            {
                return _chosenPositions[(int)assetType];
            }

            public void ChooseAsset(string data, Enum_CharacterAssetTypes assetType)
            {
                _chosenAssets[(int)assetType] = data;
            }

            public void ChangePositionData(PositionData data, Enum_CharacterAssetTypes assetType)
            {
                _chosenPositions[(int)assetType] = data;
            }

            public Fb_FirestoreStructures.FSPlayer.FSCharacter ExportSelectedSettingsAsFirestoreStructure(int currentLoadout)
            {
                Fb_FirestoreStructures.FSPlayer.FSCharacter finalPlayerCharacter = new Fb_FirestoreStructures.FSPlayer.FSCharacter();

                finalPlayerCharacter.currentloadout = currentLoadout;
                finalPlayerCharacter.AddLoadout(
                    currentLoadout,
                    "", // BACK, TODO: Add it to UI
                    GetChosenAssetId(Enum_CharacterAssetTypes.EARS),
                    GetChosenAssetId(Enum_CharacterAssetTypes.FACIALHAIR),
                    GetChosenAssetId(Enum_CharacterAssetTypes.GLASSES),
                    GetChosenAssetId(Enum_CharacterAssetTypes.WALLPAPER),
                    GetChosenAssetId(Enum_CharacterAssetTypes.HAIR),
                    GetChosenAssetId(Enum_CharacterAssetTypes.PANTS),
                    GetChosenAssetId(Enum_CharacterAssetTypes.SHIRT),
                    GetChosenAssetId(Enum_CharacterAssetTypes.SOCKS),
                    GetChosenAssetId(Enum_CharacterAssetTypes.FOOTWEAR),
                    GetChosenAssetId(Enum_CharacterAssetTypes.EMOTE_HAPPY),
                    GetChosenAssetId(Enum_CharacterAssetTypes.EMOTE_SAD),
                    GetChosenAssetId(Enum_CharacterAssetTypes.EMOTE_ANGRY),
                    GetChosenAssetId(Enum_CharacterAssetTypes.EMOTE_LAUGHING),
                    GetChosenAssetId(Enum_CharacterAssetTypes.EMOTE_SURPRISED),
                    GetChosenAssetId(Enum_CharacterAssetTypes.EMOTE_CELEBRATION),
                    GetChosenAssetId(Enum_CharacterAssetTypes.TUNE)
                );

                Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace finalPlayersFace = CreatePlayerFaceWithSettings();
                //finalPlayersFace.

                finalPlayerCharacter.face = finalPlayersFace.ToDictionary();

                return finalPlayerCharacter;
            }

            private Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace CreatePlayerFaceWithSettings()
            {
                Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace finalPlayersFace = new Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace();

                Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSEyesockets finalPlayersEyes = CreatePlayerEyesWithSettings();

                Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSMouth finalPlayersMouth = CreatePlayerMouthWithSettings();

                finalPlayersFace.browid = GetChosenAssetId(Enum_CharacterAssetTypes.BROWS);
                finalPlayersFace.browcolor = ""; // TODO: Decide what to do.
                finalPlayersFace.makeupid = GetChosenAssetId(Enum_CharacterAssetTypes.MAKEUP);
                finalPlayersFace.makeupcolor = ""; // TODO: Decide what to do.
                finalPlayersFace.noseId = GetChosenAssetId(Enum_CharacterAssetTypes.NOSE);
                finalPlayersFace.skinColor = GetChosenAssetId(Enum_CharacterAssetTypes.SKINCOLOR);
                finalPlayersFace.wrinklesId = GetChosenAssetId(Enum_CharacterAssetTypes.WRINKLES);
                finalPlayersFace.eyesockets = finalPlayersEyes.ToDictionary();
                finalPlayersFace.mouth = finalPlayersMouth.ToDictionary();

                return finalPlayersFace;
            }

            private Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSEyesockets CreatePlayerEyesWithSettings()
            {
                Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSEyesockets finalPlayersEyes = new Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSEyesockets();

                PositionData eyesPosition = GetChosenAssetPosition(Enum_CharacterAssetTypes.EYES);
                finalPlayersEyes.eyeid = GetChosenAssetId(Enum_CharacterAssetTypes.EYES);
                finalPlayersEyes.eyecolor = ""; // TODO: Decide what to do.
                finalPlayersEyes.height = eyesPosition.height;
                finalPlayersEyes.rotation = eyesPosition.rotation;
                finalPlayersEyes.scale = eyesPosition.scale;
                finalPlayersEyes.separation = eyesPosition.separation;
                finalPlayersEyes.squash = eyesPosition.squash;

                return finalPlayersEyes;
            }

            private Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSMouth CreatePlayerMouthWithSettings()
            {
                Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSMouth finalPlayersMouth = new Fb_FirestoreStructures.FSPlayer.FSCharacter.FSFace.FSMouth();

                PositionData lipsPosition = GetChosenAssetPosition(Enum_CharacterAssetTypes.LIPS);
                finalPlayersMouth.mouthid = GetChosenAssetId(Enum_CharacterAssetTypes.LIPS);
                finalPlayersMouth.mouthcolor = ""; // TODO: Decide what to do.
                finalPlayersMouth.height = lipsPosition.height;
                finalPlayersMouth.scale = lipsPosition.scale;
                finalPlayersMouth.squash = lipsPosition.squash;

                return finalPlayersMouth;
            }
        }
    }
}


