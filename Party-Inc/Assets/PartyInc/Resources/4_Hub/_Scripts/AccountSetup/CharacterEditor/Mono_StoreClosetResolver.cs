using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_StoreClosetResolver : MonoBehaviour
        {
            public const string SCENE_PROPS_KEY = "StoreClosetResolverInformation";

            [SerializeField] private GameObject _closetEditor;
            [SerializeField] private GameObject _storeEditor;

            public bool EnteredStore { get; set; }

            // Start is called before the first frame update
            void Awake()
            {
                if (!Mng_SceneProps.Current.SceneProps.ContainsKey(SCENE_PROPS_KEY)) return;

                print((string)Convert.ChangeType(Mng_SceneProps.Current.SceneProps[SCENE_PROPS_KEY], typeof(string)));

                if (((string)Convert.ChangeType(Mng_SceneProps.Current.SceneProps[SCENE_PROPS_KEY], typeof(string))).Equals("store"))
                {
                    _storeEditor.SetActive(true);
                    EnteredStore = true;
                }
                else
                {
                    _closetEditor.SetActive(true);
                    EnteredStore = false;
                }
            }
        }
    }
}


