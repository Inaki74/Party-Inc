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
            [AssetsType(Enum_AssetTypes.SKINCOLOR)]
            public List<string> skinColors;
            [AssetsType(Enum_AssetTypes.EYES)]
            public List<string> eyes;
            [AssetsType(Enum_AssetTypes.BROWS)]
            public List<string> brows;
            [AssetsType(Enum_AssetTypes.NOSE)]
            public List<string> noses;
            [AssetsType(Enum_AssetTypes.LIPS)]
            public List<string> lips;
            [AssetsType(Enum_AssetTypes.MAKEUP)]
            public List<string> makeup;
            [AssetsType(Enum_AssetTypes.WRINKLES)]
            public List<string> wrinkles;
            [AssetsType(Enum_AssetTypes.BEAUTYMARKS)]
            public List<string> beautyMarks;

            [Header("Outfit Assets")]
            [AssetsType(Enum_AssetTypes.HAIR)]
            public List<string> hairs;
            [AssetsType(Enum_AssetTypes.FACIALHAIR)]
            public List<string> facialHairs;
            [AssetsType(Enum_AssetTypes.EARS)]
            public List<string> ears;
            [AssetsType(Enum_AssetTypes.SHIRT)]
            public List<string> shirts;
            [AssetsType(Enum_AssetTypes.PANTS)]
            public List<string> pants;
            [AssetsType(Enum_AssetTypes.SOCKS)]
            public List<string> socks;
            [AssetsType(Enum_AssetTypes.FOOTWEAR)]
            public List<string> footwears;
            [AssetsType(Enum_AssetTypes.GLASSES)]
            public List<string> glasses;

            [Header("Wallpaper Assets")]
            [AssetsType(Enum_AssetTypes.WALLPAPER)]
            public List<string> wallpapers;

            [Header("Emote Assets")]
            [AssetsType(Enum_AssetTypes.EMOTE_HAPPY)]
            public List<string> happyEmotes;
            [AssetsType(Enum_AssetTypes.EMOTE_SAD)]
            public List<string> sadEmotes;
            [AssetsType(Enum_AssetTypes.EMOTE_ANGRY)]
            public List<string> angryEmotes;
            [AssetsType(Enum_AssetTypes.EMOTE_LAUGHING)]
            public List<string> laughingEmotes; //XD
            [AssetsType(Enum_AssetTypes.EMOTE_SURPRISED)]
            public List<string> surprisedEmotes;
            [AssetsType(Enum_AssetTypes.EMOTE_CELEBRATION)]
            public List<string> celebratoryEmotes;

            [Header("Tune Assets")]
            [AssetsType(Enum_AssetTypes.TUNE)]
            public List<string> tunes;
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class AssetsType : Attribute
        {
            public AssetsType(Enum_AssetTypes type)
            {
                this.Type = type;
            }

            public Enum_AssetTypes Type { get; private set; }
        }
    }
}

