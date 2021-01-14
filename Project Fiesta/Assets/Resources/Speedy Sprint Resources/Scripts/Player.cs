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

            [SerializeField] GameObject _feet;
            private bool _isGrounded;
            private bool _gravity = true;
            private float _yVelocity = 0f;
            private float _yVelocityCap = 50f;
            private Vector3 _oldFeetPos;
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
            
            private Quaternion _netRot;
            private Vector3 _lastPos;
            private Quaternion _lastRot;

            private bool _isJumping;
            private bool _isMoving;

            private bool _netDuck;
            private bool _netJump;
            private bool _netMove;
            private bool _netFalling;
            private bool _netDropping;

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

                    if (_hasLost)
                    {
                        transform.rotation = Quaternion.Lerp(_lastRot, _netRot, (float)(_time / timeToSend));
                    }
                    return;
                }

                _jumpSpeed = GameManager.Current.Gravity * -1 / 1.25f;

                _timeFloating = _timeFloatingRatio / GameManager.Current.MovingSpeed;

                _oldFeetPos = _feet.transform.position;

                StateLogic();

                transform.position += Vector3.forward * GameManager.Current.MovingSpeed * Time.deltaTime;

                if(_gravity && _currentState == PlayerStates.Airborne && !_isGrounded)
                {
                    SimulateGravity();
                }
                else
                {
                    _yVelocity = 0f;
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

                CheckIfDead(Vector3.forward);
                CheckIfDead(Vector3.back);
                CheckIfDead(Vector3.left);
                CheckIfDead(Vector3.right);
            }

            private void FixedUpdate()
            {
                
            }

            private void SimulateGravity()
            {
                if (_yVelocity > _yVelocityCap * -1 && _yVelocity < _yVelocityCap)
                {
                    //_yVelocity += GameManager.Current.Gravity * Time.deltaTime;
                }
                transform.position += Vector3.up * GameManager.Current.Gravity * Time.deltaTime;
            }

            private IEnumerator MoveToCo(float velocity)
            {
                for (int i = 0; i < _movementBuffer.Length; i++)
                {
                    if(_movementBuffer[i] != 0)
                    {
                        _isMoving = true;
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
                            Debug.Log("A1");
                            Vector3 decidedVector = new Vector3(decidedX, transform.position.y, transform.position.z);
                            transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                            yield return new WaitForEndOfFrame();
                        }
                        _isMoving = false;
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
                        Debug.Log("A1");
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
                _isJumping = true;
                bool reachedHeight = false;

                float timeElapsed = 0f;
                float distanceTravelled = 0f;

                while (!reachedHeight && !_inputManager.DuckInput){

                    Vector3 decidedVector = new Vector3(transform.position.x, startingY + height, transform.position.z);
                    Debug.Log("A3");
                    transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                    reachedHeight = transform.position.y > startingY + height - 0.2;
                    yield return new WaitForEndOfFrame();
                }

                float timeFloating = _timeFloating;

                while (reachedHeight || timeFloating > 0f)
                {
                    timeElapsed += Time.deltaTime;
                    distanceTravelled += Time.deltaTime * GameManager.Current.MovingSpeed;

                    if (distanceTravelled >= 1f)
                    {
                        Debug.Log(timeElapsed);
                        distanceTravelled = -10000;
                    }

                    reachedHeight = false;
                    timeFloating -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                };
                _isJumping = false;
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

            private void CheckIfMissedGround()
            {
                RaycastHit hit;
                //_oldFeetPos - _feet.transform.position   Vector3.Distance(_feet.transform.position, _oldFeetPos)
                if (Physics.Raycast(_feet.transform.position, _feet.transform.TransformDirection(_oldFeetPos - _feet.transform.position), out hit, Vector3.Distance(_feet.transform.position, _oldFeetPos), _whatIsGround))
                {
                    // We missed the ground, time to correct position
                    Debug.Log("Got through");
                    Debug.DrawRay(_feet.transform.position, _feet.transform.TransformDirection(_oldFeetPos - _feet.transform.position) * hit.distance, Color.cyan, 0f);

                    transform.position = hit.point + new Vector3(0f, 0.2f, 0f); 
                }
                else
                {
                    Debug.Log("Didnt get through");
                    Debug.DrawRay(_feet.transform.position, _feet.transform.TransformDirection(_oldFeetPos - _feet.transform.position) * Vector3.Distance(_feet.transform.position, _oldFeetPos), Color.blue, 0f);
                }
            }

            private void CheckIfDead(Vector3 direction)
            {
                RaycastHit hit;

                if (Physics.Raycast(_feet.transform.position, _feet.transform.TransformDirection(direction), out hit, 5f, 1 << 8))
                {
                    Debug.DrawRay(_feet.transform.position, _feet.transform.TransformDirection(direction) * hit.distance, Color.green, 0f);
                    if (hit.distance < 0.1f)
                    {
                        PlayerDie();
                    }
                }else Debug.DrawRay(_feet.transform.position, _feet.transform.TransformDirection(direction) * 5f, Color.red, 0f);
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
                transform.position += Vector3.forward * GameManager.Current.MovingSpeed * Time.deltaTime;

                if (_gravity && !_isGrounded)
                {
                    SimulateGravity();
                }
                else
                {
                    _yVelocity = 0f;
                }

                StartCoroutine("RecreateInputsCo");

                //if (_netFalling || _netJump || _netMove)
                //{
                //    transform.position = Vector3.Lerp(_lastPos, _netPos, (float)(_time / timeToSend));
                //}
                //else
                //{
                //    transform.position += Vector3.forward * GameManager.Current.MovingSpeed * Time.deltaTime;
                //}
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
                Debug.Log("DUDE DIED");
                Instantiate(_deathParticles, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }

            [PunRPC]
            public void RPC_InformPlayerLost(int playerId)
            {
                _hasLost = true;
                onPlayerDied?.Invoke(playerId);
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    //stream.SendNext(_isJumping);
                    //stream.SendNext(_isMoving);
                    //stream.SendNext(_gravity && _currentState == PlayerStates.Airborne && !_isGrounded);
                    //stream.SendNext(_currentState == PlayerStates.Dropping && !_isGrounded);

                    SendInputs(stream, _inputManager.currentInputs);

                    stream.SendNext(transform.position);
                    stream.SendNext(_upperBody.activeInHierarchy);
                    if (_hasLost) stream.SendNext(transform.rotation);
                }
                else
                {
                    //_netJump = (bool)stream.ReceiveNext();
                    //_netMove = (bool)stream.ReceiveNext();
                    //_netFalling = (bool)stream.ReceiveNext();
                    //_netDropping = (bool)stream.ReceiveNext();

                    ReceiveInputs(stream);

                    _netPos = (Vector3)stream.ReceiveNext();
                    _netDuck = (bool)stream.ReceiveNext();

                    _time = 0f;
                    _lastSendTime = _currentSendTime;
                    _currentSendTime = info.SentServerTime;
                    _lastPos = transform.position;

                    _infoReceived = true;

                    if (_hasLost)
                    {
                        _rb.constraints = RigidbodyConstraints.None;
                        _netRot = (Quaternion)stream.ReceiveNext();
                        _lastRot = transform.rotation;
                    }
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
                    Debug.Log(parcel);
                }
            }

        }
    }
}


