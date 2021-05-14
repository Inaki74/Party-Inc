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
            [SerializeField] protected Toggle[] _allToggles = new Toggle[24];
            protected const int FACE_LOWER_BOUND = 16; // 16->skin, 17->eyes, 18->brows, 19->nose, 20->lips, 21->makeup, 22->wrinkles, 23->beauty
            protected const int FACE_UPPER_BOUND = 23;
            protected const int LOADOUT_LOWER_BOUND = 0; // 0->hair, 1->fhair, 2->ears, 3->shirt, 4->pants, 5->socks, 6->shoes, 7->glasses
            protected const int LOADOUT_UPPER_BOUND = 7;
            protected const int EMOTES_LOWER_BOUND = 9; // 9->happy, 10->sad, 11->angry, 12->XD, 13->surprised, 14->celebration
            protected const int EMOTES_UPPER_BOUND = 14;

            //protected Enum_CharacterAssetTypes _lastToggled = ;

            [SerializeField] protected GameObject[] _allOptionsButtonsHolders = new GameObject[24];
            protected List<Toggle>[] _allOptionsToggles = new List<Toggle>[24];

            [SerializeField] protected GameObject _variableCarousel;
            [SerializeField] protected GameObject _positionsEditor;

            [SerializeField] protected Mono_VariableCarousel _theVariableCarousel;
            [SerializeField] protected Mono_FacePositionEditor _thePositionsEditor;

            [SerializeField] protected GameObject _assetButtonScrollView;
            [SerializeField] protected GameObject _assetButtonScrollViewPlayable;

            [SerializeField] protected GameObject _searchModalScreen;
            [SerializeField] protected GameObject _sortModalScreen;


            // Start is called before the first frame update
            void Start()
            {
                _theVariableCarousel = _variableCarousel.GetComponent<Mono_VariableCarousel>();

                SetOptionsButtons();
            }

            protected void OnToggleGetPositionInfo(PositionData positionData, Enum_CharacterAssetTypes assetType)
            {
                Mng_CharacterEditorChoicesCache.Current.ChangePositionData(positionData, assetType);
            }

            protected int GetIndexOfActiveToggleInRange(int start, int end)
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

            protected void OnPageChange(Enum_CharacterEditorPages thePage)
            {
                print("OnPageChange");

                if (_variableCarousel.activeInHierarchy)
                    _variableCarousel.SetActive(false);

                if (_positionsEditor.activeInHierarchy)
                    _positionsEditor.SetActive(false);

                // Smells like poop here
                // *SNNNNIFFFF*, yep poop.
                switch (thePage)
                {
                    case Enum_CharacterEditorPages.FACE:

                        int activeToggleIndexFace = GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND);

                        IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[activeToggleIndexFace].ToArray(), (Enum_CharacterAssetTypes)activeToggleIndexFace);

                        TriggerChosenAsset((Enum_CharacterAssetTypes)activeToggleIndexFace);

                        break;
                    case Enum_CharacterEditorPages.LOADOUT:

                        int activeToggleIndexLoadout = GetIndexOfActiveToggleInRange(LOADOUT_LOWER_BOUND, LOADOUT_UPPER_BOUND);

                        IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[activeToggleIndexLoadout].ToArray(), (Enum_CharacterAssetTypes)activeToggleIndexLoadout);

                        TriggerChosenAsset((Enum_CharacterAssetTypes)activeToggleIndexLoadout);

                        break;
                    case Enum_CharacterEditorPages.WALLPAPER:

                        IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[(int)Enum_CharacterAssetTypes.WALLPAPER].ToArray(), Enum_CharacterAssetTypes.WALLPAPER);

                        TriggerChosenAsset(Enum_CharacterAssetTypes.WALLPAPER);

                        break;
                    case Enum_CharacterEditorPages.EMOTE:

                        Enum_CharacterAssetTypes toggled = (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(EMOTES_LOWER_BOUND, EMOTES_UPPER_BOUND);

                        IdentifyToggledOptionsAndLoadPlayableCarousel(toggled);

                        break;
                    case Enum_CharacterEditorPages.TUNE:

                        IdentifyToggledOptionsAndLoadPlayableCarousel(Enum_CharacterAssetTypes.TUNE);

                        break;
                }
            }

            protected void TriggerChosenAsset(Enum_CharacterAssetTypes type)
            {
                print("TriggerChosenAsset");
                if (_positionsEditor.activeInHierarchy)
                {
                    return;
                }

                string chosenAssetId = Mng_CharacterEditorChoicesCache.Current.GetChosenAssetId(type);
                
                if (string.IsNullOrEmpty(chosenAssetId)) return;

                List<Mono_AssetButtonHandler> theButtons = new List<Mono_AssetButtonHandler>();
                foreach(GameObject assetScrollViewGO in _theVariableCarousel.CarouselElements)
                {
                    Mono_AssetScrollViewHandler theAssetScrollViewHandler = assetScrollViewGO.GetComponent<Mono_AssetScrollViewHandler>();
                    foreach(Mono_AssetButtonHandler theButtonHandler in theAssetScrollViewHandler.Toggles)
                    {
                        theButtons.Add(theButtonHandler);
                    }
                }

                // If the chosen asset id is a variation and the variation ISNT present, we need to find its parent here and trigger it.
                if (chosenAssetId.Contains(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR) && !theButtons.Any(b => b.AssetData.AssetId == chosenAssetId))
                {
                    chosenAssetId = chosenAssetId.Split(Mng_CharacterEditorCache.ASSET_NAME_SEPARATOR)[0];
                }

                print(chosenAssetId);

                Mono_AssetButtonHandler chosenButtonHandler = theButtons.FirstOrDefault(b => b.AssetData.AssetId == chosenAssetId);

                if (chosenButtonHandler != null && !chosenButtonHandler.Equals(default(Mono_AssetButtonHandler)))
                {
                    print("AAAAAA");
                    chosenButtonHandler.ToggleButton();
                }
            }

            protected void ActivateVariableCarousel(GameObject prefab, int amountElements, Data_CharacterAssetMetadata[] elements)
            {
                print("ActivateCarousel");
                if (_positionsEditor.activeInHierarchy)
                {
                    _positionsEditor.SetActive(false);
                }

                _variableCarousel.SetActive(true);

                _theVariableCarousel.UnstageCarousel();

                _theVariableCarousel.ElementToCarouselPrefab = prefab;
                _theVariableCarousel.AmountOfElementsPerScrollView = amountElements;

                if (elements.Length <= 0) return;

                _theVariableCarousel.InitializeCarousel(elements, OnToggleGetInfo);
            }

            protected void ActivatePositionSliders(Enum_CharacterAssetTypes assetType, Action<PositionData, Enum_CharacterAssetTypes> onToggle)
            {
                _variableCarousel.SetActive(false);
                _theVariableCarousel.UnstageCarousel();

                _positionsEditor.SetActive(true);

                _thePositionsEditor.InitializePositionEditor(Mng_CharacterEditorChoicesCache.Current.GetChosenAssetPosition(assetType), assetType, onToggle);
            }

            private void Awake()
            {
                Init();
            }

            private void OnDestroy()
            {
                Outro();
            }

            protected virtual void TogglePlayableCarousel(Data_CharacterAssetMetadata[] metadataArray, Enum_CharacterAssetTypes toggleSelected)
            {
                if (metadataArray.Length > 0)
                {
                    ActivateVariableCarousel(
                    _assetButtonScrollViewPlayable,
                    8,
                    metadataArray);

                    TriggerChosenAsset(toggleSelected);
                }
            }

            protected virtual void Init()
            {
                Mono_CharacterEditorNavigation.onChangePage += OnPageChange;
            }

            protected virtual void Outro()
            {
                Mono_CharacterEditorNavigation.onChangePage -= OnPageChange;
            }

            protected virtual void IdentifyToggledOptionsAndLoadPlayableCarousel(Enum_CharacterAssetTypes toggled)
            {
                Data_CharacterAssetMetadata[] metadataArray = Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType((int)toggled).ToArray();

                TogglePlayableCarousel(metadataArray, toggled);
            }

            protected virtual void IdentifyToggledOptionsAndLoadCarousel(Toggle[] toggles, Enum_CharacterAssetTypes toggleSelected)
            {
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
                                15,
                                Mng_CharacterEditorCache.Current.GetParentsMetadataListOfAssetType((int)toggleSelected).ToArray());
                        }
                        else if (i == 1)
                        {
                            print("color");
                            // color
                            ActivateVariableCarousel(
                                _assetButtonScrollView,
                                15,
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

            protected virtual void OnToggleGetInfo(Data_CharacterAssetMetadata assetData)
            {
                // If the current asset chosen is a variation
                // If the one im toggling is parent of the current chosen asset, dont change
                string currentChosenAssetId = Mng_CharacterEditorChoicesCache.Current.GetChosenAssetId(assetData.AssetType);

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

                Mng_CharacterEditorChoicesCache.Current.ChooseAsset(assetData.AssetId, assetData.AssetType);
            }

            protected void SetOptionsButtons()
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

            public void BtnOpenSearchModal()
            {
                _searchModalScreen.SetActive(true);
            }

            public void BtnOpenSortModal()
            {
                _sortModalScreen.SetActive(true);
            }

            protected int _optionCounter = 0;


            public void OnOptionToggle(int pageToggle)
            {
                // TODO THIS SHIT
                // runs twice
                _optionCounter++;

                if (_optionCounter % 2 != 0) return;

                print("OnOptionToggle");
                Enum_CharacterAssetTypes toggleSelected;

                // If its -1, then its a position toggle.
                // Yes its fucking terrible design, fuck me.
                // The name doesnt even fucking line up
                if (pageToggle == -1)
                {
                    toggleSelected = (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND);

                    IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[(int)toggleSelected].ToArray(), toggleSelected);

                    return;
                }

                if (pageToggle == 1)
                {
                    //face
                    toggleSelected = (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(FACE_LOWER_BOUND, FACE_UPPER_BOUND);
                }
                else if (pageToggle == 2)
                {
                    //loadout
                    toggleSelected = (Enum_CharacterAssetTypes)GetIndexOfActiveToggleInRange(LOADOUT_LOWER_BOUND, LOADOUT_UPPER_BOUND);
                }
                else
                {
                    //wallpaper
                    toggleSelected = Enum_CharacterAssetTypes.WALLPAPER;
                }

                IdentifyToggledOptionsAndLoadCarousel(_allOptionsToggles[(int)toggleSelected].ToArray(), toggleSelected);

                TriggerChosenAsset(toggleSelected);
            }

            public void OnBarToggle(int type)
            {
                if (!_allToggles[type].isOn)
                {
                    //Turned false, ignore
                    if (_allOptionsButtonsHolders[type] != null)
                    {
                        _allOptionsButtonsHolders[type].SetActive(false);
                    }
                    return;
                }

                //Toggle on option buttons
                if(_allOptionsButtonsHolders[type] != null)
                {
                    _allOptionsButtonsHolders[type].SetActive(true);
                }
                
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


