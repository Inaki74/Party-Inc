using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_VariableCarousel : MonoBehaviour
        {
            [SerializeField] private GameObject _elementToCarouselPrefab;
            [SerializeField] private int _amountOfElementsPerScrollView;

            private List<GameObject> _carouselElements = new List<GameObject>();

            private GraphicRaycaster _raycaster;
            private PointerEventData _pointerEventData;
            private EventSystem _eventSystem;

            private string[] _elements;

            private float _swipeDirection;
            private bool _swipe;

            private void Start()
            {
                //Fetch the Raycaster from the GameObject (the Canvas)
                _raycaster = GetComponent<GraphicRaycaster>();
                //Fetch the Event System from the Scene
                _eventSystem = FindObjectOfType<EventSystem>();
            }

            private void Update()
            {
                if(Input.touchCount > 0)
                {
                    CheckIfCarouselSwipe();
                }

                if (_swipe)
                {
                    // Move the carousel one place.
                }
            }

            public void InitializeCarousel(string[] elements)
            {
                _elements = elements;

                int amountOfButtons = elements.Length;
                int amountOfCarouselElements = Mathf.RoundToInt(amountOfButtons / _amountOfElementsPerScrollView);

                for(int i = 0; i < amountOfCarouselElements; i++)
                {
                    int amountNewList = 0;

                    if(amountOfButtons - i * _amountOfElementsPerScrollView >= 15)
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

                    elementToCarousel.GetComponent<Mono_AssetScrollViewHandler>().InitializeScrollview(newList);
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

            private void CheckForSwipe()
            {
                Vector2 touchDelta = Input.touches[0].deltaPosition;

                if(touchDelta.x > 15f)
                {
                    // Swipe right
                    _swipeDirection = 1f;
                    _swipe = true;
                }
                else if (touchDelta.x < 15f)
                {
                    // Swipe left
                    _swipeDirection = -1f;
                    _swipe = true;
                }
            }
        }
    }
}


