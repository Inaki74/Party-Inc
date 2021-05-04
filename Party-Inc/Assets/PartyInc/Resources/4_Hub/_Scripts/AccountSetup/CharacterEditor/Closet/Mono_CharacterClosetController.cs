using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CharacterClosetController : Mono_CharacterEditorController
        {
            protected override void IdentifyToggledOptionsAndLoadCarousel(Toggle[] toggles, Enum_CharacterAssetTypes toggleSelected)
            {
                // TODO: If you dont do anything else with this, please switch to an overridable property thanks.
                print("IdentifyToggledOptionsAndLoadCarousel");

                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        if (i == 0)
                        {
                            print("feature");
                            // feature
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                10,
                                Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType((int)toggleSelected).ToArray());
                        }
                        else if (i == 1)
                        {
                            print("color");
                            // color
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                10,
                                Mng_CharacterEditorCache.Current.GetVariationsOfSelectedAsset(toggleSelected).ToArray());
                        }
                        else
                        {
                            print("position");
                            // position
                            ActivatePositionSliders(toggleSelected, OnToggleGetPositionInfo);
                        }
                    }
                }
            }

            protected override void TogglePlayableCarousel(Data_CharacterAssetMetadata[] metadataArray, Enum_CharacterAssetTypes toggleSelected)
            {
                if (metadataArray.Length > 0)
                {
                    ActivateVariableCarousel(
                    _assetButtonScrollViewPlayable,
                    6,
                    metadataArray);

                    TriggerChosenAsset(toggleSelected);
                }
            }

            protected override void OnToggleGetInfo(Data_CharacterAssetMetadata assetData)
            {
                base.OnToggleGetInfo(assetData);
            }

            protected override void Init()
            {
                Mono_CharacterStoreClosetNavigation.onClosetChangePage += OnPageChange;
            }

            protected override void Outro()
            {
                Mono_CharacterStoreClosetNavigation.onClosetChangePage -= OnPageChange;
            }

            
        }
    }
}


