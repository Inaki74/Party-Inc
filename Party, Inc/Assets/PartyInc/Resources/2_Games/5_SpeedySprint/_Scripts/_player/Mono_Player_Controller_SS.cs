using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PlayInc
{
    namespace SS
    {
        /// <summary>
        /// State machine would be overkill, since theres only 4 states.
        /// </summary>
        [RequireComponent(typeof(Mono_Player_Input_SS))]
        public class Mono_Player_Controller_SS : MonoBehaviourPun
        {
            // Components
            private CapsuleCollider _cc;
            [SerializeField] private Mono_Player_Input_SS _inputManager;

            // Mesh variables
            [SerializeField] private MeshRenderer _ubmr;
            [SerializeField] private MeshRenderer _lbmr;
            [SerializeField] private Material mine;

            // The death particles
            [SerializeField] private GameObject _deathParticles;

            // The body parts of our player, useful when ducking.
            public GameObject UpperBody
            {
                get
                {
                    return _upperBody;
                }
                private set
                {
                    _upperBody = value;
                }
            }
            [SerializeField] private GameObject _upperBody;
            [SerializeField] private GameObject _lowerBody;

            public delegate void ActionPlayerLost(int playerId);
            public static event ActionPlayerLost onPlayerDied;

            private bool _hasLost;

            // Jump variables
            [SerializeField] private float _jumpHeight;
            [SerializeField] private float _jumpSpeed; //Increases with global Moving Speed.
            [SerializeField] private float _timeFloating = 0.1444639f; //Decreases with global Moving Speed so that it floats enough time to traverse _timeFloatingRation units while floating.
            private float _timeFloatingRatio = 1.7f; //Amount of units traversed while floating

            // Transform points to raycast from.
            [SerializeField] GameObject _head;
            [SerializeField] GameObject _body;
            [SerializeField] GameObject _feet;
            [SerializeField] GameObject _below;

            // Gravity related variables
            public bool IsGrounded
            {
                get
                {
                    return _isGrounded;
                }
                private set
                {
                    _isGrounded = value;
                }
            }
            private bool _isGrounded;
            private bool _gravity = true;
            [SerializeField] private LayerMask _whatIsGround;
            [SerializeField] private float _distanceGround;

            // Player states to limit player
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

            public Vector3 Direction { get; private set; }
            private Vector3 _lastPosition;

            // Start is called before the first frame update
            void Start()
            {
                if (_cc == null)
                {
                    _cc = GetComponent<CapsuleCollider>();
                }
                if (_inputManager == null)
                {
                    _inputManager = GetComponent<Mono_Player_Input_SS>();
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
                if (!photonView.IsMine) return;

                _lastPosition = transform.position; // t = 1, l = 1
                

                // Check if we are grounded
                _isGrounded = CheckIfGrounded(transform.position);

                // Jump speed scaling
                _jumpSpeed = Mng_GameManager_SS.Current.Gravity * -1 / 1.25f;

                // Time floating scaling
                _timeFloating = _timeFloatingRatio / Mng_GameManager_SS.Current.MovingSpeed;

                // Manage state logic changes
                StateChangeLogic();

                // Move forward
                transform.position += Vector3.forward * Mng_GameManager_SS.Current.MovingSpeed * Time.deltaTime; // t = 2, l = 1;

                // Act per new states
                StateActionLogic();

                // Check if we collided with obstacles that aren't labeled as obstacles (like platforms, which are ground and obstacles)
                // This is done in various different parts of the body
                //Feet
                CheckIfDeadEncapsulation(_feet.transform);

                //Body
                CheckIfDeadEncapsulation(_body.transform);

                if (_upperBody.activeInHierarchy)
                {
                    //Head if not ducking
                    CheckIfDeadEncapsulation(_head.transform);
                }

                // Check if we fell through the ground
                CheckIfPassedGround(_below.transform);

                Direction = transform.position - _lastPosition; // t = 2, l = 1 ; 
            }

            /// <summary>
            /// Simulates gravity for the player. Not a realistic simulation in any way.
            /// </summary>
            private void SimulateGravity()
            {
                transform.position += Vector3.up * Mng_GameManager_SS.Current.Gravity * Time.deltaTime;
            }

            /// <summary>
            /// Manages most State Changing logic, and also ducks in the according occasions.
            /// </summary>
            private void StateChangeLogic()
            {
                // If we duck
                if (_inputManager.DuckInput)
                {
                    // And are grounded and cooldown is done
                    if (_currentState == PlayerStates.Grounded && _currentDuckCooldown < 0f)
                    {
                        Duck();
                    }

                    // If in the air, drop and then duck
                    if (_currentState == PlayerStates.Airborne)
                    {
                        _currentState = PlayerStates.Dropping;
                    }
                }

                // If we are in the ground
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

                // If ducking
                if (_currentState == PlayerStates.Ducking)
                {
                    _currentDuckDuration += Time.deltaTime;
                }
                else
                {
                    _currentDuckCooldown -= Time.deltaTime;
                }
            }

            /// <summary>
            /// Encapsulates most of the logic of what to do depending on our states
            /// </summary>
            private void StateActionLogic()
            {
                // If in the air, gravity is activated and not grounded (these should be the same, oops)
                if (_gravity && (_currentState == PlayerStates.Airborne || _currentState == PlayerStates.Ducking || _currentState == PlayerStates.Dropping) && !_isGrounded)
                {
                    SimulateGravity();
                }

                // If we jump and are grounded or ducking
                if (_inputManager.JumpInput && (_currentState == PlayerStates.Grounded || _currentState == PlayerStates.Ducking))
                {
                    // First unduck
                    if (_currentState == PlayerStates.Ducking)
                    {
                        UnDuck();
                    }
                    // Then jump
                    _currentState = PlayerStates.Airborne; // We shouldnt change state here (that should be in StateLogic()) but I need it here!
                    StartCoroutine(JumpCo(transform.position.y, _jumpHeight, _jumpSpeed));
                }

                // If we are dropping
                if (_currentState == PlayerStates.Dropping && !_isGrounded)
                {
                    transform.position += Vector3.up * -_droppingForce * Time.deltaTime;
                }


                // If we move
                if (_inputManager.MoveInput)
                {
                    // Buffer the inputs
                    if (_moves <= 1 && _movementBuffer[_moves] == 0f)
                    {
                        _movementBuffer[_moves] = _inputManager.MoveDirection;
                        _moves++;
                    }

                    // Move
                    if (_runOnce)
                    {
                        _runOnce = false;
                        StartCoroutine(MoveToCo(_laneSwitchSpeed));
                    }
                }
            }


            /// <summary>
            /// Coroutine Function that moves our player to another lane. It moves statically from a lane to another (discretely). It also acts on its input buffer.
            /// </summary>
            /// <param name="velocity">Speed of move</param>
            /// <returns></returns>
            private IEnumerator MoveToCo(float velocity)
            {
                // Go through our input buffer
                for (int i = 0; i < _movementBuffer.Length; i++)
                {
                    // If its a movement
                    if (_movementBuffer[i] != 0)
                    {
                        // Initial state of movement
                        float decidedX = _middleRailX;
                        float currentX = transform.position.x;
                        float myEpsilon = 0.01f;

                        // Decide where are we moving
                        if (currentX > _leftRailX - myEpsilon && currentX < _leftRailX + myEpsilon)
                        {
                            if (_movementBuffer[i] < 0f)
                            {
                                decidedX = _leftRailX;
                            }
                        }

                        if (currentX > _middleRailX - myEpsilon && currentX < _middleRailX + myEpsilon)
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

                        if (currentX > _rightRailX - myEpsilon && currentX < _rightRailX + myEpsilon)
                        {
                            if (_movementBuffer[i] > 0f)
                            {
                                decidedX = _rightRailX;
                            }
                        }
                        // end decision
                        // Move
                        while (transform.position.x != decidedX)
                        {
                            Vector3 decidedVector = new Vector3(decidedX, transform.position.y, transform.position.z);
                            transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                // Reset input buffer
                _movementBuffer[0] = 0f;
                _movementBuffer[1] = 0f;
                _moves = 0;

                _runOnce = true;
            }

            /// <summary>
            /// Variation of MoveToCo, but for network recreation. Same logic.
            /// </summary>
            /// <param name="velocity"></param>
            /// <param name="direction"></param>
            /// <returns></returns>
            public IEnumerator MoveToNetCo(int direction)
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
                        transform.position = Vector3.MoveTowards(transform.position, decidedVector, _laneSwitchSpeed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                }

                _runOnce = true;
            }

            /// <summary>
            /// Coroutine Function to jump. The player jumps and then is suspended in the air for a bit (gravity is off) before falling.
            /// </summary>
            /// <param name="startingY"></param>
            /// <param name="height"></param>
            /// <param name="velocity"></param>
            /// <returns></returns>
            private IEnumerator JumpCo(float startingY, float height, float velocity)
            {
                // We deactivate gravity so it doesn't affect the jump
                _gravity = false;
                bool reachedHeight = false;

                // Jump
                while (!reachedHeight && !_inputManager.DuckInput)
                {
                    Vector3 decidedVector = new Vector3(transform.position.x, startingY + height, transform.position.z);
                    //Debug.Log("A3");
                    transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                    reachedHeight = transform.position.y > startingY + height - 0.2;
                    yield return new WaitForEndOfFrame();
                }

                float timeFloating = _timeFloating;

                // Float for a certain time
                while (timeFloating > 0f)
                {
                    timeFloating -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                };
                _gravity = true;
            }

            /// <summary>
            /// Public Coroutine Function to jump. Read last func.
            /// </summary>
            /// <param name="startingY"></param>
            /// <param name="height"></param>
            /// <param name="velocity"></param>
            /// <returns></returns>
            public IEnumerator JumpCo(float startingY)
            {
                // We deactivate gravity so it doesn't affect the jump
                _gravity = false;
                bool reachedHeight = false;

                // Jump
                while (!reachedHeight && !_inputManager.DuckInput)
                {
                    Vector3 decidedVector = new Vector3(transform.position.x, startingY + _jumpHeight, transform.position.z);
                    //Debug.Log("A3");
                    transform.position = Vector3.MoveTowards(transform.position, decidedVector, _jumpSpeed * Time.deltaTime);
                    reachedHeight = transform.position.y > startingY + _jumpHeight - 0.2;
                    yield return new WaitForEndOfFrame();
                }

                float timeFloating = _timeFloating;

                // Float for a certain time
                while (timeFloating > 0f)
                {
                    timeFloating -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                };
                _gravity = true;
            }

            /// <summary>
            /// Function that makes our character duck. This is done through the deactivation of the upper collider (and mesh).
            /// </summary>
            private void Duck()
            {
                _currentState = PlayerStates.Ducking;

                _currentDuckDuration = 0.00001f;
                _currentDuckCooldown = _duckCooldown;

                _cc.center = new Vector3(0f, -0.5f, 0f);
                _cc.height = 1;

                _upperBody.SetActive(false);
            }

            /// <summary>
            /// Function that makes our character unduck. This is done through the reactivation of the upper collider (and mesh).
            /// </summary>
            private void UnDuck()
            {
                _currentDuckDuration = _duckDuration + 0.00001f;
                _cc.center = new Vector3(0f, 0f, 0f);
                _cc.height = 2;
                _upperBody.SetActive(true);
            }

            /// <summary>
            /// Checks with a raycast if we are grounded
            /// </summary>
            /// <param name="startPosition"></param>
            /// <returns></returns>
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

            /// <summary>
            /// Checks through raycasting if we collided a non-obstacle. Die if it does.
            /// </summary>
            /// <param name="trans"></param>
            /// <param name="direction"></param>
            private void CheckIfDead(Transform trans, Vector3 direction)
            {
                RaycastHit hit;

                if (Physics.Raycast(trans.position, trans.TransformDirection(direction), out hit, 5f, 1 << 8))
                {
                    Debug.DrawRay(trans.position, trans.TransformDirection(direction) * hit.distance, Color.green, 0f);
                    if (hit.distance < 0.2f && hit.transform.gameObject.tag != "Portal")
                    {
                        Debug.Log("Dead by CheckIfDead");
                        PlayerDie();
                    }
                }else Debug.DrawRay(trans.position, trans.TransformDirection(direction) * 5f, Color.red, 0f);
            }

            private void CheckIfDeadEncapsulation(Transform trans)
            {
                if (_hasLost) return;

                CheckIfDead(trans, Vector3.forward);
                CheckIfDead(trans, Vector3.left);
                CheckIfDead(trans, Vector3.right);
            }

            /// <summary>
            /// Function that checks if we went through the ground by raycasting infinitely into the sky. If we did, it corrects the position.
            /// </summary>
            /// <param name="trans"></param>
            private void CheckIfPassedGround(Transform trans)
            {
                RaycastHit hit;

                if (Physics.Raycast(trans.position, trans.TransformDirection(Vector3.up), out hit, 1000f, 1 << 10))
                {
                    Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.up) * hit.distance, Color.red, 0f);
                    transform.position = hit.point + new Vector3(0f, 1.1f, 0f);
                }
                else
                {
                    Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.up) * 1000f, Color.green, 0f);
                    
                }
            }

            /// <summary>
            /// Function that runs when a player dies.
            /// </summary>
            private void PlayerDie()
            {
                if (Mng_GameManager_SS.Current.Testing)
                {
                    Instantiate(_deathParticles, transform.position, Quaternion.identity);
                    return;
                } 

                // We lost
                _hasLost = true;

                // Invoke events and RPC calls to inform of your death.
                onPlayerDied?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("RPC_InformPlayerLost", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("RPC_PlayerDied", RpcTarget.Others);

                // Instantiates some death particles and deactivates the object.
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

            /////////// NETWORK //////////////


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

            
        }
    }
}


