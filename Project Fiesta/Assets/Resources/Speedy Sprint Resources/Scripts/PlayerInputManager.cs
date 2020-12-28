using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        public class PlayerInputManager : MonoBehaviourPun
        {
            public bool JumpInput { get; private set; }
            public bool JumpInputPressed { get; private set; }
            public bool DuckInput { get; private set; }
            public bool MoveInput { get; private set; }
            public float MoveDirection { get; private set; }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                //TakeInputPC();

                TakeInputMobile();
            }

            private void TakeInputMobile()
            {
                if (Input.touchCount > 0)
                {
                    JumpInput = Input.touches[0].deltaPosition.y > 50f && Input.touches[0].phase != TouchPhase.Ended;

                    DuckInput = Input.touches[0].deltaPosition.y < -100f && Input.touches[0].phase != TouchPhase.Ended;

                    MoveInput = (Input.touches[0].deltaPosition.x > 100f || Input.touches[0].deltaPosition.x < -100f) && Input.touches[0].phase != TouchPhase.Ended;

                    if (MoveInput)
                    {
                        MoveDirection = Input.touches[0].deltaPosition.x;
                    }

                    Input.touches[0].deltaPosition = Vector2.zero;
                }
            }

            private void TakeInputPC()
            {
                MoveInput = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveDirection = -1;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveDirection = 1;
                }

                JumpInput = Input.GetKeyDown(KeyCode.UpArrow);

                JumpInput = Input.GetKey(KeyCode.UpArrow);

                DuckInput = Input.GetKeyDown(KeyCode.DownArrow);
            }
        }
    }
}


