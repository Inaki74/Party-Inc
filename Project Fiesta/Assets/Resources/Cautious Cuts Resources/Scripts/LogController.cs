using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    namespace CC
    {
        public class LogController : MonoBehaviourPun
        {
            public delegate void ActionLogDestroyed();
            public static event ActionLogDestroyed onLogDestroyed;

            [SerializeField] private Rigidbody _rb;
            [SerializeField] private GameObject _feetMarker;

            private Vector3 _startPos;
            private Vector3 _whereToGo;
            private Vector3 _differenceFeetCenter;
            private float _speed;
            private bool _runOnce;

            private float _windowTime;
            public float WindowTime
            {
                get
                {
                    return _windowTime;
                }
                set
                {
                    _windowTime = value;
                }
            }

            private float _waitTime;
            public float WaitTime
            {
                get
                {
                    return _waitTime;
                }
                set
                {
                    _waitTime = value;
                }
            }

            private float _timeToMove;
            public float TimeToMove
            {
                get
                {
                    return _timeToMove;
                }
                set
                {
                    _timeToMove = value;
                }
            }

            // Start is called before the first frame update
            void Start()
            {
                if (_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }

                _runOnce = false;
                _startPos = transform.position;
                _speed = 40f;
                _whereToGo = new Vector3(transform.position.x, 0.5f, transform.position.z);
                _differenceFeetCenter = transform.position - _feetMarker.transform.position;

                _whereToGo += _differenceFeetCenter;

                photonView.RPC("RPC_SendTimes", RpcTarget.Others, WaitTime, WindowTime, TimeToMove);
            }

            // Update is called once per frame
            void Update()
            {
                if(PhotonNetwork.Time >= WaitTime)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _whereToGo, Time.deltaTime * _speed);

                    if (transform.position == _whereToGo && !_runOnce)
                    {
                        _runOnce = true;
                        StartCoroutine(Wait());
                    }
                }

                
            }

            private IEnumerator Wait()
            {
                yield return new WaitForSeconds(WindowTime);

                _whereToGo = _startPos;

                yield return new WaitUntil(() => transform.position == _whereToGo);

                // Trigger event where next wave is spawned
                if(PhotonNetwork.IsMasterClient && photonView.IsMine)
                {
                    //yield return StartCoroutine(SendNextWaveCo(WaitTime));
                    object[] content = new object[] { };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(GameManager.NextLogWaveEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
                }

                if (photonView.IsMine)
                {
                    onLogDestroyed.Invoke();
                }
                
                // Dispose of the log
                // Later I gotta save it? Ill see.
                Destroy(gameObject);
            }

            public IEnumerator SendNextWaveCo(float wT)
            {
                yield return new WaitUntil(() => (float)PhotonNetwork.Time >= wT);
                object[] content = new object[] { };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(GameManager.NextLogWaveEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
            }

            private void OnEnable()
            {
                //photonView.RPC("RPC_SetActive", RpcTarget.Others, true);
            }

            private void OnDisable()
            {
                //photonView.TransferOwnership(0);
                transform.rotation = Quaternion.identity;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;

                //photonView.RPC("RPC_SetActive", RpcTarget.Others, false);
            }


            /// NETWORKING
            ///
            [PunRPC]
            public void RPC_SendTimes(float wT, float winT, float tTM)
            {
                WaitTime = wT;
                WindowTime = winT;
                TimeToMove = tTM;
            }
        }
    }
}