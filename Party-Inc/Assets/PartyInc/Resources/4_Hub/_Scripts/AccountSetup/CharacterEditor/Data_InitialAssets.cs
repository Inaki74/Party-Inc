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
            public List<string> skinColors;
            [AssetsType(Enum_CharacterAssetTypes.EYES)]
            public List<string> eyes;
            [AssetsType(Enum_CharacterAssetTypes.BROWS)]
            public List<string> brows;
            [AssetsType(Enum_CharacterAssetTypes.NOSE)]
            public List<string> noses;
            [AssetsType(Enum_CharacterAssetTypes.LIPS)]
            public List<string> lips;
            [AssetsType(Enum_CharacterAssetTypes.MAKEUP)]
            public List<string> makeup;
            [AssetsType(Enum_CharacterAssetTypes.WRINKLES)]
            public List<string> wrinkles;
            [AssetsType(Enum_CharacterAssetTypes.BEAUTYMARKS)]
            public List<string> beautyMarks;

            [Header("Outfit Assets")]
            [AssetsType(Enum_CharacterAssetTypes.HAIR)]
            public List<string> hairs;
            [AssetsType(Enum_CharacterAssetTypes.FACIALHAIR)]
            public List<string> facialHairs;
            [AssetsType(Enum_CharacterAssetTypes.EARS)]
            public List<string> ears;
            [AssetsType(Enum_CharacterAssetTypes.SHIRT)]
            public List<string> shirts;
            [AssetsType(Enum_CharacterAssetTypes.PANTS)]
            public List<string> pants;
            [AssetsType(Enum_CharacterAssetTypes.SOCKS)]
            public List<string> socks;
            [AssetsType(Enum_CharacterAssetTypes.FOOTWEAR)]
            public List<string> footwears;
            [AssetsType(Enum_CharacterAssetTypes.GLASSES)]
            public List<string> glasses;

            [Header("Wallpaper Assets")]
            [AssetsType(Enum_CharacterAssetTypes.WALLPAPER)]
            public List<string> wallpapers;

            [Header("Emote Assets")]
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_HAPPY)]
            public List<string> happyEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_SAD)]
            public List<string> sadEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_ANGRY)]
            public List<string> angryEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_LAUGHING)]
            public List<string> laughingEmotes; //XD
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_SURPRISED)]
            public List<string> surprisedEmotes;
            [AssetsType(Enum_CharacterAssetTypes.EMOTE_CELEBRATION)]
            public List<string> celebratoryEmotes;

            [Header("Tune Assets")]
            [AssetsType(Enum_CharacterAssetTypes.TUNE)]
            public List<string> tunes;
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

