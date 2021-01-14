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
            [SerializeField] private float _inputAcceptanceLength;

            public bool JumpInput { get; private set; }
            public bool JumpInputPressed { get; private set; }
            public bool DuckInput { get; private set; }
            public bool MoveInput { get; private set; }
            public int MoveDirection { get; private set; }

            public Queue<string> currentInputs = new Queue<string>();

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    TakeInputMobile();
                }
                else
                {
                    TakeInputPC();
                }
            }

            private void TakeInputMobile()
            {
                MoveInput = false; JumpInput = false; DuckInput = false;
                MoveDirection = 0;

                if (Input.touchCount > 0)
                {
                    if(Input.touches[0].phase == TouchPhase.Began)
                    {
                        StopCoroutine("CoTouchManagement");
                        StartCoroutine("CoTouchManagement");
                    }
                }
            }

            private IEnumerator CoTouchManagement()
            {
                bool foundInput = false;
                Vector3 startPoint = Input.touches[0].position;
                Vector3 endPoint = startPoint;
                while(Input.touches[0].phase != TouchPhase.Ended)
                {
                    MoveInput = false; JumpInput = false; DuckInput = false;
                    MoveDirection = 0;

                    endPoint = Input.touches[0].position;

                    if(Vector3.Distance(startPoint, endPoint) > _inputAcceptanceLength && !foundInput)
                    {
                        foundInput = true;
                        DetermineInput(startPoint, endPoint);

                        if (JumpInput)
                        {
                            currentInputs.Enqueue("Jump");
                        }
                        if (MoveInput)
                        {
                            if(MoveDirection == 1)
                            {
                                currentInputs.Enqueue("MoveRight");
                            }
                            else
                            {
                                currentInputs.Enqueue("MoveLeft");
                            }
                        }
                    }

                    
                    yield return new WaitForEndOfFrame();
                }
            }

            private void DetermineInput(Vector3 start, Vector3 end)
            {
                Vector3 directionVector = end - start;
                float ratio = directionVector.x / directionVector.y;

                if (directionVector.x > 0f)
                {
                    if(directionVector.y > 0f)
                    {
                        if (ratio <= 0.577f)
                        {
                            JumpInput = true;
                            return;
                        }

                        if (ratio > 0.577f)
                        {
                            MoveInput = true;
                            MoveDirection = 1;
                            return;
                        }
                    }
                    else
                    {
                        if (ratio <= -0.557f)
                        {
                            MoveInput = true;
                            MoveDirection = 1;
                            return;
                        }

                        if (ratio > -0.557f)
                        {
                            DuckInput = true;
                            return;
                        }
                    }
                }
                else
                {
                    if(directionVector.y > 0f)
                    {
                        if (ratio > -0.577f)
                        {
                            JumpInput = true;
                            return;
                        }

                        if (ratio <= -0.577f)
                        {
                            MoveInput = true;
                            MoveDirection = -1;
                            return;
                        }
                    }
                    else
                    {
                        if (ratio >= 0.557f)
                        {
                            MoveInput = true;
                            MoveDirection = -1;
                            return;
                        }

                        if (ratio < 0.557f)
                        {
                            DuckInput = true;
                            return;
                        }
                    }
                }
            }

            private void TakeInputPC()
            {
                MoveInput = false; JumpInput = false; DuckInput = false;
                MoveDirection = 0;

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

                DuckInput = Input.GetKeyDown(KeyCode.DownArrow);

                if (JumpInput)
                {
                    currentInputs.Enqueue("Jump");
                }
                if (MoveInput)
                {
                    if (MoveDirection == 1)
                    {
                        currentInputs.Enqueue("MoveRight");
                    }
                    else
                    {
                        currentInputs.Enqueue("MoveLeft");
                    }
                }
            }
        }
    }
}


