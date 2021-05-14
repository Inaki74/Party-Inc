using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_StoreClosetResolver : MonoBehaviour
        {
            public const string SCENE_PROPS_KEY = "StoreClosetResolverInformation";

            public bool OnStore { get; private set; }

            public delegate void ClosetStoreSwitch(bool toStore);
            public static event ClosetStoreSwitch onClosetStoreSwitch;

            [SerializeField] private Toggle[] _storeClosetToggles = new Toggle[2]; // 0 -> closet, 1 -> store
            [SerializeField] private GameObject _closetEditor;
            [SerializeField] private GameObject _storeEditor;

            private bool _firstToggle = true;

            public bool EnteredStore { get; set; }

            // Start is called before the first frame update
            void Awake()
            {
                if (!Mng_SceneProps.Current.SceneProps.ContainsKey(SCENE_PROPS_KEY)) return;

                if (((string)Convert.ChangeType(Mng_SceneProps.Current.SceneProps[SCENE_PROPS_KEY], typeof(string))).Equals("store"))
                {
                    _storeClosetToggles[1].isOn = true;

                    //_storeEditor.SetActive(true);
                    //OnStore = true;

                    EnteredStore = true;
                }
                else
                {
                    _storeClosetToggles[0].isOn = false;

                    //_closetEditor.SetActive(true);
                    //OnStore = false;

                    EnteredStore = false;
                }
            }

            public void SwitchStoreCloset(int storeOrCloset)
            {
                // Reset the chosen assets
                // Chosen assets is shared between store and closet.
                // Reset the variable carousel as well (OnPageChange).

                if (!_storeClosetToggles[storeOrCloset].isOn || _firstToggle)
                {
                    _firstToggle = false;
                    return;
                }

                if(storeOrCloset == 1)
                {
                    _storeEditor.SetActive(true);
                    _closetEditor.SetActive(false);
                    OnStore = true;
                }
                else
                {
                    _closetEditor.SetActive(true);
                    _storeEditor.SetActive(false);
                    OnStore = false;
                }

                StartCoroutine("WaitAFrameCo");
                
            }

            private IEnumerator WaitAFrameCo()
            {
                yield return new WaitForEndOfFrame();
                onClosetStoreSwitch?.Invoke(OnStore);
            }
        }
    }
}


