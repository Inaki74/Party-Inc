using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace PartyInc
{
    namespace LL
    {
        public struct FPSync
        {
            public Vector3 position;
            public Vector3 rbVelocity;
        }

        public class Mono_FakePlayer_LL : MonoInt_SnapshotInterpolator<FPSync>
        {
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private Mono_Camera_Synchronizer_LL _trolleySync;
            [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;

            private Mono_Player_Controller_LL[] _allPlayerControllers;

            private Transform _playerThatHasCamera;
            private float _originalX;
            private bool _lostCamera;

            private void Start()
            {
                _allPlayerControllers = FindObjectsOfType<Mono_Player_Controller_LL>();

                if (PhotonNetwork.IsMasterClient) photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            }

            // Update is called once per frame
            void FixedUpdate()
            {
                if (!photonView.IsMine) return;

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

            //public void LoseCamera(Transform player)
            //{
            //    _playerThatHasCamera = player;
            //    _lostCamera = true;
            //    photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            //}

            //public void RegainCamera()
            //{
            //    //_camera.Follow = transform;
            //    _playerThatHasCamera = null;
            //    _lostCamera = false;
            //    transform.position = Vector3.right * _originalX;//Vector3.Lerp(transform.position, Vector3.right * _originalX, Time.deltaTime * 10f);
            //    //_trolleySync.ChangingOwner = true;
            //}

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
                photonView.TransferOwnership(playerId);
            }

            private Queue<int> _controlQueue = new Queue<int>();

            private void OnPlayerEnteredThreshold(int playerId)
            {
                if(_controlQueue.Count == 0)
                {
                    // Give camera control to this player

                }

                _controlQueue.Enqueue(playerId);
            }

            private void OnPlayerLeftThreshold(int playerId)
            {
                if(playerId == _controlQueue.Peek())
                {
                    _controlQueue.Dequeue();

                    // Give camera control to the next player in the queue
                    if (_controlQueue.Count == 0)
                    {
                        // No players left in the queue, last player keeps Ownership, but loses control of camera to the "scene"

                    }
                    else
                    {

                    }
                }
                else
                {
                    // Look for player id in the queue and remove it
                }

            }










            /// SYNC MOVEMENTS///////
            /// 
            protected override void Interpolate(State rhs, State lhs, float t)
            {
                FPSync rhsInfo = rhs.info;
                FPSync lhsInfo = lhs.info;

                transform.position = Vector3.Lerp(lhsInfo.position, rhsInfo.position, t);
                _rb.velocity = Vector3.Lerp(lhsInfo.rbVelocity, rhsInfo.rbVelocity, t);
            }

            protected override void Extrapolate(State newest, float extrapTime)
            {
                FPSync newestInfo = newest.info;

                transform.position = newestInfo.position + newestInfo.rbVelocity * extrapTime;
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                stream.SendNext(transform.position);
                stream.SendNext(_rb.velocity);
            }

            protected override FPSync ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                Vector3 netPos = (Vector3)stream.ReceiveNext();
                Vector3 netVel = (Vector3)stream.ReceiveNext();

                FPSync fpInfo;
                fpInfo.position = netPos;
                fpInfo.rbVelocity = netVel;

                return fpInfo;
            }
        }
    }
}


