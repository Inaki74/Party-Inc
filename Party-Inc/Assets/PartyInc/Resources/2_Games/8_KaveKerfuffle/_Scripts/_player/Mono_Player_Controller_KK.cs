using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_Player_Controller_KK : MonoBehaviourPun
        {
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
            [SerializeField] private Mono_Player_Input_KK _input;
            [SerializeField] private Mono_Player_Synchronizer_KK _sync;
            [SerializeField] private MeshRenderer _mr;
            [SerializeField] private CapsuleCollider _cc;
            [SerializeField] private Material _mineMaterial;

            private Cinemachine.CinemachineImpulseSource _shake;

            [SerializeField] private GameObject _deathParticlesPrefab;

            [SerializeField] private GameObject _top;
            [SerializeField] private GameObject _bottom;

            public GameObject Top
            {
                get
                {
                    return _top;
                }
                private set
                {
                    _top = value;
                }
            }

            public GameObject Bottom
            {
                get
                {
                    return _bottom;
                }
                private set
                {
                    _bottom = value;
                }
            }

            [SerializeField] private GameObject _spray;

            [SerializeField] private float _sprayStrength;

            private bool _invincible = true;

            private void Start()
            {
                if (_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }

                if (_input == null)
                {
                    _input = GetComponent<Mono_Player_Input_KK>();
                }

                if (_sync == null)
                {
                    _sync = GetComponent<Mono_Player_Synchronizer_KK>();
                }

                if (_mr == null)
                {
                    _mr = GetComponent<MeshRenderer>();
                }

                _shake = FindObjectOfType<Cinemachine.CinemachineImpulseSource>();

                if (photonView.IsMine) _mr.material = _mineMaterial;
                else
                {
                    Color col = _mr.material.color;
                    col.a = 0.4f;
                    _mr.material.color = col;
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine) return;

                transform.position += Vector3.right * Mng_GameManager_KK.Current.MovementSpeed * Time.deltaTime;

                if (_rb.velocity.x != 0)
                {
                    _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
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
                if (topSide)
                {
                    PhotonNetwork.Instantiate("_particles/" + _spray.name, _top.transform.position, Quaternion.identity);
                }
                else
                {
                    PhotonNetwork.Instantiate("_particles/" + _spray.name, _bottom.transform.position, Quaternion.AngleAxis(180f, Vector3.right));
                }
            }

            private void PlayerLost()
            {
                Mng_GameManager_KK.Current.IsHighScore = HighScoreHelpers.DetermineHighScoreInt(PartyInc.Constants.KK_KEY_HISCORE, Mng_GameManager_KK.Current.CurrentGate, true);

                object[] content = new object[] { Mng_GameManager_KK.Current.CurrentGate, PhotonNetwork.LocalPlayer.ActorNumber };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(Constants.PlayerDiedEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);

                photonView.RPC("RPC_MakeVulnerable", RpcTarget.Others);
                Die();
            }

            private void Die()
            {
                _mr.enabled = false;
                _cc.enabled = false;

                Instantiate(_deathParticlesPrefab, transform.position, Quaternion.identity);
                _shake.GenerateImpulse(new Vector3(1f, 1f,0f) * Mng_GameManager_KK.Current.MovementSpeed * 0.01333333333f);
            }

            [PunRPC]
            public void RPC_MakeVulnerable()
            {
                _invincible = false;
                _cc.height = 2.4f;
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.tag == "Obstacle")
                {
                    if (photonView.IsMine)
                    {
                        PlayerLost();
                    }
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                if (other.gameObject.tag == "Obstacle")
                {
                    if (!photonView.IsMine && !_invincible)
                    {
                        Die();
                    }
                }
            }

            private void OnTriggerStay(Collider other)
            {
                if (other.gameObject.tag == "Obstacle")
                {
                    if (!photonView.IsMine && !_invincible)
                    {
                        Die();
                    }
                }
            }
        }
    }
}
