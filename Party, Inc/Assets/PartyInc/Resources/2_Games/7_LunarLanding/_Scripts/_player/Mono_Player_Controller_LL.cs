using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_Player_Controller_LL : MonoBehaviourPun
        {
            [SerializeField] private Mono_CameraTrolley_LL _trolley;

            public Rigidbody Rb
            {
                get
                {
                    return _rb;
                }
                private set
                {
                    _rb = value;
                }
            }
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private Mono_Player_Input_LL _input;
            [SerializeField] private MeshRenderer _mr;
            [SerializeField] private Material _mineMaterial;

            [SerializeField] private GameObject _top;
            [SerializeField] private GameObject _bottom;
            [SerializeField] private GameObject _spray;

            [SerializeField] private GameObject _markOne;
            [SerializeField] private GameObject _markTwo;

            [SerializeField] private float _sprayStrength;

            private float _maxX = 13f;
            private float _startingXRelativeToPlayer;

            private bool _touchingObstacle;

            private void Start()
            {
                if(_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }

                if (_input == null)
                {
                    _input = GetComponent<Mono_Player_Input_LL>();
                }

                if (_mr == null)
                {
                    _mr = GetComponent<MeshRenderer>();
                }

                _trolley = FindObjectOfType<Mono_CameraTrolley_LL>();

                _startingXRelativeToPlayer = transform.position.x;

                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine) _mr.material = _mineMaterial;
                    else
                    {
                        Color col = _mr.material.color;
                        col.a = 0.4f;
                        _mr.material.color = col;
                    }
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                if (!_touchingObstacle)
                {
                    transform.position += Vector3.right * Mng_GameManager_LL.Current.MovementSpeed * Time.deltaTime;
                }
                else
                {
                    _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
                }

                if(Mathf.Abs(transform.position.x - _startingXRelativeToPlayer) >= _maxX)
                {
                    // We are getting through the max distance
                    _trolley._extra = _rb.velocity.x;
                    if(_trolley.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber) _trolley.photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else if(_trolley.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    _trolley._extra = 0f;
                    _trolley.photonView.TransferOwnership(0);
                }

                if(_rb.velocity.x <= 0)
                {
                    _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
                }

                ProcessInput();

                _startingXRelativeToPlayer += (Mng_GameManager_LL.Current.MovementSpeed + _trolley._extra) * Time.deltaTime;
            }

            private void ProcessInput()
            {
                if (_input.SprayUpInput)
                {
                    _rb.AddForce(Vector3.down * _sprayStrength, ForceMode.Force);
                    SprayAnimation(true);
                }

                if (_input.SprayDownInput)
                {
                    _rb.AddForce(Vector3.up * _sprayStrength, ForceMode.Force);
                    SprayAnimation(false);
                }
            }

            private void SprayAnimation(bool topSide)
            {
                GameObject spray;

                if (topSide)
                {
                    spray = Instantiate(_spray, _top.transform.position, Quaternion.identity);
                }
                else
                {
                    spray = Instantiate(_spray, _bottom.transform.position, Quaternion.AngleAxis(180f, Vector3.right));
                }

                ParticleSystem pSys = spray.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = pSys.main;
                main.duration = Time.deltaTime;

                pSys.Play();
            }

            private void Die()
            {
                Mng_GameManager_LL.Current.IsHighScore = HighScoreHelpers.DetermineHighScoreInt(PartyInc.Constants.LL_KEY_HISCORE, Mng_GameManager_LL.Current.CurrentGate, true);

                object[] content = new object[] { Mng_GameManager_LL.Current.CurrentGate, PhotonNetwork.LocalPlayer.ActorNumber };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                PhotonNetwork.RaiseEvent(Constants.PlayerDiedEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);

                Destroy(gameObject);
            }

            private void OnTriggerEnter(Collider other)
            {
                if(other.gameObject.tag == "DeathPlane" && (photonView.IsMine || !PhotonNetwork.IsConnected))
                {
                    Die();
                }
            }

            private void OnCollisionEnter(Collision collision)
            {
                if(collision.gameObject.tag == "Obstacle" && (photonView.IsMine || !PhotonNetwork.IsConnected))
                {
                    _touchingObstacle = true;
                }
            }

            private void OnCollisionStay(Collision collision)
            {
                if (collision.gameObject.tag == "Obstacle" && (photonView.IsMine || !PhotonNetwork.IsConnected))
                {
                    _touchingObstacle = true;
                }
            }

            private void OnCollisionExit(Collision collision)
            {
                if (collision.gameObject.tag == "Obstacle" && (photonView.IsMine || !PhotonNetwork.IsConnected))
                {
                    Debug.Log("DUDE");
                    _touchingObstacle = false;
                }
            }
        }
    }
}



