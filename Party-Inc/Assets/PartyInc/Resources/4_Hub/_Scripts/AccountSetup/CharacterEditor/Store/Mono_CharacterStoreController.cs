using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                Data_CharacterAssetMetadata[] metadataArray = Mng_CharacterEditorCache.Current.GetParentsDisplayAssetsMetadata((int)toggled).ToArray();

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
                                Mng_CharacterEditorCache.Current.GetParentsDisplayAssetsMetadata((int)toggleSelected).ToArray());
                        }
                        else if (i == 1)
                        {
                            // Closet buttons
                            print("color");
                            // color
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                10,
                                Mng_CharacterEditorCache.Current.GetVariationsOfSelectedAsset(toggleSelected).ToArray());
                        }
                    }
                }
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
                // This looks the same
                base.OnToggleGetInfo(assetData);
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


