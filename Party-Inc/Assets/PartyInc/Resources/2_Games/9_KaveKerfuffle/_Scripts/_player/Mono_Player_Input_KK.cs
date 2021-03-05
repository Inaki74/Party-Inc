using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_Player_Input_KK : MonoBehaviour
        {
            public bool JumpInput { get; set; }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                if (Application.isMobilePlatform)
                {
                    GetInputMobile();
                }
                else
                {
                    GetInputPC();
                }
            }

            private void GetInputMobile()
            {

            }

            private void GetInputPC()
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    JumpInput = true;
                }

                
            }
        }
    }
}


