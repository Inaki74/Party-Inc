using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle))]
        public class Mono_AssetButtonPlayableHandler : MonoBehaviour
        {
            private Toggle _theToggle;

            //DATA

            private void Awake()
            {
                _theToggle = GetComponent<Toggle>();
            }

            public void InitializeButton(ToggleGroup theTG)
            {
                _theToggle.group = theTG;
            }
        }
    }
}