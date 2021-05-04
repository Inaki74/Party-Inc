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
            [SerializeField] private GameObject _loading;

            private int _lastClosetPage = 1;
            private int _lastStorePage = 2;

            public delegate void ActionChangePage(Enum_CharacterEditorPages page);
            public static event ActionChangePage onClosetChangePage;
            public static event ActionChangePage onStoreChangePage;

            private void Start()
            {
                if (FindObjectOfType<Mono_StoreClosetResolver>().EnteredStore)
                {
                    StartCoroutine(ActivateStorePage());
                }
                else
                {
                    _closetPages[_lastClosetPage].SetActive(true);

                    onClosetChangePage?.Invoke((Enum_CharacterEditorPages)_lastClosetPage);
                }
            }

            private void OnDisable()
            {
                
            }

            private IEnumerator ActivateStorePage()
            {
                _storePages[_lastStorePage].SetActive(true);
                _loading.SetActive(true);

                yield return new WaitUntil(() => Mng_CharacterEditorCache.Current.GetAssetStoreReady());
                _loading.SetActive(false);

                onStoreChangePage?.Invoke((Enum_CharacterEditorPages)_lastStorePage);
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


