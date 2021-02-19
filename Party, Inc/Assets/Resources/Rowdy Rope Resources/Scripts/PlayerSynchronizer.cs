using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace PlayInc
{
    namespace RR
    {
        public struct PlayerSync
        {
            public Vector3 position;
            public Vector3 velocity;
            public float drag;
            public Quaternion rotation;
            public Vector3 angVelocity;
        }

        [RequireComponent(typeof(Player))]
        public class PlayerSynchronizer : SnapshotInterpolator<PlayerSync>
        {
            [SerializeField]
            private Player _player;

            // Start is called before the first frame update
            void Start()
            {
                if(_player == null)
                {
                    _player = GetComponent<Player>();
                }
            }

            protected override void Extrapolate(State newest, float extrapTime)
            {
                PlayerSync newInfo = newest.info;
                transform.position = newInfo.position + extrapTime * newInfo.velocity;

                _player.Rb.drag = newInfo.drag;

                if (_player.hasLost)
                {
                    transform.rotation = Quaternion.AngleAxis(newInfo.angVelocity.magnitude * extrapTime, newInfo.angVelocity) * newInfo.rotation;
                }
            }

            protected override void Interpolate(State rhs, State lhs, float t)
            {
                PlayerSync rhsInfo = rhs.info;
                PlayerSync lhsInfo = lhs.info;

                transform.position = Vector3.Lerp(lhsInfo.position, rhsInfo.position, t);
                _player.Rb.velocity = Vector3.Lerp(lhsInfo.velocity, rhsInfo.velocity, t);
                _player.Rb.drag = rhsInfo.drag;

                if (_player.hasLost)
                {
                    transform.rotation = Quaternion.Slerp(lhsInfo.rotation, rhsInfo.rotation, t);
                    _player.Rb.angularVelocity = Vector3.Lerp(lhsInfo.angVelocity, rhsInfo.angVelocity, t);
                }
            }

            protected override PlayerSync ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                Vector3 lastVel = (Vector3)stream.ReceiveNext();
                Vector3 lastPos = (Vector3)stream.ReceiveNext();
                float lastDrag = (float)stream.ReceiveNext();

                PlayerSync pInfo;
                if (_player.hasLost)
                {
                    _player.Rb.constraints = RigidbodyConstraints.None;
                    Quaternion lastRot = (Quaternion)stream.ReceiveNext();
                    Vector3 lastAngVel = (Vector3)stream.ReceiveNext();
                    pInfo.rotation = lastRot;
                    pInfo.angVelocity = lastAngVel;
                }
                else
                {
                    pInfo.rotation = Quaternion.identity;
                    pInfo.angVelocity = Vector3.zero;
                }

                pInfo.position = lastPos;
                pInfo.velocity = lastVel;
                pInfo.drag = lastDrag;

                return pInfo;
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                stream.SendNext(_player.Rb.velocity);
                stream.SendNext(transform.position);
                stream.SendNext(_player.Rb.drag);
                if (_player.hasLost)
                {
                    stream.SendNext(transform.rotation);
                    stream.SendNext(_player.Rb.angularVelocity);
                } 
            }
        }
    }
}


