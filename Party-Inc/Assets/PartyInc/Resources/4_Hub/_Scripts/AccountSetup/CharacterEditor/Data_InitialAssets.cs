using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        [CreateAssetMenu(fileName = "Data", menuName = "InitialPlayerAssetsConfiguration")]
        public class Data_InitialAssets : ScriptableObject
        {
            [Header("Face Assets")]
            // Since the face assets will always be the same assets
            // Might get them from their folders in resources
            // While we dont have that though, this will do.
            [AssetsType(Enum_CharacterAssetTypes.SKINCOLOR)]
            public List<Data_CharacterAssetMetadata> skinColors;
            [AssetsType(Enum_CharacterAssetTypes.EYES)]
            public List<Data_CharacterAssetMetadata> eyes;
            [AssetsType(Enum_CharacterAssetTypes.BROWS)]
            public List<Data_CharacterAssetMetadata> brows;
            [AssetsType(Enum_CharacterAssetTypes.NOSE)]
            public List<Data_CharacterAssetMetadata> noses;
            [AssetsType(Enum_CharacterAssetTypes.LIPS)]
            public List<Data_CharacterAssetMetadata> lips;
            [AssetsType(Enum_CharacterAssetTypes.MAKEUP)]
            public List<Data_CharacterAssetMetadata> makeup;
            [AssetsType(Enum_CharacterAssetTypes.WRINKLES)]
            public List<Data_CharacterAssetMetadata> wrinkles;
            [AssetsType(Enum_CharacterAssetTypes.BEAUTYMARKS)]
            public List<Data_CharacterAssetMetadata> beautyMarks;

            [Header("Outfit Assets")]
            [AssetsType(Enum_CharacterAssetTypes.HAIR)]
            public List<Data_CharacterAssetMetadata> hairs;
            [AssetsType(Enum_CharacterAssetTypes.FACIALHAIR)]
            public List<Data_CharacterAssetMetadata> facialHairs;
            [AssetsType(Enum_CharacterAssetTypes.EARS)]
            public List<Data_CharacterAssetMetadata> ears;
            [AssetsType(Enum_CharacterAssetTypes.SHIRT)]
            public List<Data_CharacterAssetMetadata> shirts;
            [AssetsType(Enum_CharacterAssetTypes.PANTS)]
            public List<Data_CharacterAssetMetadata> pants;
            [AssetsType(Enum_CharacterAssetTypes.SOCKS)]
            public List<Data_CharacterAssetMetadata> socks;
            [AssetsType(Enum_CharacterAssetTypes.FOOTWEAR)]
            public List<Data_CharacterAssetMetadata> footwears;
            [AssetsType(Enum_CharacterAssetTypes.GLASSES)]
            public List<Data_CharacterAssetMetadata> glasses;

            [Header("Wallpaper Assets")]
            [AssetsType(Enum_CharacterAssetTypes.WALLPAPER)]
            public List<Data_CharacterAssetMetadata> wallpapers;

            [Header("Emote Assets")]
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_HAPPY)]
            public List<Data_CharacterAssetMetadata> happyEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_SAD)]
            public List<Data_CharacterAssetMetadata> sadEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_ANGRY)]
            public List<Data_CharacterAssetMetadata> angryEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_LAUGHING)]
            public List<Data_CharacterAssetMetadata> laughingEmotes; //XD
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_SURPRISED)]
            public List<Data_CharacterAssetMetadata> surprisedEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_CELEBRATION)]
            public List<Data_CharacterAssetMetadata> celebratoryEmotes;

            [Header("Tune Assets")]
            [AssetsType(Enum_CharacterAssetTypes.TUNE)]
            public List<Data_CharacterAssetMetadata> tunes;
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class AssetsType : Attribute
        {
            public AssetsType(Enum_CharacterAssetTypes type)
            {
                this.Type = type;
            }

            public Enum_CharacterAssetTypes Type { get; private set; }
        }
    }
}

