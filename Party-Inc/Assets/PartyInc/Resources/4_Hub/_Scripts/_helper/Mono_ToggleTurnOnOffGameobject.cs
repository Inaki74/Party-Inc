using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle))]
        public class Mono_ToggleTurnOnOffGameobject : MonoBehaviour
        {
            [SerializeField] private List<GameObject> _turnOffObjects = new List<GameObject>();

            private Toggle _theToggle;

            private void Awake()
            {
                _theToggle = GetComponent<Toggle>();
            }

            public void ToggleChanged(bool isOn)
            {
                if (_theToggle == null)
                {
                    _theToggle = GetComponent<Toggle>();
                }

                if (_theToggle.isOn)
                {
                    SetActiveList(true);
                }
                else
                {
                    SetActiveList(false);
                }
            }

            private void SetActiveList(bool b)
            {
                foreach(GameObject go in _turnOffObjects)
                {
                    go.SetActive(b);
                }
            }
        }
    }
}


