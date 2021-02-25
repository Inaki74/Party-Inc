using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    namespace LL
    {
        public struct CameraInfo
        {
            public Vector3 position;
            public Vector3 rbVelocity;
        }

        public class Mono_Camera_Synchronizer_LL : MonoBehaviourPun
        {
            [SerializeField] private Rigidbody _rb;

            [SerializeField] private Transform _irrelevantPoint;
            public Transform IrrelevantPoint
            {
                get
                {
                    return _irrelevantPoint;
                }
                private set
                {
                    _irrelevantPoint = value;
                }
            }

            [SerializeField] private Transform _irrelevantPointTwo;
            public Transform IrrelevantPointTwo
            {
                get
                {
                    return _irrelevantPointTwo;
                }
                private set
                {
                    _irrelevantPointTwo = value;
                }
            }

            public bool ChangingOwner { get; set; }

            private void Start()
            {
                //if (PhotonNetwork.IsMasterClient) photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            }

            //private void OnEnable()
            //{
            //    PhotonNetwork.AddCallbackTarget(this);
            //}

            //private void OnDisable()
            //{
            //    PhotonNetwork.RemoveCallbackTarget(this);
            //}

            //protected override void Extrapolate(State newest, float extrapTime)
            //{
            //    CameraInfo newestInfo = newest.info;

            //    transform.position = newestInfo.position + newestInfo.rbVelocity * extrapTime;
            //}

            //protected override void Interpolate(State rhs, State lhs, float t)
            //{
            //    CameraInfo rhsInfo = rhs.info;
            //    CameraInfo lhsInfo = lhs.info;

            //    transform.position = Vector3.Lerp(lhsInfo.position, rhsInfo.position, t);
            //    _rb.velocity = Vector3.Lerp(lhsInfo.rbVelocity, rhsInfo.rbVelocity, t);
            //}

            //protected override CameraInfo ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            //{
            //    Vector3 netPos = (Vector3)stream.ReceiveNext();
            //    Vector3 netVel = (Vector3)stream.ReceiveNext();

            //    CameraInfo cameraInfo;
            //    cameraInfo.position = netPos;
            //    cameraInfo.rbVelocity = netVel;

            //    return cameraInfo;
            //}

            //protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            //{
            //    stream.SendNext(transform.position);
            //    stream.SendNext(_rb.velocity);
            //}

            //public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
            //{
            //    //ChangingOwner = true;
            //}

            //public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
            //{
            //    Debug.Log("Transferred");
            //    ChangingOwner = false;
            //}
        }
    }
}



