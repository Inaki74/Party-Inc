using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        
        // 6 pages
        public class Mono_CharacterEditorController : MonoBehaviour
        {
            //      0       1       2           3           4       5        6        7
            // Overview -> Face -> Outfit -> Wallpaper -> Emote -> Tune -> Photo -> Signup
            [SerializeField] private GameObject[] _pages;
            [SerializeField] private GameObject _base;
            [SerializeField] private GameObject _variableCarousel;
            private int _currentPage = 0;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void BtnBack()
            {
                if(_currentPage == 0)
                {
                    // Back to Fork Scene
                }
                else if(_currentPage == 1)
                {
                    _pages[_currentPage].SetActive(false);
                    _base.SetActive(false);
                    _variableCarousel.SetActive(false);
                    _currentPage--;
                }
                else
                {
                    _pages[_currentPage].SetActive(false);
                    _currentPage--;
                    _pages[_currentPage].SetActive(true);
                }
            }

            public void BtnNext()
            {
                if(_currentPage == 7)
                {
                    // Finish sign up

                }
                else if (_currentPage == 0)
                {
                    _base.SetActive(true);
                    _variableCarousel.SetActive(true);
                    _currentPage++;
                    _pages[_currentPage].SetActive(true);
                }
                else
                {
                    _pages[_currentPage].SetActive(false);
                    _currentPage++;
                    _pages[_currentPage].SetActive(true);
                }
            }
        }
    }
}


