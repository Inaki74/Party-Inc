using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_RotateDeviceController : MonoBehaviour
        {
            private float _waitTime;
            private bool _runOnce;

            // Start is called before the first frame update
            void Start()
            {
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                Screen.autorotateToPortrait = false;
            }

            // Update is called once per frame
            void Update()
            {
                if(_waitTime > 3f && !_runOnce)
                {
                    _runOnce = true;
                    Screen.orientation = ScreenOrientation.Landscape;
                }

                _waitTime += Time.deltaTime;
            }
        }
    }
}