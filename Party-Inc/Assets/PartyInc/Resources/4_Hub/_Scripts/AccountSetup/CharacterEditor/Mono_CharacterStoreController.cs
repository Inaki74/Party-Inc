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
            protected override void IdentifyToggledOptionsAndLoadCarousel(Toggle[] toggles, Enum_CharacterAssetTypes toggleSelected)
            {
                base.IdentifyToggledOptionsAndLoadCarousel(toggles, toggleSelected);
            }

            protected override void TogglePlayableCarousel(Data_CharacterAssetMetadata[] metadataArray, Enum_CharacterAssetTypes toggleSelected)
            {
                base.TogglePlayableCarousel(metadataArray, toggleSelected);
            }

            protected override void OnToggleGetInfo(Data_CharacterAssetMetadata assetData)
            {
                base.OnToggleGetInfo(assetData);
            }

            protected override void Init()
            {
                Mono_CharacterStoreClosetNavigation.onStoreChangePage += OnPageChange;
            }

            protected override void Outro()
            {
                Mono_CharacterStoreClosetNavigation.onStoreChangePage += OnPageChange;
            }
        }
    }
}


