using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_Launcher : MonoBehaviour
        {
            private string gameVersion = "1.0";

            [SerializeField] private bool testingFrames;
            [SerializeField] private int frames;

            // Start is called before the first frame update
            void Start()
            {
                Application.targetFrameRate = 90;

                if (testingFrames)
                {
                    Application.targetFrameRate = frames;
                }
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}


