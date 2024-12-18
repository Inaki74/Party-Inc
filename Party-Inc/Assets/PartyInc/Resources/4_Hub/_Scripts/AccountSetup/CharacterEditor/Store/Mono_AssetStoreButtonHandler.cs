﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Mono_ToggleWithOnHoldAction))]
        public class Mono_AssetStoreButtonHandler : Mono_AssetButtonHandler
        {
            [SerializeField] private Mono_ToggleWithOnHoldAction _onHoldAction;
            [SerializeField] protected Text _priceText;
            protected AssetsStoreData storeData;

            public override void InitializeButton(Data_CharacterAssetMetadata assetData, ToggleGroup theTG, Action<Data_CharacterAssetMetadata> onToggle)
            {
                base.InitializeButton(assetData, theTG, onToggle);

                //_onHoldAction.OnHold = OnHoldButton;

                _theToggle.onValueChanged.AddListener(delegate
                {
                    OnToggleOff();
                });

                // We need:
                // Price of the asset
                // Get from the storeCache
                StartCoroutine(GetStoreInformation());
            }

            private void OnHoldButton()
            {
                Mng_CharacterEditorChoicesCache.Current.AddStoreAssetToCart(storeData);
            }

            private IEnumerator GetStoreInformation()
            {
                // I think this is unneeded
                yield return new WaitUntil(() => Mng_CharacterEditorCache.Current.GetAssetStoreReady());

                storeData = Mng_CharacterEditorCache.Current.GetAssetStoreData(AssetData.AssetId, (int)AssetData.AssetType);

                _priceText.text = storeData.baseprice.ToString();
            }

            private void OnToggleOff()
            {
                if (!_theToggle.isOn)
                {
                    print(this.name + " says: TOGGLED OFFFF");
                    Mng_CharacterEditorChoicesCache.Current.RemoveStoreAssetFromCart(storeData);
                }
            }

            protected override void AwakeOverride()
            {
                base.AwakeOverride();
            }

            protected override void DestroyOverride()
            {
                base.DestroyOverride();
                _theToggle.onValueChanged.RemoveAllListeners();
            }
        }
    }
}


