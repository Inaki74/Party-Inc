using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CharacterStoreClosetNavigation : MonoBehaviour
        {
            [SerializeField] private GameObject[] _closetPages;
            [SerializeField] private GameObject[] _storePages;
            [SerializeField] private GameObject _base;

            private int _lastClosetPage = 1;
            private int _lastStorePage = 1;

            public delegate void ActionChangePage(Enum_CharacterEditorPages page);
            public static event ActionChangePage onClosetChangePage;
            public static event ActionChangePage onStoreChangePage;

            // Start is called before the first frame update
            private void OnEnable()
            {
                // TODO: Need a way to determine if the user entered to the store first or to the closet first

                _closetPages[_lastClosetPage].SetActive(true);

                onClosetChangePage?.Invoke((Enum_CharacterEditorPages)_lastClosetPage);
            }

            private void OnDisable()
            {
                
            }

            public void OnStorePageToggle(int pageType)
            {
                _storePages[_lastStorePage].SetActive(false);
                _lastStorePage = pageType;
                _storePages[pageType].SetActive(true);

                onStoreChangePage?.Invoke((Enum_CharacterEditorPages)pageType);
            }

            public void OnClosetPageToggle(int pageType)
            {
                _closetPages[_lastClosetPage].SetActive(false);
                _lastClosetPage = pageType;
                _closetPages[pageType].SetActive(true);

                onClosetChangePage?.Invoke((Enum_CharacterEditorPages) pageType);
            }
        }
    }
}


