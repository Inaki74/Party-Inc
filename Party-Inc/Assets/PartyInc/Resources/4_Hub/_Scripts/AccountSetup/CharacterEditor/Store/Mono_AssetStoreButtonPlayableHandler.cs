﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetStoreButtonPlayableHandler : Mono_AssetStoreButtonHandler
        {
            public override void InitializeButton(Data_CharacterAssetMetadata assetData, ToggleGroup theTG, Action<Data_CharacterAssetMetadata> onToggle)
            {
                base.InitializeButton(assetData, theTG, onToggle);

                

            }

            protected override void AwakeOverride()
            {
                base.AwakeOverride();
            }

            protected override void DestroyOverride()
            {
                base.DestroyOverride();
            }
        }
    }
}

