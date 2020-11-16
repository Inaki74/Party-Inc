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

            private bool _hasLost;

            [SerializeField] float _jumpHeight;
            [SerializeField] float _jumpSpeed;

            private Vector3 _velocity;

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
            private float _duckCompensationSetting = 0.4f; //Do not change
            private float _duckCompensation;
            private Vector3 _standingScale;
            private Vector3 _duckingScale = new Vector3(1f, 0.5f, 1f);
            [SerializeField] private float _droppingForce;

            private float _compensation = 0.12f; //DO not change

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
            private Vector3 _lastPos;
            private Vector3 _lastVel;
            private float _lastDrag;
            private Quaternion _lastRot;

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

                _standingScale = transform.localScale;

                _runOnce = true;
                _middleRailX = transform.position.x;
                _leftRailX = _middleRailX - 3;
                _rightRailX = _middleRailX + 3;

                _duckCompensation = 0f;

                _currentState = PlayerStates.Grounded;
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected && _infoReceived)
                {
                    transform.position = Vector3.Lerp(transform.position, _lastPos, Time.deltaTime);
                    _rb.velocity = _lastVel;
                    _rb.drag = _lastDrag;
                    if (_hasLost)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, _lastRot, Time.deltaTime);
                    }
                    return;
                }

                bool isGrounded = CheckIfGrounded();

                StateLogic(isGrounded);
            }

            private void FixedUpdate()
            {
                // The compensation exists because when the object ducks (shrinks) it hits the ground a bit, slowing down a tiny bit
                // But can add up and ruin the game. I lerped the position, still doesnt work. DOnt know what to do. So this will do till i find a real solution.
                // Nevermind, its always going back. I dont know why, and I want to move on. SO heres a compensation.
                // Nevermind 2, i found out its the intersection point of planes.
                _velocity = new Vector3(0f, _rb.velocity.y, GameManager.Current.MovingSpeed + _compensation + _duckCompensation);

                if (_inputManager.JumpInput && _currentState == PlayerStates.Grounded)
                {
                    _currentState = PlayerStates.Airborne;
                    StartCoroutine(JumpCo(transform.position.y, _jumpHeight, _jumpSpeed));
                }

                if(_currentState == PlayerStates.Dropping)
                {
                    _velocity = new Vector3(0f, -_droppingForce, GameManager.Current.MovingSpeed);
                }

                if(_inputManager.MoveInput)
                {
                    if (_runOnce)
                    {
                        _runOnce = false;
                        StartCoroutine(MoveToCo(_laneSwitchSpeed, _inputManager.MoveDirection));
                    }
                }

                //DecideDrag();

                _rb.velocity = _velocity;
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
                    Vector3 decidedVector = new Vector3(decidedX, transform.position.y, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, decidedVector, velocity * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
                _runOnce = true;
            }

            private void StateLogic(bool isGrounded)
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

                // If its not grounded its in the air.
                if (!isGrounded && _currentState != PlayerStates.Dropping && _currentState != PlayerStates.Ducking)
                {
                    _currentState = PlayerStates.Airborne;
                }

                // If ducked in the air, it must fall quick. Once it reaches the ground, duck.
                if (_currentState == PlayerStates.Dropping && isGrounded)
                {
                    Duck();
                }

                // If grounded, set state to grounded. If came from ducking, unduck.
                if (_currentDuckDuration > _duckDuration && isGrounded)
                {
                    if (_currentState == PlayerStates.Ducking)
                    {
                        UnDuck();
                    }
                    _currentState = PlayerStates.Grounded;
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
                _rb.useGravity = false;
                bool reachedHeight = false;

                while (!reachedHeight && !_inputManager.DuckInput){
                    Vector3 decidedVector = new Vector3(transform.position.x, startingY + height, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, decidedVector, velocity * Time.deltaTime);
                    reachedHeight = transform.position.y > startingY + height - 0.05;
                    yield return new WaitForEndOfFrame();
                }

                float timeFloating = 0.1f;

                while (reachedHeight || timeFloating > 0f)
                {
                    reachedHeight = false;
                    timeFloating -= Time.deltaTime;
                    transform.position = new Vector3(transform.position.x, startingY + height, transform.position.z);
                }
                _rb.useGravity = true;
            }


            private void Duck()
            {
                _currentState = PlayerStates.Ducking;

                _currentDuckDuration = 0.00001f;
                _currentDuckCooldown = _duckCooldown;
                _duckCompensation = _duckCompensationSetting;

                StopCoroutine("DuckCo");
                StartCoroutine(DuckCo(_duckingScale, 0.1f));
            }

            private void UnDuck()
            {
                _duckCompensation = 0f;
                StopCoroutine("DuckCo");
                StartCoroutine(DuckCo(_standingScale, 0.1f));
            }

            private IEnumerator DuckCo(Vector3 targetScale, float time)
            {
                while (transform.localScale != targetScale)
                {
                    transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, time);
                    if(transform.localScale.y - targetScale.y > 0)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0.5f, transform.position.y), time);
                    }
                    
                    yield return new WaitForEndOfFrame();
                }
            }

            private bool CheckIfGrounded()
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position - new Vector3(0f, _cc.bounds.extents.y - _cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), out hit, _distanceGround, _whatIsGround))
                {
                    Debug.DrawRay(transform.position - new Vector3(0f, _cc.bounds.extents.y - _cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), Color.red, hit.distance);
                    return true;
                }
                else
                {
                    Debug.DrawRay(transform.position - new Vector3(0f, _cc.bounds.extents.y - _cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), Color.green, _distanceGround);
                    return false;
                }
            }

            [PunRPC]
            public void RPC_InformPlayerLost()
            {
                _hasLost = true;
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(_rb.velocity);
                    stream.SendNext(transform.position);
                    stream.SendNext(_rb.drag);
                    if (_hasLost) stream.SendNext(transform.rotation);
                }
                else
                {
                    _lastVel = (Vector3)stream.ReceiveNext();
                    _lastPos = (Vector3)stream.ReceiveNext();
                    _lastDrag = (float)stream.ReceiveNext();

                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    _lastPos += lag * _lastVel;

                    _infoReceived = true;

                    if (_hasLost)
                    {
                        _rb.constraints = RigidbodyConstraints.None;
                        _lastRot = (Quaternion)stream.ReceiveNext();
                    }
                }
            }
        }
    }
}


