using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_AssetButtonPlayableHandler : Mono_AssetButtonHandler
        {
            [SerializeField] private Sprite _pausedSprite;
            [SerializeField] private Sprite _playingSprite;
            [SerializeField] private Image _assetImage;

            //DATA
            protected override void DestroyOverride()
            {
                base.DestroyOverride();

                if (_assetData.AssetType == Enum_CharacterAssetTypes.TUNE)
                {
                    _theToggle.onValueChanged.RemoveListener(delegate {
                        TuneEvent();
                    });
                }
            }

            public override void InitializeButton(Data_CharacterAssetMetadata assetData, ToggleGroup theTG, Action<Data_CharacterAssetMetadata> onToggle)
            {
                base.InitializeButton(assetData, theTG, onToggle);

                if (assetData.AssetType == Enum_CharacterAssetTypes.TUNE)
                {
                    if (_theToggle.isOn)
                    {
                        _assetImage.sprite = _pausedSprite;
                    }
                    else
                    {
                        _assetImage.sprite = _playingSprite;
                    }

                    _assetImage.SetNativeSize();

                    _theToggle.onValueChanged.AddListener(delegate {
                        TuneEvent();
                    });
                }
            }

            private void TuneEvent()
            {
                if (_theToggle.isOn)
                {
                    _assetImage.sprite = _pausedSprite;
                }
                else
                {
                    _assetImage.sprite = _playingSprite;
                }

                _assetImage.SetNativeSize();
            }
        }
    }
}