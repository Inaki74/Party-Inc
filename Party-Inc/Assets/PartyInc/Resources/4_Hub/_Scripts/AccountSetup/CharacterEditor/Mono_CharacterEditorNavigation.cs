using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public enum Enum_CharacterEditorPages
        {
            OVERVIEW = 0,
            FACE = 1,
            LOADOUT = 2,
            WALLPAPER = 3,
            EMOTE= 4,
            TUNE = 5,
            PHOTO = 6,
            SIGNUP = 7
        }

        // Only for character creation
        public class Mono_CharacterEditorNavigation : MonoBehaviour
        {
            //      0       1       2            3          4       5        6        7
            // Overview -> Face -> Loadout -> Wallpaper -> Emote -> Tune -> Photo -> Signup
            [SerializeField] private GameObject[] _pages;
            [SerializeField] private GameObject _base;
            [SerializeField] private Mono_SignUpController _signUpController;
            [SerializeField] private GameObject _characterPreview;

            public delegate void ActionChangePage(Enum_CharacterEditorPages page);
            public static event ActionChangePage onChangePage;

            private int _currentPage = 0;

            // Start is called before the first frame update
            void Start()
            {
                
            }

            public void BtnBack()
            {
                if(_currentPage == (int)Enum_CharacterEditorPages.OVERVIEW)
                {
                    // Back to Fork Scene
                    Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
                    Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.PLAYER_FORK);
                    
                }
                else if(_currentPage == (int)Enum_CharacterEditorPages.FACE)
                {
                    _pages[_currentPage].SetActive(false);
                    _base.SetActive(false);
                    _currentPage--;
                }
                else if (_currentPage == (int)Enum_CharacterEditorPages.PHOTO)
                {
                    _base.SetActive(true);
                    _pages[_currentPage].SetActive(false);
                    _currentPage--;
                    _pages[_currentPage].SetActive(true);
                }
                else if(_currentPage == (int)Enum_CharacterEditorPages.SIGNUP)
                {
                    _characterPreview.SetActive(true);
                    _pages[_currentPage].SetActive(false);
                    _currentPage--;
                    _pages[_currentPage].SetActive(true);
                }
                else
                {
                    _pages[_currentPage].SetActive(false);
                    _currentPage--;
                    _pages[_currentPage].SetActive(true);
                }

                onChangePage?.Invoke((Enum_CharacterEditorPages)_currentPage);
            }

            public void BtnNext()
            {
                if (_currentPage == (int)Enum_CharacterEditorPages.OVERVIEW)
                {
                    _base.SetActive(true);
                    _currentPage++;
                    _pages[_currentPage].SetActive(true);
                }
                else if (_currentPage == (int)Enum_CharacterEditorPages.TUNE)
                {
                    _base.SetActive(false);
                    // Stop showing character
                    _pages[_currentPage].SetActive(false);
                    _currentPage++;
                    _pages[_currentPage].SetActive(true);
                }
                else if(_currentPage == (int)Enum_CharacterEditorPages.PHOTO)
                {
                    // Stop showing character
                    _characterPreview.SetActive(false);
                    _pages[_currentPage].SetActive(false);
                    _currentPage++;
                    _pages[_currentPage].SetActive(true);
                }
                else if (_currentPage == (int)Enum_CharacterEditorPages.SIGNUP)
                {
                    // Finish sign up
                    _signUpController.SignUp();
                }
                else
                {
                    _pages[_currentPage].SetActive(false);
                    _currentPage++;
                    _pages[_currentPage].SetActive(true);
                }

                onChangePage?.Invoke((Enum_CharacterEditorPages)_currentPage);
            }
        }
    }
}


