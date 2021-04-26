using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_FacePositionEditor : MonoBehaviour
        {
            [SerializeField] private Mono_FacialFeaturesEditor _theEditor;

            private PositionData _newPositionData;

            [SerializeField] private VerticalLayoutGroup _layoutGroup;

            [SerializeField] private GameObject _heightGO;
            [SerializeField] private GameObject _separationGO;
            [SerializeField] private GameObject _rotationGO;
            [SerializeField] private GameObject _scaleGO;
            [SerializeField] private GameObject _squashGO;

            [SerializeField] private Slider _height;
            [SerializeField] private Slider _separation;
            [SerializeField] private Slider _rotation;
            [SerializeField] private Slider _scale;
            [SerializeField] private Slider _squash;

            // Start is called before the first frame update
            void Start()
            {
                //_theEditor = FindObjectOfType<Mono_FacialFeaturesEditor>();
            }

            // Update is called once per frame
            void Update()
            {

            }

            public void InitializePositionEditor(PositionData oldPositionData, Enum_CharacterAssetTypes assetType, Action<PositionData, Enum_CharacterAssetTypes> onToggle)
            {
                _height.onValueChanged.RemoveAllListeners();
                _separation.onValueChanged.RemoveAllListeners();
                _rotation.onValueChanged.RemoveAllListeners();
                _scale.onValueChanged.RemoveAllListeners();
                _squash.onValueChanged.RemoveAllListeners();

                _heightGO.SetActive(false);
                _separationGO.SetActive(false);
                _rotationGO.SetActive(false);
                _scaleGO.SetActive(false);
                _squashGO.SetActive(false);

                switch (assetType)
                {
                    case Enum_CharacterAssetTypes.EYES:
                        EyesInit(oldPositionData, onToggle);
                        break;
                    case Enum_CharacterAssetTypes.BROWS:
                        LipsInit(oldPositionData, onToggle);
                        break;
                    case Enum_CharacterAssetTypes.LIPS:
                        LipsInit(oldPositionData, onToggle);
                        break;
                    case Enum_CharacterAssetTypes.BEAUTYMARKS:
                        break;
                }
            }

            private void OnSliderMove(float value, int sliderId, Enum_CharacterAssetTypes asset, Action<PositionData, Enum_CharacterAssetTypes> onToggle)
            {
                bool isEye = asset == Enum_CharacterAssetTypes.EYES;

                switch (sliderId)
                {
                    case 0:
                        if (isEye)
                        {
                            _theEditor.EyeHeight = value;
                        }
                        else
                        {
                            _theEditor.MouthHeight = value;
                        }
                        
                        _newPositionData.height = value;
                        break;
                    case 1:
                        _theEditor.EyeSeparation = value;
                        _newPositionData.separation = value;
                        break;
                    case 2:
                        _theEditor.EyeRotation = value;
                        _newPositionData.rotation = value;
                        break;
                    case 3:
                        if (isEye)
                        {
                            _theEditor.EyeScale = value;
                        }
                        else
                        {
                            _theEditor.MouthScale = value;
                        }
                        _newPositionData.scale = value;
                        break;
                    case 4:
                        if (isEye)
                        {
                            _theEditor.EyeSquash = value;
                        }
                        else
                        {
                            _theEditor.MouthSquash = value;
                        }
                        _newPositionData.squash = value;
                        break;
                    default:
                        break;
                }

                onToggle(_newPositionData, asset);
            }

            private void EyesInit(PositionData oldPositionData, Action<PositionData, Enum_CharacterAssetTypes> onToggle)
            {
                _layoutGroup.spacing = 80;

                _heightGO.SetActive(true);
                _separationGO.SetActive(true);
                _rotationGO.SetActive(true);
                _scaleGO.SetActive(true);
                _squashGO.SetActive(true);

                _height.minValue = Mono_FacialFeaturesEditor.EYE_HEIGHT_LOW;
                _height.maxValue = Mono_FacialFeaturesEditor.EYE_HEIGHT_HI;
                _separation.minValue = Mono_FacialFeaturesEditor.EYE_SEPARATION_LOW;
                _separation.maxValue = Mono_FacialFeaturesEditor.EYE_SEPARATION_HI;
                _rotation.minValue = Mono_FacialFeaturesEditor.EYE_ROTATION_LOW;
                _rotation.maxValue = Mono_FacialFeaturesEditor.EYE_ROTATION_HI;
                _scale.minValue = Mono_FacialFeaturesEditor.EYE_SCALE_LOW;
                _scale.maxValue = Mono_FacialFeaturesEditor.EYE_SCALE_HI;
                _squash.minValue = Mono_FacialFeaturesEditor.EYE_SQUASH_LOW;
                _squash.maxValue = Mono_FacialFeaturesEditor.EYE_SQUASH_HI;

                _height.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 0, Enum_CharacterAssetTypes.EYES, onToggle);
                });
                _separation.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 1, Enum_CharacterAssetTypes.EYES, onToggle);
                });
                _rotation.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 2, Enum_CharacterAssetTypes.EYES, onToggle);
                });
                _scale.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 3, Enum_CharacterAssetTypes.EYES, onToggle);
                });
                _squash.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 4, Enum_CharacterAssetTypes.EYES, onToggle);
                });

                _height.value = oldPositionData.height;
                _separation.value = oldPositionData.separation;
                _rotation.value = oldPositionData.rotation;
                _scale.value = oldPositionData.scale;
                _squash.value = oldPositionData.squash;
            }

            private void LipsInit(PositionData oldPositionData, Action<PositionData, Enum_CharacterAssetTypes> onToggle)
            {
                _layoutGroup.spacing = 150;

                _height.gameObject.SetActive(true);
                _scale.gameObject.SetActive(true);
                _squash.gameObject.SetActive(true);

                _height.minValue = Mono_FacialFeaturesEditor.MOUTH_HEIGHT_LOW;
                _height.maxValue = Mono_FacialFeaturesEditor.MOUTH_HEIGHT_HI;
                _scale.minValue = Mono_FacialFeaturesEditor.MOUTH_SCALE_LOW;
                _scale.maxValue = Mono_FacialFeaturesEditor.MOUTH_SCALE_HI;
                _squash.minValue = Mono_FacialFeaturesEditor.MOUTH_SQUASH_LOW;
                _squash.maxValue = Mono_FacialFeaturesEditor.MOUTH_SQUASH_HI;

                _height.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 0, Enum_CharacterAssetTypes.LIPS, onToggle);
                });
                _scale.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 3, Enum_CharacterAssetTypes.LIPS, onToggle);
                });
                _squash.onValueChanged.AddListener((value) => {
                    OnSliderMove(value, 4, Enum_CharacterAssetTypes.LIPS, onToggle);
                });

                _height.value = oldPositionData.height;
                _scale.value = oldPositionData.scale;
                _squash.value = oldPositionData.squash;
            }
        }
    }
}


