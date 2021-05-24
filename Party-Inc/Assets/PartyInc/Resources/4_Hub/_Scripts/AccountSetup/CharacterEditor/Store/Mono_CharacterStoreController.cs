using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CharacterStoreController : Mono_CharacterEditorController
        {
            [SerializeField] private GameObject _storeAssetButtonScrollView;
            [SerializeField] private GameObject _storeAssetButtonScrollViewPlayable;

            [SerializeField] private GameObject _checkoutModalScreen;
            [SerializeField] private GameObject _indicatorBadge;
            [SerializeField] private Text _indicatorCountText;

            public void BtnOpenCheckoutModal()
            {
                _checkoutModalScreen.SetActive(true);
            }

            protected override void IdentifyToggledOptionsAndLoadPlayableCarousel(Enum_CharacterAssetTypes toggled)
            {
                Data_CharacterAssetMetadata[] metadataArray = Mng_CharacterEditorCache.Current.GetParentsStoreAssetsMetadataOfAssetType((int)toggled).ToArray();

                TogglePlayableCarousel(metadataArray, toggled);
            }

            protected override void IdentifyToggledOptionsAndLoadCarousel(Toggle[] toggles, Enum_CharacterAssetTypes toggleSelected)
            {
                print("IdentifyToggledOptionsAndLoadCarousel STORE");

                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        if (i == 0)
                        {
                            // Store buttons
                            // This activation needs to be different (button hold)
                            print("feature");
                            // feature
                            ActivateVariableCarousel(
                                _storeAssetButtonScrollView,
                                10,
                                Mng_CharacterEditorCache.Current.GetParentsStoreAssetsMetadataOfAssetType((int)toggleSelected).ToArray());
                        }
                        else if (i == 1)
                        {
                            // Closet buttons
                            print("color");
                            // color
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                10,
                                Mng_CharacterEditorCache.Current.GetVariationsOfAsset(Mng_CharacterEditorChoicesCache.Current.GetChosenStoreAssetId(toggleSelected), toggleSelected).ToArray());
                        }
                    }
                }
            }

            protected override void TriggerChosenAsset(Enum_CharacterAssetTypes type)
            {
                print("TriggerChosenAsset");
                if (_positionsEditor.activeInHierarchy)
                {
                    return;
                }

                string chosenAssetId = Mng_CharacterEditorChoicesCache.Current.GetChosenStoreAssetId(type);

                TriggerChosenAsset(chosenAssetId);
            }

            protected override void TogglePlayableCarousel(Data_CharacterAssetMetadata[] metadataArray, Enum_CharacterAssetTypes toggleSelected)
            {
                if (metadataArray.Length > 0)
                {
                    ActivateVariableCarousel(
                    _storeAssetButtonScrollViewPlayable,
                    6,
                    metadataArray);

                    TriggerChosenAsset(toggleSelected);
                }
            }

            protected override void OnToggleGetInfo(Data_CharacterAssetMetadata assetData)
            {
                // If the current asset chosen is a variation
                // If the one im toggling is parent of the current chosen asset, dont change
                string currentChosenAssetId = Mng_CharacterEditorChoicesCache.Current.GetChosenStoreAssetId(assetData.AssetType);

                if (!string.IsNullOrEmpty(currentChosenAssetId))
                {
                    if (currentChosenAssetId.Contains(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR))
                    {
                        // Its a variation
                        string[] splitCurrentChosenAssetId = currentChosenAssetId.Split(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR);
                        if (splitCurrentChosenAssetId[0] == assetData.AssetId)
                        {
                            // The current chosen asset is a child of the triggered button
                            return;
                        }
                    }
                }

                Mng_CharacterEditorChoicesCache.Current.SetChosenStoreAssetId(assetData.AssetId, assetData.AssetType);

                bool isAVariation = assetData.AssetId.Contains(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR);

                if (!isAVariation)
                {
                    AssetsStoreData assetStoreData = Mng_CharacterEditorCache.Current.GetAssetStoreData(assetData.AssetId, (int)assetData.AssetType);
                    Mng_CharacterEditorChoicesCache.Current.AddStoreAssetToCart(assetStoreData);
                }
            }

            protected override void Init()
            {
                Mono_CharacterStoreClosetNavigation.onStoreChangePage += OnPageChange;
                Mono_CharacterStoreChoicesCache.onChosenAssetListModified += UpdateCheckoutIndicator;
            }

            protected override void Outro()
            {
                Mono_CharacterStoreClosetNavigation.onStoreChangePage -= OnPageChange;
                Mono_CharacterStoreChoicesCache.onChosenAssetListModified -= UpdateCheckoutIndicator;
            }

            private void UpdateCheckoutIndicator(int amountOfElements)
            {
                if(amountOfElements <= 0)
                {
                    _indicatorBadge.SetActive(false);
                    return;
                }

                if (!_indicatorBadge.activeInHierarchy)
                {
                    _indicatorBadge.SetActive(true);
                }

                _indicatorCountText.text = amountOfElements.ToString();
            }
        }
    }
}


