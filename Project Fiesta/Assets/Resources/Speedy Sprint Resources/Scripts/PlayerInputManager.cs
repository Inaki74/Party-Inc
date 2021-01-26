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

            private static float _angleBounds = 45f; // The angle in which we want to accept jump/duck instead of move

            public bool JumpInput { get; private set; }
            public bool JumpInputPressed { get; private set; }
            public bool DuckInput { get; private set; }
            public bool MoveInput { get; private set; }
            public int MoveDirection { get; private set; }

            public Queue<string> currentInputs = new Queue<string>();

            // Update is called once per frame
            void Update()
            {
                if ((!photonView.IsMine && PhotonNetwork.IsConnected) || !GameManager.Current.GameBegan) return;

                if(Application.isMobilePlatform)
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
                if (Input.touchCount > 0)
                {
                    if(Input.touches[0].phase == TouchPhase.Began)
                    {
                        StopCoroutine("CoTouchManagement");
                        StartCoroutine("CoTouchManagement");
                    }
                }
                else
                {
                    MoveInput = false; JumpInput = false; DuckInput = false;
                    MoveDirection = 0;
                }
            }

            private IEnumerator CoTouchManagement()
            {
                bool foundInput = false;
                Vector3 startPoint = Input.touches[0].position;
                Vector3 endPoint = startPoint;
                while(Input.touches[0].phase != TouchPhase.Ended && Input.touchCount > 0)
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
                            //Debug.Log("JUMP");
                            currentInputs.Enqueue("Jump");
                        }
                        if (MoveInput)
                        {
                            if(MoveDirection == 1)
                            {
                                //Debug.Log("RIGHT");
                                currentInputs.Enqueue("MoveRight");
                            }
                            else
                            {
                                //Debug.Log("LEFT");
                                currentInputs.Enqueue("MoveLeft");
                            }
                        }

                        if (DuckInput)
                        {
                            //Debug.Log("DUCK");
                        }
                    }

                    yield return new WaitForEndOfFrame();
                }
            }

            private void DetermineInput(Vector3 start, Vector3 end)
            {
                Vector3 directionVector = end - start;
                float ratio = directionVector.y / directionVector.x;
                float ratioBounds = Mathf.Sin(_angleBounds * Mathf.Deg2Rad) / Mathf.Cos(_angleBounds * Mathf.Deg2Rad); // This is fucking tangent you monke

                //Debug.Log("r = " + ratioBounds + ", f = " + ratio + " from v = " + directionVector);

                // See f = 0 and f = infinity cases
                if(directionVector.x == 0f && directionVector.y > 0f)
                {
                    JumpInput = true;
                    return;
                }

                if(directionVector.x == 0f && directionVector.y < 0f) {
                    DuckInput = true;
                    return;
                }

                if(directionVector.y == 0f && directionVector.x > 0f)
                {
                    MoveInput = true;
                    MoveDirection = 1;
                    return;
                }

                if(directionVector.y == 0f && directionVector.x < 0f)
                {
                    MoveInput = true;
                    MoveDirection = -1;
                    return;
                }

                // See for other cases
                if (directionVector.x > 0f)
                {
                    if(directionVector.y > 0f)
                    {
                        if(ratio > ratioBounds)
                        {
                            JumpInput = true;
                            return;
                        }
                        else
                        {
                            MoveInput = true;
                            MoveDirection = 1;
                            return;
                        }
                    }
                    else
                    {
                        if(ratio < -ratioBounds)
                        {
                            DuckInput = true;
                            return;
                        }
                        else
                        {
                            MoveInput = true;
                            MoveDirection = 1;
                            return;
                        }
                    }
                }
                else
                {
                    if(directionVector.y > 0f)
                    {
                        if(ratio < -ratioBounds)
                        {
                            JumpInput = true;
                            return;
                        }
                        else
                        {
                            MoveInput = true;
                            MoveDirection = -1;
                            return;
                        }
                        
                    }
                    else
                    {
                        if (ratio > ratioBounds)
                        {
                            DuckInput = true;
                            return;

                        }
                        else
                        {
                            MoveInput = true;
                            MoveDirection = -1;
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


