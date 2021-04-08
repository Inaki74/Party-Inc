using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_RescaleUIToAspectRatio : MonoBehaviour
        {
            [SerializeField] private GameObject _objectToScale;

            // Start is called before the first frame update
            void Start()
            {
                float originalAspect = 1125f / 2436f;
                float currentAspect = (float)Screen.width / (float)Screen.height;

                _objectToScale.transform.localScale = Hub.Mono_Rescaler.RescaleValueToAspectRatio(originalAspect, _objectToScale.transform.localScale);

                _objectToScale.transform.position = Hub.Mono_Rescaler.RescaleValueToAspectRatio(originalAspect, _objectToScale.transform.position);
            }
        }
    }
}


