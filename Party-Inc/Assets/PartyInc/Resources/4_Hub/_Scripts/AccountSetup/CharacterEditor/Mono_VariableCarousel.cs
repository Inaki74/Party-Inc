using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using DentedPixel;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_VariableCarousel : MonoBehaviour
        {
            private GraphicRaycaster _raycaster;
            private PointerEventData _pointerEventData;
            private EventSystem _eventSystem;

            [SerializeField] private ToggleGroup _elementsToggleGroup;

            [SerializeField] private GameObject _elementToCarouselPrefab;
            public GameObject ElementToCarouselPrefab
            {
                get
                {
                    return _elementToCarouselPrefab;
                }
                set
                {
                    _elementToCarouselPrefab = value;
                }
            }

            [SerializeField] private int _amountOfElementsPerScrollView;
            public int AmountOfElementsPerScrollView
            {
                get
                {
                    return _amountOfElementsPerScrollView;
                }
                set
                {
                    _amountOfElementsPerScrollView = value;
                }
            }

            private List<GameObject> _carouselElements = new List<GameObject>();
            public List<GameObject> CarouselElements
            {
                get
                {
                    return _carouselElements;
                }
                private set
                {
                    _carouselElements = value;
                }
            }

            [SerializeField] private float _carouselMovingDistance;
            [SerializeField] private ContentType _contentType;
            [SerializeField] private RectTransform _carousel;
            [SerializeField] private float _carouselSpeed;

            private enum ContentType
            {
                AssetScrollView,
                AssetScrollViewPlayable
            }

            private int _carouselLength;
            private int _lastCarouselSpot;
            private int _carouselSpot; // 0 to _carouselLength - 1
            private bool _carouselMoving;

            // Place indicator variables
            [SerializeField] private GameObject _placeIndicatorPrefab;
            [SerializeField] private GameObject _placeIndicatorHolder;
            private Button[] _placeIndicators;

            // Swipe variables
            private int _swipeDirection;
            private bool _swipe;
            private float _startingX;
            private float _acumulativeThreshold = (float)Screen.width / 10f;

            private void Start()
            {
                _carouselSpot = 0;

                //Fetch the Raycaster from the GameObject (the Canvas)
                _raycaster = GetComponent<GraphicRaycaster>();
                //Fetch the Event System from the Scene
                _eventSystem = FindObjectOfType<EventSystem>();

                // Get the information required
                //InitializeCarousel(test.ToArray());
            }

            private void Update()
            {
                if(Input.touchCount > 0)
                {
                    CheckIfCarouselSwipe();
                }

                if (_swipe && !_carouselMoving)
                {
                    _carouselMoving = true;

                    // Move the carousel one place.
                    StartCoroutine(CarouselSwipeCo(_swipeDirection));

                    UpdateCarouselSpotAndIndicators();
                }
            }

            private void UpdateCarouselSpotAndIndicators()
            {
                _lastCarouselSpot = _carouselSpot;

                if (_swipeDirection > 0)
                {
                    _carouselSpot--;
                }
                else
                {
                    _carouselSpot++;
                }

                _placeIndicators[_lastCarouselSpot].interactable = false;
                _placeIndicators[_carouselSpot].interactable = true;
            }

            private IEnumerator CarouselSwipeCo(float swipeDirection)
            {
                // Lerp the holder in direction
                float originalPosition = _carousel.localPosition.x;
                float destination = originalPosition + _carouselMovingDistance * swipeDirection;

                LeanTween.moveLocalX(_carousel.gameObject, destination, 0.5f).setEaseOutExpo();

                yield return new WaitForSeconds(0.5f);

                _carouselMoving = false;
                _swipe = false;
            }

            public void InitializeCarousel(Data_CharacterAssetMetadata[] elements, Action<Data_CharacterAssetMetadata> onToggle)
            {
                if(elements.Count() == 0)
                {
                    print("No elements");
                }

                int amountOfButtons = elements.Length;
                int amountOfCarouselElements = Mathf.CeilToInt((float)amountOfButtons / (float)_amountOfElementsPerScrollView);

                _carouselLength = amountOfCarouselElements;

                _placeIndicators = new Button[amountOfCarouselElements];

                for(int i = 0; i < amountOfCarouselElements; i++)
                {
                    int amountNewList = 0;

                    // 
                    if(amountOfButtons - i * _amountOfElementsPerScrollView >= _amountOfElementsPerScrollView)
                    {
                        amountNewList = _amountOfElementsPerScrollView;
                    }
                    else
                    {
                        amountNewList = amountOfButtons - i * _amountOfElementsPerScrollView;
                    }

                    Data_CharacterAssetMetadata[] newList = new Data_CharacterAssetMetadata[amountNewList];

                    for(int j = 0; j < amountNewList; j++)
                    {
                        newList[j] = elements[i * _amountOfElementsPerScrollView + j];
                    }

                    GameObject elementToCarousel = Instantiate(_elementToCarouselPrefab, transform);
                    _carouselElements.Add(elementToCarousel);
                    
                    if(_contentType == ContentType.AssetScrollView)
                    {
                        elementToCarousel.GetComponent<Mono_AssetScrollViewHandler>().InitializeScrollview(newList, _elementsToggleGroup, onToggle);
                    }

                    elementToCarousel.GetComponent<RectTransform>().transform.SetParent(_carousel.transform);
                    elementToCarousel.GetComponent<RectTransform>().localPosition = new Vector2(_carouselMovingDistance * i, -437f); // -437 is the value I found leaves the element in the right place

                    GameObject carouselIndicator = Instantiate(_placeIndicatorPrefab, _placeIndicatorHolder.transform);
                    _placeIndicators[i] = carouselIndicator.GetComponent<Button>();
                    if(i == 0)
                    {
                        _placeIndicators[i].interactable = true;
                    }
                    else
                    {
                        _placeIndicators[i].interactable = false;
                    }
                    
                }

                _lastCarouselSpot = 0;
            }

            public void UnstageCarousel()
            {
                _carousel.localPosition = new Vector2(0f, _carousel.localPosition.y);

                for(int i = 0; i < _placeIndicatorHolder.transform.childCount; i++)
                {
                    Destroy(_carousel.transform.GetChild(i).gameObject);
                    Destroy(_placeIndicatorHolder.transform.GetChild(i).gameObject);
                }

                _carouselSpot = 0;
                _carouselElements.Clear();
                _placeIndicators = null;

                _swipe = false;
                
            }

            private void CheckIfCarouselSwipe()
            {
                if (CheckForFingerOnCarousel())
                {
                    if (!_swipe)
                    {
                        if (Input.touches[0].phase == TouchPhase.Began)
                        {
                            _startingX = Input.touches[0].position.x;
                        }

                        _swipe = CheckForSwipe();

                        if (Input.touches[0].phase == TouchPhase.Ended)
                        {
                            _startingX = 0f;
                        }
                    }
                }
            }

            private bool CheckForFingerOnCarousel()
            {
                //Set up the new Pointer Event
                _pointerEventData = new PointerEventData(_eventSystem);
                //Set the Pointer Event Position to that of the mouse position
                _pointerEventData.position = Input.touches[0].position;

                //Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();

                //Raycast using the Graphics Raycaster and mouse click position
                _raycaster.Raycast(_pointerEventData, results);

                // If any of the objects hit is part of the carousel

                return results.Any(ray => ray.gameObject.tag == "Carousel");
            }

            private bool CheckForSwipe()
            {
                //TODO: Finish
                // If the swipe is soft, its only valid if it moves enough
                int strongSwipe = CheckForStrongSwipe();
                int softSwipe = 0;

                if(strongSwipe == 0)
                {
                    softSwipe = CheckForAcumulativeSwipe();
                }

                if((strongSwipe == 1 || softSwipe == 1) && _carouselSpot > 0)
                {
                    _swipeDirection = 1;
                    return true;
                }

                if((strongSwipe == -1 || softSwipe == -1) && _carouselSpot < _carouselLength - 1)
                {
                    _swipeDirection = -1;
                    return true;
                }

                return false;
            }

            private int CheckForStrongSwipe()
            {
                float startingDelta = Input.touches[0].position.x - _startingX;

                // The first deltaPosition of Input.touches always has infinite value.
                if(startingDelta > 1.0f)
                {
                    // If theswipe is strong (large deltaPosition), its a valid swipe
                    Vector2 touchDelta = Input.touches[0].deltaPosition;

                    if (touchDelta.x > 80f)
                    {
                        // Swipe right
                        return 1;
                    }
                    else if (touchDelta.x < -80f)
                    {
                        // Swipe left
                        return -1;
                    }
                }
                
                return 0;
            }

            private int CheckForAcumulativeSwipe()
            {
                if (Mathf.Abs(Input.touches[0].position.x - _startingX) > _acumulativeThreshold)
                {
                    if (_startingX - Input.touches[0].position.x > 0)
                    {
                        return -1;
                    }

                    if (_startingX - Input.touches[0].position.x < 0)
                    {
                        return 1;
                    }
                }

                return 0;
            }
        }
    }
}


