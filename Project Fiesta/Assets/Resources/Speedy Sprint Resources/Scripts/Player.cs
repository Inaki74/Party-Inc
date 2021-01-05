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

            [SerializeField] private GameObject _upperBody;
            [SerializeField] private GameObject _lowerBody;

            public delegate void ActionPlayerLost(int playerId);
            public static event ActionPlayerLost onPlayerDied;

            private bool _hasLost;

            [SerializeField] float _jumpHeight;
            [SerializeField] float _jumpSpeed;

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
            private bool _runOnce;
            [SerializeField] private float _laneSwitchSpeed;
            private float _leftRailX;
            private float _middleRailX;
            private float _rightRailX;

            [SerializeField] private LayerMask _whatIsGround;
            [SerializeField] private float _distanceGround;

            // Network variables

            private bool _infoReceived;
            private Vector3 _netPos;
            private Vector3 _netScale;
            private Quaternion _netRot;
            private Vector3 _lastPos;
            private Vector3 _lastScale;
            private Quaternion _lastRot;

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
                if (!photonView.IsMine && PhotonNetwork.IsConnected && _infoReceived)
                {
                    double timeToSend = _currentSendTime - _lastSendTime;

                    _time += Time.deltaTime;
                    if(Vector3.Distance(_lastPos, _netPos) < _networkTeleportDistance)
                    {
                        
                        transform.position = Vector3.Lerp(_lastPos, _netPos, (float)(_time / timeToSend));
                    }
                    else
                    {
                        transform.position = _netPos;
                    }

                    transform.localScale = Vector3.MoveTowards(_lastScale, _netScale, (float)(_time / timeToSend));

                    if (_hasLost)
                    {
                        transform.rotation = Quaternion.Lerp(_lastRot, _netRot, (float)(_time / timeToSend));
                    }
                    return;
                }

                _oldFeetPos = _feet.transform.position;

                _isGrounded = CheckIfGrounded(transform.position);
                
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
                    if (_runOnce)
                    {
                        _runOnce = false;
                        StartCoroutine(MoveToCo(_laneSwitchSpeed, _inputManager.MoveDirection));
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
                    _yVelocity += GameManager.Current.Gravity * Time.deltaTime;
                }
                transform.position += Vector3.up * _yVelocity * Time.deltaTime;
            }

            private IEnumerator MoveToCo(float velocity, float direction)
            {
                float decidedX = _middleRailX;
                float currentX = transform.position.x;

                if (currentX == _leftRailX)
                {
                    if(direction < 0f)
                    {
                        decidedX = _leftRailX;
                    }
                }

                if(currentX == _middleRailX)
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

                if(currentX == _rightRailX)
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
                    Debug.Log("A3");
                    transform.position = Vector3.Lerp(transform.position, decidedVector, velocity * Time.deltaTime);
                    reachedHeight = transform.position.y > startingY + height - 0.2;
                    yield return new WaitForEndOfFrame();
                }

                //float timeFloating = 0.05f;

                //while (reachedHeight || timeFloating > 0f)
                //{
                //    reachedHeight = false;
                //    timeFloating -= Time.deltaTime;
                //    yield return new WaitForEndOfFrame();
                //};
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
                    if (hit.distance < 0.2f)
                    {
                        PlayerDie();
                    }
                }else Debug.DrawRay(_feet.transform.position, _feet.transform.TransformDirection(direction) * 5f, Color.red, 0f);
            }

            private void PlayerDie()
            {
                gameObject.SetActive(false);
                _hasLost = true;
                onPlayerDied?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("RPC_InformPlayerLost", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("RPC_Disable", RpcTarget.All);
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

            [PunRPC]
            public void RPC_Disable()
            {
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

                    stream.SendNext(transform.position);
                    stream.SendNext(transform.localScale);
                    if (_hasLost) stream.SendNext(transform.rotation);
                }
                else
                {
                    _netPos = (Vector3)stream.ReceiveNext();
                    _netScale = (Vector3)stream.ReceiveNext();

                    _time = 0f;
                    _lastSendTime = _currentSendTime;
                    _currentSendTime = info.SentServerTime;
                    _lastPos = transform.position;
                    _lastScale = transform.localScale;

                    _infoReceived = true;

                    if (_hasLost)
                    {
                        _rb.constraints = RigidbodyConstraints.None;
                        _netRot = (Quaternion)stream.ReceiveNext();
                        _lastRot = transform.rotation;
                    }
                }
            }
        }
    }
}


