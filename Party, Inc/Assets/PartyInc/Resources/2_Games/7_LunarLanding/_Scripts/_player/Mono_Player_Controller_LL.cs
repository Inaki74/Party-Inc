﻿using System.Collections;
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
            [SerializeField] private Mono_Camera_Synchronizer_LL _trolleySync;
            private Cinemachine.CinemachineVirtualCamera _camera;
            private Mono_FakePlayer_LL _fakePlayer;

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

            private float _maxX;

            private float _startingMaxX = 13f; // -> 4/13 = r1, where 4 = starting game speed
            private float _finalMaxX = 15.1f; // -> 15/15.1 = r2, where 15 = final game speed
            private float _timeToReachFinalMaxXInSeconds = 80f; // Same as GameManager function

            private bool _touchingObstacle;
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

            private bool _runOnce;

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

                _trolleySync = FindObjectOfType<Mono_Camera_Synchronizer_LL>();
                _camera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
                _fakePlayer = FindObjectOfType<Mono_FakePlayer_LL>();

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
                if (!_boosted)
                {
                    _rb.velocity = new Vector3(Mng_GameManager_LL.Current.MovementSpeed, _rb.velocity.y, 0f);
                }
                else
                {
                    _rb.velocity += Vector3.right * (Mng_GameManager_LL.Current.MovementSpeed - Mng_GameManager_LL.Current.LastRecordedMovementSpeed);
                }

                if (_touchingObstacle)
                {
                    _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                if (_maxX <= _finalMaxX)
                {
                    _maxX = _startingMaxX + Mng_GameManager_LL.Current.InGameTime * ((_finalMaxX - _startingMaxX) / _timeToReachFinalMaxXInSeconds);
                }


                if (Mathf.Abs(transform.position.x) - Mathf.Abs(_trolleySync.IrrelevantPointTwo.position.x) >= _maxX && !_runOnce)
                {
                    // Got through threshold
                    // OnPlayerEnteredThreshold event
                    // _runOnce = true;
                }
                else if (Mathf.Abs(transform.position.x) - Mathf.Abs(_trolleySync.IrrelevantPointTwo.position.x) < _maxX && _runOnce)
                {
                    // Got away of the threshold
                    // If I had the camera, I lose it
                    // To who? It doesnt matter, all that it matters is that you lost it.
                    // Fake player decides who gets the camera next.
                    // OnPlayerLeftThreshold event
                    // _runOnce = false;
                }

                //if (Mathf.Abs(transform.position.x) - Mathf.Abs(_trolleySync.IrrelevantPointTwo.position.x) >= _maxX && !_runOnce)
                //{
                //    // We are getting through the max distance
                //    //if(_trolleySync.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
                //    //{
                //    //    Debug.Log("Got camera!");
                //    //    //_trolleySync.photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                //    //    //_camera.Follow = transform;
                //    //    _fakePlayer.LoseCamera(transform);
                //    //}
                //    _runOnce = true;
                //    Debug.Log("Got camera!");
                //    _fakePlayer.LoseCamera(transform);
                //}
                //else if(Mathf.Abs(transform.position.x) - Mathf.Abs(_trolleySync.IrrelevantPointTwo.position.x) < _maxX && _runOnce &&
                //    _fakePlayer.photonView.IsMine)//if(_trolleySync.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)// && !_trolleySync.ChangingOwner)
                //{
                //    Debug.Log("Lost Camera...");
                //    _runOnce = false;
                //    _fakePlayer.RegainCamera();
                //}

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
                    _touchingObstacle = false;
                }
            }
        }
    }
}



