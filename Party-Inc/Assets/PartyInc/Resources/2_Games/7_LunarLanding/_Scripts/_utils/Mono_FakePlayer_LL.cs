using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

namespace PartyInc
{
    namespace LL
    {
        public struct FPSync
        {
            public int playerId;
            public double timeSent;

            public FPSync(int id, double time)
            {
                playerId = id;
                timeSent = time;
            }

            public override bool Equals(object obj)
            {
                FPSync aux = (FPSync)obj;
                return aux.playerId == playerId;
            }

            public override int GetHashCode()
            {
                return (int)(playerId * (float)timeSent);
            }
        }

        public class Mono_FakePlayer_LL : MonoBehaviourPun
        {
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private Mono_Camera_Synchronizer_LL _trolleySync;
            [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;

            private Mono_Player_Controller_LL[] _allPlayerControllers = new Mono_Player_Controller_LL[4];

            private Transform _playerThatHasCamera;
            private float _originalX;
            private bool _lostCamera = false;

            private void Awake()
            {
                PhotonNetwork.NetworkingClient.EventReceived += OnPlayerEnteredThreshold;
                PhotonNetwork.NetworkingClient.EventReceived += OnPlayerLeftThreshold;
            }

            private void OnDestroy()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= OnPlayerEnteredThreshold;
                PhotonNetwork.NetworkingClient.EventReceived -= OnPlayerLeftThreshold;
            }

            private void Update()
            {
                if(_allPlayerControllers.Count() != Mng_GameManager_LL.Current.playerCount)
                {
                    _allPlayerControllers = FindObjectsOfType<Mono_Player_Controller_LL>();
                }
            }

            // Update is called once per frame
            void FixedUpdate()
            {
                _originalX = _trolleySync.IrrelevantPoint.position.x;
                if (_lostCamera)
                {
                    transform.position = new Vector3(_playerThatHasCamera.position.x, transform.position.y, 0f);//Vector3.Lerp(transform.position, new Vector3(_playerThatHasCamera.position.x, transform.position.y, 0f), Time.deltaTime * 10f);
                }
                else
                {
                    _rb.velocity = new Vector3(Mng_GameManager_LL.Current.MovementSpeed, 0f, 0f);
                }
            }

            private void GiveCamera(int playerId)
            {
                foreach(Mono_Player_Controller_LL p in _allPlayerControllers)
                {
                    if(p.photonView.OwnerActorNr == playerId)
                    {
                        _playerThatHasCamera = p.transform;
                    }
                }

                _lostCamera = true;
            }

            private void RegainCamera()
            {
                _playerThatHasCamera = null;
                _lostCamera = false;
                transform.position = Vector3.right * _originalX;//Vector3.Lerp(transform.position, Vector3.right * _originalX, Time.deltaTime * 10f);
            }

            private Queue<FPSync> _controlQueue = new Queue<FPSync>();
            [SerializeField] private List<int> _watchQueue = new List<int>();

            private void OnPlayerEnteredThreshold(EventData evData)
            {
                if(evData.Code == Constants.PlayerEnteredThresholdEventCode)
                {
                    object[] data = (object[])evData.CustomData;
                    int playerId = (int)data[0];
                    double time = (double)data[1];

                    FPSync sync = new FPSync(playerId, time);
                    Queue<int> aux = Stt_TADHelpers.ToQueue(_watchQueue);

                    if (_controlQueue.Count == 0)
                    {
                        // Give camera control to this player
                        GiveCamera(playerId);

                        _controlQueue.Enqueue(sync);
                        aux.Enqueue(sync.playerId);
                    }
                    else
                    {
                        _controlQueue.Enqueue(sync);

                        // Elements could have come out of order through the network.
                        _controlQueue = Stt_TADHelpers.Order(_controlQueue);
                        aux.Clear();
                        foreach (FPSync s in _controlQueue)
                        {
                            aux.Enqueue(s.playerId);
                        }

                        if (_controlQueue.Peek().playerId == playerId)
                        {
                            GiveCamera(playerId);
                        }
                    }

                    _watchQueue = aux.ToList();

                    Debug.Log("Is watch queue the same as control queue? :" + TestWatchQueue(_watchQueue, _controlQueue));
                }
            }

            private void OnPlayerLeftThreshold(EventData evData)
            {
                if (evData.Code == Constants.PlayerLeftThresholdEventCode)
                {
                    object[] data = (object[])evData.CustomData;
                    int playerId = (int)data[0];
                    double time = (double)data[1];

                    FPSync sync = new FPSync(playerId, time);
                    Queue<int> aux = Stt_TADHelpers.ToQueue(_watchQueue);

                    if (sync.Equals(_controlQueue.Peek()))
                    {
                        _controlQueue.Dequeue();
                        aux.Dequeue();

                        // Give camera control to the next player in the queue
                        if (_controlQueue.Count == 0)
                        {
                            // No players left in the queue, loses control of camera to the "scene"
                            RegainCamera();
                        }
                        else
                        {
                            GiveCamera(_controlQueue.Peek().playerId);
                        }
                    }
                    else
                    {
                        // Look for player id in the queue and remove it
                        _controlQueue = Stt_TADHelpers.Remove(_controlQueue, sync);
                        foreach (FPSync s in _controlQueue)
                        {
                            aux.Enqueue(s.playerId);
                        }
                    }

                    _watchQueue = aux.ToList();

                    Debug.Log("Is watch queue the same as control queue? :" + TestWatchQueue(_watchQueue, _controlQueue));
                }
            }

            private bool TestWatchQueue(List<int> wQ, Queue<FPSync> q)
            {
                FPSync[] auxQ = q.ToArray();
                int[] auxWQ = wQ.ToArray();

                for(int i = 0; i < 4; i++)
                {
                    if(auxQ[i].playerId != auxWQ[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}


