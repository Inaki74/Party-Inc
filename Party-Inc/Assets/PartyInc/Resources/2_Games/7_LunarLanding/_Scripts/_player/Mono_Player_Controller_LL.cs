using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_Player_Controller_LL : MonoBehaviourPun
        {
            [SerializeField] private Mono_Camera_Synchronizer_LL _trolleySync;

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
            [SerializeField] private CapsuleCollider _cc;

            public CapsuleCollider Cc
            {
                get
                {
                    return _cc;
                }
                private set
                {
                    _cc = value;
                }
            }

            [SerializeField] private GameObject _top;
            [SerializeField] private GameObject _bottom;
            [SerializeField] private GameObject _spray;

            [SerializeField] private GameObject _markOne;
            [SerializeField] private GameObject _markTwo;

            [SerializeField] private float _sprayStrength;

            private bool _boosted;
            public bool Boosted
            {
                get
                {
                    return _boosted;
                }
                set
                {
                    _boosted = value;
                }
            }

            private bool _runOnce = false;

            private void Start()
            {
                if (_rb == null)
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

                _trolleySync = FindObjectOfType<Mono_Camera_Synchronizer_LL>();

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

            private void FixedUpdate()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                if (!_boosted)
                {
                    _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                //if (Mng_GameManager_LL.Current.MaxX <= _finalMaxX)
                //{
                //    //_maxX = _startingMaxX + Mng_GameManager_LL.Current.InGameTime * ((_finalMaxX - _startingMaxX) / _timeToReachFinalMaxXInSeconds);
                //}

                if (Mathf.Abs(transform.position.x - _trolleySync.IrrelevantPointTwo.position.x) >= Mng_GameManager_LL.Current.MaxX && !_runOnce)
                {
                    Debug.Log("Got through threshold");
                    // Got through threshold
                    // OnPlayerEnteredThreshold event
                    object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.Time };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(Constants.PlayerEnteredThresholdEventCode, content, raiseEventOptions, SendOptions.SendReliable);

                    _runOnce = true;
                }
                else if (Mathf.Abs(transform.position.x - _trolleySync.IrrelevantPointTwo.position.x) < Mng_GameManager_LL.Current.MaxX && _runOnce)
                {
                    Debug.Log("Left threshold");
                    // Got away of the threshold
                    // If I had the camera, I lose it
                    // To who? It doesnt matter, all that it matters is that you lost it.
                    // Fake player decides who gets the camera next.
                    // OnPlayerLeftThreshold event
                    object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, 0d };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(Constants.PlayerLeftThresholdEventCode, content, raiseEventOptions, SendOptions.SendReliable);

                    _runOnce = false;
                }

                ProcessInput();
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

            public void ResetBoost(float duration, float speed)
            {
                StartCoroutine(ResetBoostCo(duration, speed));
            }

            private IEnumerator ResetBoostCo(float duration, float speed)
            {
                yield return new WaitForSeconds(duration);

                _rb.velocity = _rb.velocity - Vector3.right * speed;
                Boosted = false;
            }

            private void Die()
            {
                Mng_GameManager_LL.Current.IsHighScore = HighScoreHelpers.DetermineHighScoreInt(PartyInc.Constants.LL_KEY_HISCORE, Mng_GameManager_LL.Current.CurrentGate, true);

                object[] content = new object[] { Mng_GameManager_LL.Current.CurrentGate, PhotonNetwork.LocalPlayer.ActorNumber };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(Constants.PlayerDiedEventCode, content, raiseEventOptions, SendOptions.SendReliable);

                PhotonNetwork.Destroy(gameObject);
            }

            private void OnTriggerEnter(Collider other)
            {
                if (other.gameObject.tag == "DeathPlane")
                {
                    Die();
                }
            }
        }
    }
}





