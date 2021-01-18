using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        /// <summary>
        /// State machine would be overkill, since theres only 4 states.
        /// </summary>
        [RequireComponent(typeof(Rigidbody))]
        public class Player : MonoBehaviourPun, IPunObservable
        {
            private Rigidbody _rb;
            private CapsuleCollider _cc;
            private PlayerInputManager _inputManager;
            [SerializeField] private MeshRenderer _ubmr;
            [SerializeField] private MeshRenderer _lbmr;
            [SerializeField] private Material mine;
            
            [SerializeField] private GameObject _deathParticles;

            [SerializeField] private GameObject _upperBody;
            [SerializeField] private GameObject _lowerBody;

            public delegate void ActionPlayerLost(int playerId);
            public static event ActionPlayerLost onPlayerDied;

            private bool _hasLost;

            [SerializeField] private float _jumpHeight;
            [SerializeField] private float _jumpSpeed;
            [SerializeField] private float _timeFloating = 0.1444639f;

            private float _timeFloatingRatio = 1.7f;

            [SerializeField] GameObject _head;
            [SerializeField] GameObject _body;
            [SerializeField] GameObject _feet;
            private bool _isGrounded;
            private bool _gravity = true;
            private enum PlayerStates
            {
                Airborne,
                Grounded,
                Ducking,
                Dropping
            };
            private PlayerStates _currentState;

            // Duck variables
            [SerializeField] private float _duckDuration;
            private float _currentDuckDuration;
            [SerializeField] private float _duckCooldown;
            private float _currentDuckCooldown;
            [SerializeField] private float _droppingForce;

            // Movement variables
            private float[] _movementBuffer = new float[2]; // Movement buffer meant for stacking a single movement if it was played while moving
            private int _moves = 0;
            private bool _runOnce;
            [SerializeField] private float _laneSwitchSpeed;
            private float _leftRailX;
            private float _middleRailX;
            private float _rightRailX;

            [SerializeField] private LayerMask _whatIsGround;
            [SerializeField] private float _distanceGround;

            // Network variables
            private Queue<string> inputsToDo = new Queue<string>();

            private bool _infoReceived;
            private Vector3 _netPos;
            
            private Vector3 _lastPos;

            private bool _netDuck;

            private float _time;
            private double _lastSendTime;
            private double _currentSendTime;
            [SerializeField] private float _networkTeleportDistance;

            // Start is called before the first frame update
            void Start()
            {
                if(_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }
                if (_cc == null)
                {
                    _cc = GetComponent<CapsuleCollider>();
                }
                if (_inputManager == null)
                {
                    _inputManager = GetComponent<PlayerInputManager>();
                }

                _currentDuckDuration = _duckDuration + 1;

                _runOnce = true;
                _middleRailX = transform.position.x;
                _leftRailX = _middleRailX - 3;
                _rightRailX = _middleRailX + 3;

                _currentState = PlayerStates.Grounded;

                if (photonView.IsMine)
                {
                    _ubmr.material = mine;
                    _lbmr.material = mine;
                }
            }

            // Update is called once per frame
            void Update()
            {
                _isGrounded = CheckIfGrounded(transform.position);

                if (!photonView.IsMine && PhotonNetwork.IsConnected && _infoReceived)
                {
                    double timeToSend = _currentSendTime - _lastSendTime;

                    _time += Time.deltaTime;

                    PositionPrediction(timeToSend);

                    _upperBody.SetActive(_netDuck);

                    return;
                }

                _jumpSpeed = GameManager.Current.Gravity * -1 / 1.25f;

                _timeFloating = _timeFloatingRatio / GameManager.Current.MovingSpeed;

                StateLogic();

                transform.position += Vector3.forward * GameManager.Current.MovingSpeed * Time.deltaTime;

                if(_gravity && _currentState == PlayerStates.Airborne && !_isGrounded)
                {
                    SimulateGravity();
                }

                if (_inputManager.JumpInput && (_currentState == PlayerStates.Grounded || _currentState == PlayerStates.Ducking))
                {
                    if(_currentState == PlayerStates.Ducking)
                    {
                        UnDuck();
                    }
                    _currentState = PlayerStates.Airborne;
                    StartCoroutine(JumpCo(transform.position.y, _jumpHeight, _jumpSpeed));
                }

                if (_currentState == PlayerStates.Dropping && !_isGrounded)
                {
                    transform.position += Vector3.up * -_droppingForce * Time.deltaTime;
                }

                if (_inputManager.MoveInput)
                {
                    if(_moves <= 1 && _movementBuffer[_moves] == 0f)
                    {
                        _movementBuffer[_moves] = _inputManager.MoveDirection;
                        _moves++;
                    }

                    if (_runOnce)
                    {
                        _runOnce = false;
                        StartCoroutine(MoveToCo(_laneSwitchSpeed));
                    }
                }

                CheckIfDead(_feet.transform, Vector3.forward);
                CheckIfDead(_feet.transform, Vector3.back);
                CheckIfDead(_feet.transform, Vector3.left);
                CheckIfDead(_feet.transform, Vector3.right);

                CheckIfDead(_body.transform, Vector3.forward);
                CheckIfDead(_body.transform, Vector3.back);
                CheckIfDead(_body.transform, Vector3.left);
                CheckIfDead(_body.transform, Vector3.right);

                if (_upperBody.activeInHierarchy)
                {
                    CheckIfDead(_head.transform, Vector3.forward);
                    CheckIfDead(_head.transform, Vector3.back);
                    CheckIfDead(_head.transform, Vector3.left);
                    CheckIfDead(_head.transform, Vector3.right);
                }
            }

            private void SimulateGravity()
            {
                transform.position += Vector3.up * GameManager.Current.Gravity * Time.deltaTime;
            }

            private IEnumerator MoveToCo(float velocity)
            {
                for (int i = 0; i < _movementBuffer.Length; i++)
                {
                    if(_movementBuffer[i] != 0)
                    {
                        float decidedX = _middleRailX;
                        float currentX = transform.position.x;

                        if (currentX == _leftRailX)
                        {
                            if (_movementBuffer[i] < 0f)
                            {
                                decidedX = _leftRailX;
                            }
                        }

                        if (currentX == _middleRailX)
                        {
                            if (_movementBuffer[i] > 0f)
                            {
                                decidedX = _rightRailX;
                            }

                            if (_movementBuffer[i] < 0f)
                            {
                                decidedX = _leftRailX;
                            }
                        }

                        if (currentX == _rightRailX)
                        {
                            if (_movementBuffer[i] > 0f)
                            {
                                decidedX = _rightRailX;
                            }
                        }

                        while (transform.position.x != decidedX)
                        {
                            //Debug.Log("A1");
                            Vector3 decidedVector = new Vector3(decidedX, transform.position.y, transform.position.z);
                            transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                _movementBuffer[0] = 0f;
                _movementBuffer[1] = 0f;
                _moves = 0;

                _runOnce = true;
            }

            private IEnumerator MoveToNetCo(float velocity, int direction)
            {
                if (direction != 0)
                {
                    float decidedX = _middleRailX;
                    float currentX = transform.position.x;

                    if (currentX == _leftRailX)
                    {
                        if (direction < 0f)
                        {
                            decidedX = _leftRailX;
                        }
                    }

                    if (currentX == _middleRailX)
                    {
                        if (direction > 0f)
                        {
                            decidedX = _rightRailX;
                        }

                        if (direction < 0f)
                        {
                            decidedX = _leftRailX;
                        }
                    }

                    if (currentX == _rightRailX)
                    {
                        if (direction > 0f)
                        {
                            decidedX = _rightRailX;
                        }
                    }

                    while (transform.position.x != decidedX)
                    {
                        //Debug.Log("A1");
                        Vector3 decidedVector = new Vector3(decidedX, transform.position.y, transform.position.z);
                        transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                }

                _runOnce = true;
            }


            private void StateLogic()
            {
                if (_inputManager.DuckInput)
                {
                    if (_currentState == PlayerStates.Grounded && _currentDuckCooldown < 0f)
                    {
                        Duck();
                    }

                    if (_currentState == PlayerStates.Airborne)
                    {
                        _currentState = PlayerStates.Dropping;
                    }
                }

                if (_isGrounded)
                {
                    // If ducked in the air, it must fall quick. Once it reaches the ground, duck.
                    if (_currentState == PlayerStates.Dropping)
                    {
                        Duck();
                    }

                    // If grounded, set state to grounded. If came from ducking, unduck.
                    if (_currentDuckDuration > _duckDuration)
                    {
                        if(_currentState == PlayerStates.Ducking)
                        {
                            UnDuck();
                        }
                        _currentState = PlayerStates.Grounded;
                    }
                }
                else
                {
                    // If its not grounded its in the air.
                    if (_currentState != PlayerStates.Dropping && _currentState != PlayerStates.Ducking)
                    {
                        _currentState = PlayerStates.Airborne;
                    }
                }

                if (_currentState == PlayerStates.Ducking)
                {
                    _currentDuckDuration += Time.deltaTime;
                }
                else
                {
                    _currentDuckCooldown -= Time.deltaTime;
                }
            }

            private IEnumerator JumpCo(float startingY, float height, float velocity)
            {
                _gravity = false;
                bool reachedHeight = false;

                while (!reachedHeight && !_inputManager.DuckInput){

                    Vector3 decidedVector = new Vector3(transform.position.x, startingY + height, transform.position.z);
                    //Debug.Log("A3");
                    transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                    reachedHeight = transform.position.y > startingY + height - 0.2;
                    yield return new WaitForEndOfFrame();
                }

                float timeFloating = _timeFloating;

                while (reachedHeight || timeFloating > 0f)
                {
                    reachedHeight = false;
                    timeFloating -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                };
                _gravity = true;
            }

            private void Duck()
            {
                _currentState = PlayerStates.Ducking;

                _currentDuckDuration = 0.00001f;
                _currentDuckCooldown = _duckCooldown;

                _cc.center = new Vector3(0f, -0.5f, 0f);
                _cc.height = 1;

                _upperBody.SetActive(false);
            }

            private void UnDuck()
            {
                _currentDuckDuration = _duckDuration + 0.00001f;
                _cc.center = new Vector3(0f, 0f, 0f);
                _cc.height = 2;
                _upperBody.SetActive(true);
            }

            private bool CheckIfGrounded(Vector3 startPosition)
            {
                RaycastHit hit;

                if (Physics.Raycast(startPosition, transform.TransformDirection(Vector3.down), out hit, _distanceGround, _whatIsGround))
                {
                    Debug.DrawRay(startPosition, transform.TransformDirection(Vector3.down) * hit.distance, Color.red, 0f);
                    return true;
                }
                else
                {
                    Debug.DrawRay(startPosition, transform.TransformDirection(Vector3.down) * _distanceGround, Color.green, 0f);
                    return false;
                }
            }

            private void CheckIfDead(Transform trans, Vector3 direction)
            {
                if (_hasLost) return;

                RaycastHit hit;

                if (Physics.Raycast(trans.position, trans.TransformDirection(direction), out hit, 5f, 1 << 8))
                {
                    Debug.DrawRay(trans.position, trans.TransformDirection(direction) * hit.distance, Color.green, 0f);
                    if (hit.distance < 0.6f)
                    {
                        Debug.Log("Dead by CheckIfDead");
                        PlayerDie();
                    }
                }else Debug.DrawRay(trans.position, trans.TransformDirection(direction) * 5f, Color.red, 0f);
            }

            private void PlayerDie()
            {
                _hasLost = true;
                onPlayerDied?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("RPC_InformPlayerLost", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("RPC_PlayerDied", RpcTarget.Others);
                Instantiate(_deathParticles, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }

            private void OnTriggerEnter(Collider other)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                if (other.gameObject.tag == "Obstacle")
                {
                    PlayerDie();
                }
            }

            private void PositionPrediction(double timeToSend)
            {
                transform.position = Vector3.Lerp(_lastPos, _netPos, (float)(_time / timeToSend));

                StartCoroutine("RecreateInputsCo");
            }

            private IEnumerator RecreateInputsCo()
            {
                while(inputsToDo.Count != 0)
                {
                    string input = inputsToDo.Dequeue();

                    if(input == "MoveRight")
                    {
                        yield return StartCoroutine(MoveToNetCo(_laneSwitchSpeed, 1));
                    }
                    if(input == "MoveLeft")
                    {
                        yield return StartCoroutine(MoveToNetCo(_laneSwitchSpeed, -1));
                    }
                    if(input == "Jump")
                    {
                        while (!_isGrounded)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        yield return StartCoroutine(JumpCo(transform.position.y, _jumpHeight, _jumpSpeed));
                    }
                }
            }

            [PunRPC]
            public void RPC_PlayerDied()
            {
                //Debug.Log("DUDE DIED");
                Instantiate(_deathParticles, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }

            [PunRPC]
            public void RPC_InformPlayerLost(int playerId)
            {
                _hasLost = true;
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (!GameManager.Current.PlayersConnectedAndReady || _inputManager == null) return;

                if (stream.IsWriting)
                {
                    SendInputs(stream, _inputManager.currentInputs);

                    stream.SendNext(transform.position);
                    stream.SendNext(_upperBody.activeInHierarchy);
                }
                else
                {
                    ReceiveInputs(stream);

                    _netPos = (Vector3)stream.ReceiveNext();
                    _netDuck = (bool)stream.ReceiveNext();

                    _time = 0f;
                    _lastSendTime = _currentSendTime;
                    _currentSendTime = info.SentServerTime;
                    _lastPos = transform.position;

                    _infoReceived = true;
                }
            }

            private void SendInputs(PhotonStream stream, Queue<string> q)
            {
                stream.SendNext(q.Count);

                for (int i = 0; i < q.Count; i++)
                {
                    stream.SendNext(q.Dequeue());
                }
            }

            private void ReceiveInputs(PhotonStream stream)
            {
                int length = (int)stream.ReceiveNext();

                for (int i = 0; i < length; i++)
                {
                    string parcel = (string)stream.ReceiveNext();
                    inputsToDo.Enqueue(parcel);
                }
            }
        }
    }
}


