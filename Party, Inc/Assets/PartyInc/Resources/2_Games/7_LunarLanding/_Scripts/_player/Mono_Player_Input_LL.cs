using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_Player_Input_LL : MonoBehaviour
        {
            private static float ScreenHeight = Screen.height / 2;

            public bool SprayUpInput { get; set; }
            public bool SprayDownInput { get; set; }

            // Update is called once per frame
            void Update()
            {
                if (Application.isMobilePlatform)
                {
                    MobileInput();
                }
                else
                {
                    PCInput();
                }
            }

            private void MobileInput()
            {
                if(Input.touchCount > 0)
                {
                    Touch t = Input.touches[0];

                    bool GetTouchUp = t.position.y > ScreenHeight;
                    bool GetTouchDown = t.position.y <= ScreenHeight;

                    if (Input.touchCount > 1)
                    {
                        Touch t1 = Input.touches[1];

                        GetTouchUp = GetTouchUp || t1.position.y > ScreenHeight;
                        GetTouchDown =  GetTouchDown || t1.position.y <= ScreenHeight;
                    }
                    
                    if (GetTouchDown)
                    {
                        SprayDownInput = true;
                    }
                    else
                    {
                        SprayDownInput = false;
                    }

                    if (GetTouchUp)
                    {
                        SprayUpInput = true;
                    }
                    else
                    {
                        SprayUpInput = false;
                    }
                }
                else
                {
                    SprayDownInput = false; SprayUpInput = false;
                }
            }

            private void PCInput()
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    SprayDownInput = true;
                }
                else
                {
                    SprayDownInput = false;
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    SprayUpInput = true;
                }
                else
                {
                    SprayUpInput = false;
                }
            }
        }
    }
}


