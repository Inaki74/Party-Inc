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
            [AssetsType(16)]
            public List<string> skinColors;
            [AssetsType(17)]
            public List<string> eyes;
            [AssetsType(18)]
            public List<string> brows;
            [AssetsType(19)]
            public List<string> noses;
            [AssetsType(20)]
            public List<string> lips;
            [AssetsType(21)]
            public List<string> makeup;
            [AssetsType(22)]
            public List<string> wrinkles;
            [AssetsType(23)]
            public List<string> beautyMarks;

            [Header("Outfit Assets")]
            [AssetsType(0)]
            public List<string> hairs;
            [AssetsType(1)]
            public List<string> facialHairs;
            [AssetsType(2)]
            public List<string> ears;
            [AssetsType(3)]
            public List<string> shirts;
            [AssetsType(4)]
            public List<string> pants;
            [AssetsType(5)]
            public List<string> socks;
            [AssetsType(6)]
            public List<string> footwears;
            [AssetsType(7)]
            public List<string> glasses;

            [Header("Wallpaper Assets")]
            [AssetsType(8)]
            public List<string> wallpapers;

            [Header("Emote Assets")]
            [AssetsType(9)]
            public List<string> happyEmotes;
            [AssetsType(10)]
            public List<string> sadEmotes;
            [AssetsType(11)]
            public List<string> angryEmotes;
            [AssetsType(12)]
            public List<string> laughingEmotes; //XD
            [AssetsType(13)]
            public List<string> surprisedEmotes;
            [AssetsType(14)]
            public List<string> celebratoryEmotes;

            [Header("Tune Assets")]
            [AssetsType(15)]
            public List<string> tunes;
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class AssetsType : Attribute
        {
            public AssetsType(int type)
            {
                this.Type = type;
            }

            public int Type { get; private set; }
        }
    }
}

