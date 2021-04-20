using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle))]
        public class Mono_AssetButtonPlayableHandler : MonoBehaviour
        {
            [SerializeField] private Sprite _pausedSprite;
            [SerializeField] private Sprite _playingSprite;
            [SerializeField] private Image _assetImage;

            private string _assetId;
            private Toggle _theToggle;
            private Enum_CharacterAssetTypes _assetType;

            //DATA

            private void Awake()
            {
                _theToggle = GetComponent<Toggle>();
            }

            private void OnDestroy()
            {
                if (_assetType == Enum_CharacterAssetTypes.TUNE)
                {
                    _theToggle.onValueChanged.RemoveListener(delegate {
                        TuneEvent();
                    });
                }
            }

            public void InitializeButton(CharacterAsset assetId, ToggleGroup theTG, Enum_CharacterAssetTypes assetTypes)
            {
                _assetId = assetId.id;

                _theToggle.group = theTG;

                _assetType = assetTypes;

                if(_assetType == Enum_CharacterAssetTypes.TUNE)
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