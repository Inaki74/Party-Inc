using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace FiestaTime
{
    namespace TT
    {
        [RequireComponent(typeof(Rigidbody))]
        public class PlayerController : MonoBehaviourPun, IPunObservable
        {
            [SerializeField] private Rigidbody Rb;
            [SerializeField] private MeshRenderer Mr;

            public delegate void ActionCrossFinish(int playerId);
            public static event ActionCrossFinish onCrossFinishLine;

            public delegate void ActionPlayerLost(int playerId);
            public static event ActionPlayerLost onPlayerDied;

            private bool crossedLine = false;

            private float originalDrag;
            [SerializeField] private float punishmentDrag;

            public Material mine;

            // Sync variables
            private Vector3 lastPos;
            private Vector3 lastVel;

            // Boolean that is true in the frame that you tapped.
            private bool tap;
            private bool bufferedTap;
            private bool infoReceived;

            //UI elements that reflect input variables (to be removed)
            [SerializeField] private RectTransform middleLine;
            [SerializeField] private RectTransform toleranceLineOneMin;
            [SerializeField] private RectTransform toleranceLineOneMax;
            [SerializeField] private RectTransform toleranceLineTwoMin;
            [SerializeField] private RectTransform toleranceLineTwoMax;
            [SerializeField] private Text leftRight;

            // Input Variables
            private int tapCount = 0;
            private float firstX = -1f;
            private float secondX = -1f;
            private float middleX;
            private float toleranceRightMin;
            private float toleranceRightMax = 1000000;
            private float toleranceLeftMin;
            private float toleranceLeftMax = -1000000;
            private static float tolerance = Screen.width/4f;
            private int lastInput; //1 is right, -1 is left, 0 is base case

            [SerializeField] private float tapForce;

            [SerializeField] private float minimumTimeIdle;
            private float timeIdle;

            // Start is called before the first frame update
            void Start()
            {
                Debug.Log("PlayerController start");
                if (Rb == null) Rb = GetComponent<Rigidbody>();
                if (Mr == null) Mr = GetComponent<MeshRenderer>();

                originalDrag = Rb.drag;

                if (photonView.IsMine) Mr.material = mine;
                else leftRight.enabled = false;
            }

            // Update is called once per frame
            void Update()
            {
                if(!photonView.IsMine && PhotonNetwork.IsConnected && infoReceived)
                {
                    transform.position = Vector3.Lerp(transform.position, lastPos, Time.deltaTime);
                    Rb.velocity = lastVel;
                    return;
                }

                // Make the players move a bit more forward and stop
                if (crossedLine) return;

                if (GameManager.Current.gameBegan)
                {
                    CheckForInput(false);
                    CheckForInputPC();

                    if(timeIdle > minimumTimeIdle)
                    {
                        Rb.drag = punishmentDrag;
                    }
                    else
                    {
                        Rb.drag = originalDrag;
                    }
                }

                timeIdle += Time.deltaTime;
            }

            private void LateUpdate()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                // Make the players move a bit more forward and stop
                if (crossedLine) return;

                if (GameManager.Current.gameBegan) { CheckForInput(true); CheckForInputPC(); }
            }

            private void FixedUpdate()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                // Make the players move a bit more forward and stop
                if (crossedLine) return;

                if (tap || bufferedTap)
                {
                    MoveForward();

                    if (tap && bufferedTap)
                    {
                        tap = false;
                    }else if (bufferedTap)
                    {
                        bufferedTap = false;
                    }
                    else
                    {
                        tap = false;
                    }
                }
            }

            private void OnDisable()
            {
                Rb.velocity = Vector3.zero;
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                if (collision.gameObject.tag == "DeathPlane")
                {
                    gameObject.SetActive(false);
                    onPlayerDied?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);
                    photonView.RPC("RPC_Disable", RpcTarget.All);
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                if(other.tag == "FinishPlane")
                {
                    // You win xd
                    crossedLine = true;
                    onCrossFinishLine?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);
                }
            }

            private void MoveForward()
            {
                Rb.AddForce(tapForce * GameManager.forwardVector, ForceMode.Impulse);
            }

            private void CheckForInput(bool late)
            {
                if(Input.touchCount > 0)
                {
                    SetupInputCenter(late);
                    
                    if (tap) bufferedTap = GetInput();
                    else tap = GetInput();
                }
            }

            private void SetupInputCenter(bool late)
            {
                if (!late)
                {
                    if (Input.touches[0].phase == TouchPhase.Began) tapCount++;

                    if (tapCount == 1)
                    {
                        firstX = Input.touches[0].position.x;
                    }
                    else if (tapCount == 2)
                    {
                        secondX = Input.touches[0].position.x;
                        middleX = (firstX + secondX) / 2;

                        if(firstX < secondX)
                        {
                            toleranceLeftMax = firstX - tolerance;
                            toleranceLeftMin = firstX + tolerance;
                            toleranceRightMax = secondX + tolerance;
                            toleranceRightMin = secondX - tolerance;
                        }
                        else
                        {
                            toleranceRightMax = firstX + tolerance;
                            toleranceRightMin = firstX - tolerance;
                            toleranceLeftMax = secondX - tolerance;
                            toleranceLeftMin = secondX + tolerance;
                        }

                        if(toleranceLeftMax <= 6)
                        {
                            toleranceLeftMax = 12;
                        }

                        if(toleranceRightMax >= Screen.width - 6)
                        {
                            toleranceRightMax = Screen.width - 12;
                        }

                        if(toleranceRightMin < middleX)
                        {
                            toleranceRightMin = -20000;
                        }

                        if(toleranceLeftMin > middleX)
                        {
                            toleranceLeftMin = 20000;
                        }

                        SetUILines();
                    }
                }
            }

            private bool GetInput()
            {
                bool ret = false;

                if(Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Stationary || Input.touches[0].phase == TouchPhase.Moved)
                {
                    Debug.Log(Input.touches[0].position.x < middleX);

                    if (lastInput == 0)
                    {
                        //Case in which input was never given.
                        //When setting up
                        ret = true;
                        if(firstX > Screen.width / 2)
                        {
                            lastInput = 1;
                        }
                        else
                        {
                            lastInput = -1;
                        }
                    }
                    else if(lastInput == 1 && Input.touches[0].position.x < middleX)
                    {
                        //Last was right, now it needs to do left to advance
                        ret = true;
                        lastInput = -1;
                        leftRight.text = "left";
                    }
                    else if(lastInput == -1 && Input.touches[0].position.x > middleX)
                    {
                        //Last was left, now it needs to do right to advance.
                        ret = true;
                        lastInput = 1;
                        leftRight.text = "right";
                    }

                    if((lastInput == -1 && (Input.touches[0].position.x < toleranceLeftMax || Input.touches[0].position.x > toleranceLeftMin)) ||
                       (lastInput == 1 && (Input.touches[0].position.x > toleranceRightMax || Input.touches[0].position.x < toleranceRightMin)))
                    {
                        //The player tapped outside of the tolerance mark.
                        //We need to redefine the input center
                        tapCount = 1;
                        firstX = Input.touches[0].position.x;
                    }
                }

                if (ret) timeIdle = 0f;

                return ret;
            }

            private void CheckForInputPC()
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (tap) bufferedTap = true;
                    else tap = true;

                    timeIdle = 0f;
                }
            }

            private void SetUILines()
            {
                // Placing middle line.
                Vector2 offsetMidMin = new Vector2(middleX, -4088);
                Vector2 offsetMidMax = new Vector2(middleX + 6 - Screen.width, 4472);
                middleLine.offsetMin = offsetMidMin;
                middleLine.offsetMax = offsetMidMax;

                // Placing max tolerance line 1 MAX.
                Vector2 offsetTol1Min = new Vector2(toleranceLeftMax, -4088);
                Vector2 offsetTol1Max = new Vector2(toleranceLeftMax + 6 - Screen.width, 4472);
                toleranceLineOneMax.offsetMin = offsetTol1Min;
                toleranceLineOneMax.offsetMax = offsetTol1Max;

                // Placing max tolerance line 1 MIN.
                Vector2 offsetTol11Min = new Vector2(toleranceLeftMin, -4088);
                Vector2 offsetTol11Max = new Vector2(toleranceLeftMin + 6 - Screen.width, 4472);
                toleranceLineOneMin.offsetMin = offsetTol11Min;
                toleranceLineOneMin.offsetMax = offsetTol11Max;

                // Placing max tolerance line 2 MAX.
                Vector2 offsetTol2Min = new Vector2(toleranceRightMax, -4088);
                Vector2 offsetTol2Max = new Vector2(toleranceRightMax + 6 - Screen.width, 4472);
                toleranceLineTwoMax.offsetMin = offsetTol2Min;
                toleranceLineTwoMax.offsetMax = offsetTol2Max;

                // Placing max tolerance line 2 MIN.
                Vector2 offsetTol22Min = new Vector2(toleranceRightMin, -4088);
                Vector2 offsetTol22Max = new Vector2(toleranceRightMin + 6 - Screen.width, 4472);
                toleranceLineTwoMin.offsetMin = offsetTol22Min;
                toleranceLineTwoMin.offsetMax = offsetTol22Max;

                middleLine.gameObject.SetActive(true);
                toleranceLineOneMax.gameObject.SetActive(true);
                toleranceLineTwoMax.gameObject.SetActive(true);
                toleranceLineOneMin.gameObject.SetActive(true);
                toleranceLineTwoMin.gameObject.SetActive(true);
            }

            public Rigidbody GetRigidbody()
            {
                return Rb;
            }

            public Transform GetTransform()
            {
                return gameObject.transform;
            }

            [PunRPC]
            public void RPC_Disable()
            {
                gameObject.SetActive(false);
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(transform.position);
                    stream.SendNext(Rb.velocity);
                }
                else
                {
                    lastPos = (Vector3)stream.ReceiveNext();
                    lastVel = (Vector3)stream.ReceiveNext();

                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    lastPos += lastVel * lag;

                    infoReceived = true;
                }
            }
        }
    }
}