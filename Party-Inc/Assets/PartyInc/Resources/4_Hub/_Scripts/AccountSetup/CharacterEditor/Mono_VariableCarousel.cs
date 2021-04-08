using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using DentedPixel;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_VariableCarousel : MonoBehaviour
        {
            [SerializeField] private List<string> test = new List<string>();

            [SerializeField] private GameObject _elementToCarouselPrefab;
            [SerializeField] private int _amountOfElementsPerScrollView;

            private GraphicRaycaster _raycaster;
            private PointerEventData _pointerEventData;
            private EventSystem _eventSystem;

            private string[] _elements;

            private float _swipeDirection;
            private bool _swipe;

            private void Start()
            {
                _carouselSpot = 0;

                //Fetch the Raycaster from the GameObject (the Canvas)
                _raycaster = GetComponent<GraphicRaycaster>();
                //Fetch the Event System from the Scene
                _eventSystem = FindObjectOfType<EventSystem>();

                InitializeCarousel(test.ToArray());
            }

            private List<GameObject> _carouselElements = new List<GameObject>();

            [SerializeField] private float _carouselMovingDistance;
            [SerializeField] private ContentType _contentType;
            [SerializeField] private RectTransform _carousel;
            [SerializeField] private float _carouselSpeed;

            private enum ContentType
            {
                AssetScrollView
            }

            private float _carouselLength;
            private int _carouselSpot; // 0 to _carouselLength - 1
            private bool _carouselMoving;

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
                }
            }

            private IEnumerator CarouselSwipeCo(float swipeDirection)
            {
                // Lerp the holder in direction
                float originalPosition = _carousel.localPosition.x;
                float destination = originalPosition + _carouselMovingDistance * swipeDirection;
                Vector2 destinationVector = new Vector2(destination, _carousel.localPosition.y);
                print(Vector2.Distance(destinationVector, _carousel.localPosition));

                while(Vector2.Distance(destinationVector, _carousel.localPosition) > 0.5f)
                {
                    LeanTween.moveLocalX(_carousel.gameObject, destination, 0.5f).setEaseOutExpo();
                    yield return new WaitForEndOfFrame();
                }

                if(swipeDirection > 0)
                {
                    _carouselSpot--;
                }
                else
                {
                    _carouselSpot++;
                }

                _carouselMoving = false;
                _swipe = false;
            }

            public void InitializeCarousel(string[] elements)
            {
                _elements = elements;

                int amountOfButtons = elements.Length;
                int amountOfCarouselElements = Mathf.RoundToInt(amountOfButtons / _amountOfElementsPerScrollView);

                _carouselLength = amountOfCarouselElements;

                for(int i = 0; i < amountOfCarouselElements; i++)
                {
                    int amountNewList = 0;

                    if(amountOfButtons - i * _amountOfElementsPerScrollView >= _amountOfElementsPerScrollView)
                    {
                        amountNewList = _amountOfElementsPerScrollView;
                    }
                    else
                    {
                        amountNewList = amountOfButtons - i * _amountOfElementsPerScrollView;
                    }

                    string[] newList = new string[amountNewList];

                    for(int j = 0; j < 15; j++)
                    {
                        newList[j] = elements[i * _amountOfElementsPerScrollView + j];
                    }

                    GameObject elementToCarousel = Instantiate(_elementToCarouselPrefab, transform);
                    _carouselElements.Add(elementToCarousel);

                    if(_contentType == ContentType.AssetScrollView)
                    {
                        elementToCarousel.GetComponent<Mono_AssetScrollViewHandler>().InitializeScrollview(newList);
                    }

                    elementToCarousel.GetComponent<RectTransform>().transform.SetParent(_carousel.transform);
                    //elementToCarousel.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
                    elementToCarousel.GetComponent<RectTransform>().localPosition = new Vector2(_carouselMovingDistance * i, -437f);
                }

                // Append each of _carouselElements items to the actual carousel
            }

            private void CheckIfCarouselSwipe()
            {
                if (CheckForFingerOnCarousel())
                {
                    if (!_swipe)
                    {
                        CheckForSwipe();
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

            private bool _firstTouch;
            private float _startingX;
            private float _acumulativeX;
            private float _acumulativeThreshold = (float)Screen.width / 10f;

            private void CheckForSwipe()
            {
                //TODO: Finish
                // If the swipe is soft, its only valid if it moves enough
                bool acumulativeSwipeRight = false;
                bool acumulativeSwipeLeft = false;

                if (_firstTouch)
                {
                    _firstTouch = false;
                    _startingX = Input.touches[0].position.x;
                }

                if(Mathf.Abs(Input.touches[0].position.x - _startingX) > _acumulativeThreshold)
                {
                    if(_startingX - Input.touches[0].position.x > 0)
                    {
                        acumulativeSwipeRight = true;
                    }

                    if (_startingX - Input.touches[0].position.x < 0)
                    {
                        acumulativeSwipeRight = false;
                    }
                }

                // If theswipe is strong (large deltaPosition), its a valid swipe
                Vector2 touchDelta = Input.touches[0].deltaPosition;

                if ((touchDelta.x > 80f || acumulativeSwipeRight) && _carouselSpot > 0)
                {
                    // Swipe right
                    _swipeDirection = 1f;
                    _swipe = true;
                }
                else if ((touchDelta.x < -80f || acumulativeSwipeLeft) && _carouselSpot < _carouselLength - 1)
                {
                    // Swipe left
                    _swipeDirection = -1f;
                    _swipe = true;
                }

                if (_swipe)
                {
                    _firstTouch = true;
                    _startingX = 0f;
                }
            }
        }
    }
}


