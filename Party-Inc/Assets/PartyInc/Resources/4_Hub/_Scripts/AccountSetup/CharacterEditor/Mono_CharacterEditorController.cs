using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CharacterEditorController : MonoBehaviour
        {
            
            [SerializeField] private Toggle[] _allToggles = new Toggle[24];
            private const int FACE_LOWER_BOUND = 16; // 16->skin, 17->eyes, 18->brows, 19->nose, 20->lips, 21->makeup, 22->wrinkles, 23->beauty
            private const int FACE_UPPER_BOUND = 23;
            private const int OUTFIT_LOWER_BOUND = 0; // 0->hair, 1->fhair, 2->ears, 3->shirt, 4->pants, 5->socks, 6->shoes, 7->glasses
            private const int OUTFIT_UPPER_BOUND = 7;
            private const int EMOTES_LOWER_BOUND = 9; // 9->happy, 10->sad, 11->angry, 12->XD, 13->surprised, 14->celebration
            private const int EMOTES_UPPER_BOUND = 14;

            //private Enum_CharacterAssetTypes _lastToggled = ;

            [SerializeField] private GameObject[] _allOptionsButtonsHolders = new GameObject[24];
            private List<Toggle>[] _allOptionsToggles = new List<Toggle>[24];

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

                SetOptionsButtons();
            }

            private void OnToggleGetInfo(Data_CharacterAssetMetadata assetData)
            {
                // If the current asset chosen is a variation
                // If the one im toggling is parent of the current chosen asset, dont change
                string currentChosenAssetId = Mng_CharacterEditorCache.Current.GetChosenAssetId(assetData.AssetType);

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

                Mng_CharacterEditorCache.Current.ChooseAsset(assetData.AssetId, assetData.AssetType);
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
                print("OnPageChange");

                if (_variableCarousel.activeInHierarchy)
                    _variableCarousel.SetActive(false);

                if (_constantCarousel.activeInHierarchy)
                    _constantCarousel.SetActive(false);

                if (_positionsEditor.activeInHierarchy)
                    _positionsEditor.SetActive(false);

                // Smells like poop here
                // *SNNNNIFFFF*, yep poop.
                switch (thePage)
                {
                    case Enum_CharacterEditorPages.OVERVIEW:
                        // Might do nothing
                        break;
                    case Enum_CharacterEditorPages.FACE:

                        int activeToggleIndexFace = GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND);

                        IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[activeToggleIndexFace].ToArray(), (Enum_CharacterAssetTypes)activeToggleIndexFace);

                        TriggerChosenAsset((Enum_CharacterAssetTypes)activeToggleIndexFace);

                        break;
                    case Enum_CharacterEditorPages.OUTFIT:

                        int activeToggleIndexOutfit = GetIndexOfActiveToggleInRange(OUTFIT_LOWER_BOUND, OUTFIT_UPPER_BOUND);

                        IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[activeToggleIndexOutfit].ToArray(), (Enum_CharacterAssetTypes)activeToggleIndexOutfit);

                        TriggerChosenAsset((Enum_CharacterAssetTypes)activeToggleIndexOutfit);

                        break;
                    case Enum_CharacterEditorPages.WALLPAPER:

                        IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[(int)Enum_CharacterAssetTypes.WALLPAPER].ToArray(), Enum_CharacterAssetTypes.WALLPAPER);

                        TriggerChosenAsset(Enum_CharacterAssetTypes.WALLPAPER);

                        break;
                    case Enum_CharacterEditorPages.EMOTE:

                        Data_CharacterAssetMetadata[] emotesMetadataArray = Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType(GetIndexOfActiveToggleInRange(EMOTES_LOWER_BOUND, EMOTES_UPPER_BOUND)).ToArray();

                        if (emotesMetadataArray.Length > 0)
                        {
                            ActivateVariableCarousel(
                            _assetButtonScrollViewPlayable,
                            8,
                            emotesMetadataArray);

                            TriggerChosenAsset((Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(EMOTES_LOWER_BOUND, EMOTES_UPPER_BOUND));
                        }

                        break;
                    case Enum_CharacterEditorPages.TUNE:

                        Data_CharacterAssetMetadata[] tuneMetadataArray = Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType((int)Enum_CharacterAssetTypes.TUNE).ToArray();

                        if (tuneMetadataArray.Length > 0)
                        {
                            ActivateVariableCarousel(
                            _assetButtonScrollViewPlayable,
                            8,
                            tuneMetadataArray);

                            TriggerChosenAsset(Enum_CharacterAssetTypes.TUNE);
                        }

                        break;
                    case Enum_CharacterEditorPages.PHOTO:
                        break;
                    case Enum_CharacterEditorPages.SIGNUP:
                        break;
                    default:
                        break;
                }
            }

            private void TriggerChosenAsset(Enum_CharacterAssetTypes type)
            {
                print("TriggerChosenAsset");
                // The system must remember which choice i made.
                // I can get the choice made from the dictionary of chosen assets
                string chosenAssetId = Mng_CharacterEditorCache.Current.GetChosenAssetId(type);
                
                //There are no objects.
                if (string.IsNullOrEmpty(chosenAssetId)) return;

                // If the chosen asset id is a variation, we need to find its parent here and trigger it.
                if (chosenAssetId.Contains(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR))
                {
                    //Its a variation
                    chosenAssetId = chosenAssetId.Split(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR)[0];
                }

                // Now i need to look through the list of all the active toggles
                // To get that list i need to get the carousel elements
                // Foreach element get its AssetScrollView component
                // Get the buttons it has
                // Thats the list
                List<Mono_AssetButtonHandler> theButtons = new List<Mono_AssetButtonHandler>();
                foreach(GameObject assetScrollViewGO in _theVariableCarousel.CarouselElements)
                {
                    Mono_AssetScrollViewHandler theAssetScrollViewHandler = assetScrollViewGO.GetComponent<Mono_AssetScrollViewHandler>();
                    foreach(Mono_AssetButtonHandler theButtonHandler in theAssetScrollViewHandler.Toggles)
                    {
                        theButtons.Add(theButtonHandler);
                    }
                }

                print(theButtons.Count);

                //if (theButtons.Count <= 0) return;

                // Find the one bound to the chosen asset
                Mono_AssetButtonHandler chosenButtonHandler = theButtons.First(b => b.AssetData.AssetId == chosenAssetId);

                // Toggle it.
                chosenButtonHandler.ToggleButton();
            }

            private void ActivateVariableCarousel(GameObject prefab, int amountElements, Data_CharacterAssetMetadata[] elements)
            {
                print("ActivateCarousel");
                _variableCarousel.SetActive(true);

                _theVariableCarousel.UnstageCarousel();

                _theVariableCarousel.ElementToCarouselPrefab = prefab;
                _theVariableCarousel.AmountOfElementsPerScrollView = amountElements;

                if (elements.Length <= 0) return;



                _theVariableCarousel.InitializeCarousel(elements, OnToggleGetInfo);
            }

            private void Awake()
            { 
                Mono_CharacterEditorNavigation.onChangePage += OnPageChange;
            }

            private void OnDestroy()
            {
                Mono_CharacterEditorNavigation.onChangePage -= OnPageChange;
            }

            private void IdentifyToggledOptionsAndLoadCarousel(Toggle[] toggles, Enum_CharacterAssetTypes toggleSelected)
            {
                print("IdenitfyToggledOptionsAndLoadCarousel");

                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        if (i == 0)
                        {
                            // feature
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                15,
                                Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType((int)toggleSelected).ToArray());
                        }
                        else if (i == 1)
                        {
                            // color
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                15,
                                Mng_CharacterEditorCache.Current.GetVariationsOfSelectedAsset(toggleSelected).ToArray());
                        }
                        else
                        {
                            // position
                        }
                    }
                }
            }

            private void SetOptionsButtons()
            {

                for(int i = 0; i < _allOptionsButtonsHolders.Length; i++)
                {
                    if (_allOptionsButtonsHolders[i] == null) continue;

                    _allOptionsToggles[i] = new List<Toggle>();

                    foreach(Toggle theToggle in _allOptionsButtonsHolders[i].GetComponentsInChildren<Toggle>())
                    {
                        _allOptionsToggles[i].Add(theToggle);
                    }
                }
            }

            private int _optionCounter = 0;

            public void OnOptionToggle(int pageToggle)
            {
                // Toggle groups always deactivate the toggled toggle and then activate the one that was selected.
                // Therefore, if we have a counter starting at 0, we only accept the ones that have the counter be a pair number
                // runs twice
                _optionCounter++;

                if (_optionCounter % 2 != 0) return;

                print("OnOptionToggle");
                Enum_CharacterAssetTypes toggleSelected;
                if (pageToggle == 1)
                {
                    //face
                    toggleSelected = (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND);
                }
                else if (pageToggle == 2)
                {
                    //outfit
                    toggleSelected = (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(OUTFIT_LOWER_BOUND, OUTFIT_UPPER_BOUND);
                }
                else
                {
                    //wallpaper
                    toggleSelected = Enum_CharacterAssetTypes.WALLPAPER;
                }

                IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[(int)toggleSelected].ToArray(), toggleSelected);

                TriggerChosenAsset(toggleSelected);
            }

            private int _barCounter = 0;

            public void OnBarToggle(int type)
            {
                print(type);

                // runs twice
                //_barCounter++;

                //_allOptionsButtonsHolders[type].SetActive(false);

                //if (_barCounter % 2 != 0) return;

                //Toggle on option buttons
                _allOptionsButtonsHolders[type].SetActive(true);

                print("OnBarToggle");
                if (type >= (int)Enum_CharacterAssetTypes.EMOTE_HAPPY && type <= (int)Enum_CharacterAssetTypes.TUNE)
                {
                    ActivateVariableCarousel(_assetButtonScrollViewPlayable, 8, Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType(type).ToArray());

                    TriggerChosenAsset((Enum_CharacterAssetTypes)type);
                }
                else
                {

                    IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[type].ToArray(), (Enum_CharacterAssetTypes)type);

                    TriggerChosenAsset((Enum_CharacterAssetTypes)type);
                }
            }
        }
    }
}


