using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_Player_Input_KK : MonoBehaviourPun
        {
            private static float ScreenWidth = Screen.width / 2;

            public bool SprayUpInput { get; set; }
            public bool SprayDownInput { get; set; }

            // Update is called once per frame
            void Update()
            {
                if (!Mng_GameManager_KK.Current.GameBegan || (PhotonNetwork.IsConnected && !photonView.IsMine)) return;

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
                if (Input.touchCount > 0)
                {
                    Touch t = Input.touches[0];

                    bool GetTouchUp = t.position.x > ScreenWidth;
                    bool GetTouchDown = t.position.x <= ScreenWidth;

                    Debug.Log("Touch information: " + t.position.x);
                    Debug.Log("Screen Width: " + ScreenWidth);

                    if (Input.touchCount > 1)
                    {
                        Touch t1 = Input.touches[1];

                        GetTouchUp = GetTouchUp || t1.position.x > ScreenWidth;
                        GetTouchDown = GetTouchDown || t1.position.x <= ScreenWidth;
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
