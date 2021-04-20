using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CharacterEditorController : MonoBehaviour
        {
            // Face Toggles
            [SerializeField] private Toggle[] _allToggles = new Toggle[24];
            private const int FACE_LOWER_BOUND = 16; // 16->skin, 17->eyes, 18->brows, 19->nose, 20->lips, 21->makeup, 22->wrinkles, 23->beauty
            private const int FACE_UPPER_BOUND = 23;
            private const int OUTFIT_LOWER_BOUND = 0; // 0->hair, 1->fhair, 2->ears, 3->shirt, 4->pants, 5->socks, 6->shoes, 7->glasses
            private const int OUTFIT_UPPER_BOUND = 7;
            private const int EMOTES_LOWER_BOUND = 9; // 9->happy, 10->sad, 11->angry, 12->XD, 13->surprised, 14->celebration
            private const int EMOTES_UPPER_BOUND = 14;

            [SerializeField] private Toggle[] _faceOptionsButtons = new Toggle[3]; // 1->Features, 2->Color, 3->Position
            [SerializeField] private Toggle[] _outfitOptionsButtons = new Toggle[2]; // 1->Features, 2->Color

            [SerializeField] private GameObject _variableCarousel;
            [SerializeField] private GameObject _constantCarousel;
            [SerializeField] private GameObject _positionsEditor;

            [SerializeField] private Mono_VariableCarousel _theVariableCarousel;

            [SerializeField] private GameObject _assetButtonScrollView;
            [SerializeField] private GameObject _assetButtonScrollViewPlayable;

            // Start is called before the first frame update
            void Start()
            {
                _theVariableCarousel = _variableCarousel.GetComponent<Mono_VariableCarousel>();
            }

            // Update is called once per frame
            void Update()
            {
            }

            private int GetIndexOfActiveToggleInRange(int start, int end)
            {
                for(int i = start; i <= end; i++)
                {
                    if (_allToggles[i].isOn)
                    {
                        return i;
                    }
                }

                print("ERROR: NO TOGGLE FOUND ON.");
                return -1;
            }

            private void OnPageChange(Enum_CharacterEditorPages thePage)
            {
                if (_variableCarousel.activeInHierarchy)
                    _variableCarousel.SetActive(false);

                if (_constantCarousel.activeInHierarchy)
                    _constantCarousel.SetActive(false);

                if (_positionsEditor.activeInHierarchy)
                    _positionsEditor.SetActive(false);

                switch (thePage)
                {
                    case Enum_CharacterEditorPages.OVERVIEW:
                        // Might do nothing
                        break;
                    case Enum_CharacterEditorPages.FACE:

                        if (_faceOptionsButtons[0].isOn)
                        {
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                15,
                                Mng_CharacterEditorCache.Current.GetTypeList(GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND)).ToArray(),
                                (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND));
                        }
                        else if (_faceOptionsButtons[1].isOn)
                        {
                            // Will use a variable carousel, but with a fixed list
                            //ActivateVariableCarousel(
                            //    _assetButtonScrollView,
                            //    15,
                            //    Mng_CharacterEditorCache.Current.GetVariations().ToArray(),
                            //    (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND)
                            //);

                            _constantCarousel.SetActive(true);
                        }
                        else
                        {
                            _positionsEditor.SetActive(true);
                        }

                        break;
                    case Enum_CharacterEditorPages.OUTFIT:

                        if (_outfitOptionsButtons[0].isOn)
                        {
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                15,
                                Mng_CharacterEditorCache.Current.GetTypeList(GetIndexOfActiveToggleInRange(OUTFIT_LOWER_BOUND, OUTFIT_UPPER_BOUND)).ToArray(),
                                (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(OUTFIT_LOWER_BOUND, OUTFIT_UPPER_BOUND));
                        }
                        else
                        {
                            // Will use a variable carousel, but with a fixed list
                            _constantCarousel.SetActive(true);
                        }

                        break;
                    case Enum_CharacterEditorPages.WALLPAPER:
                        ActivateVariableCarousel(
                            _assetButtonScrollView,
                            15,
                            Mng_CharacterEditorCache.Current.GetTypeList((int)Enum_CharacterAssetTypes.WALLPAPER).ToArray(),
                            Enum_CharacterAssetTypes.WALLPAPER);
                        break;
                    case Enum_CharacterEditorPages.EMOTE:
                        ActivateVariableCarousel(
                            _assetButtonScrollViewPlayable,
                            8,
                            Mng_CharacterEditorCache.Current.GetTypeList(GetIndexOfActiveToggleInRange(EMOTES_LOWER_BOUND, EMOTES_UPPER_BOUND)).ToArray(),
                            (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(EMOTES_LOWER_BOUND, EMOTES_UPPER_BOUND));
                        break;
                    case Enum_CharacterEditorPages.TUNE:
                        ActivateVariableCarousel(
                            _assetButtonScrollViewPlayable,
                            8,
                            Mng_CharacterEditorCache.Current.GetTypeList((int)Enum_CharacterAssetTypes.TUNE).ToArray(),
                            Enum_CharacterAssetTypes.TUNE);
                        break;
                    case Enum_CharacterEditorPages.PHOTO:
                        break;
                    case Enum_CharacterEditorPages.SIGNUP:
                        break;
                    default:
                        break;
                }
            }

            private void ActivateVariableCarousel(GameObject prefab, int amountElements, CharacterAsset[] elements, Enum_CharacterAssetTypes assetType)
            {
                _variableCarousel.SetActive(true);

                _theVariableCarousel.UnstageCarousel();

                _theVariableCarousel.ElementToCarouselPrefab = prefab;
                _theVariableCarousel.AmountOfElementsPerScrollView = amountElements;

                _theVariableCarousel.InitializeCarousel(elements, assetType);
            }

            private void Awake()
            {
                Mono_CharacterEditorNavigation.onChangePage += OnPageChange;
            }

            private void OnDestroy()
            {
                Mono_CharacterEditorNavigation.onChangePage -= OnPageChange;
            }

            public void OnToggle(int type)
            {
                if(type >= (int)Enum_CharacterAssetTypes.EMOTE_HAPPY && type <= (int)Enum_CharacterAssetTypes.TUNE)
                {
                    ActivateVariableCarousel(_assetButtonScrollViewPlayable, 8, Mng_CharacterEditorCache.Current.GetTypeList(type).ToArray(), (Enum_CharacterAssetTypes)type);
                }
                else
                {
                    ActivateVariableCarousel(_assetButtonScrollView, 15, Mng_CharacterEditorCache.Current.GetTypeList(type).ToArray(), (Enum_CharacterAssetTypes)type);
                }
            }
        }
    }
}


