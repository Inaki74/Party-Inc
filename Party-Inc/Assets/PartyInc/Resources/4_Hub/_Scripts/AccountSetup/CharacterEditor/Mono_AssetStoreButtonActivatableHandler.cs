using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetStoreButtonActivatableHandler : Mono_AssetButtonHandler
        {
            [SerializeField] private Text _priceText;

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


