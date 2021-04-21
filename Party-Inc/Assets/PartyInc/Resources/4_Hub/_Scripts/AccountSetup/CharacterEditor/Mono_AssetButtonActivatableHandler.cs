using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetButtonActivatableHandler : Mono_AssetButtonHandler
        {
            public override void InitializeButton(Data_CharacterAssetMetadata assetData, ToggleGroup theTG, Action<Data_CharacterAssetMetadata> onToggle)
            {
                base.InitializeButton(assetData, theTG, onToggle);
            }
        }
    }
}


