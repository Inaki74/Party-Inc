using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public struct PlayerInfo
        {
            public Vector3 position;
            public Vector3 rbVelocity;
        }

        public class Mono_Player_Synchronizer_KK : MonoInt_SnapshotInterpolator<PlayerInfo>
        {
            [SerializeField] private Mono_Player_Controller_KK _player;

            protected override void Extrapolate(State newest, float extrapTime)
            {
                PlayerInfo newestInfo = newest.info;

                transform.position = newestInfo.position + newestInfo.rbVelocity * extrapTime;
            }

            protected override void Interpolate(State rhs, State lhs, float t)
            {
                PlayerInfo rhsInfo = rhs.info;
                PlayerInfo lhsInfo = lhs.info;

                transform.position = Vector3.Lerp(lhsInfo.position, rhsInfo.position, t);
                _player.Rb.velocity = Vector3.Lerp(lhsInfo.rbVelocity, rhsInfo.rbVelocity, t);
            }

            protected override PlayerInfo ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                Vector3 _netPos = (Vector3)stream.ReceiveNext();
                Vector3 _netVel = (Vector3)stream.ReceiveNext();

                PlayerInfo ret;
                ret.position = _netPos;
                ret.rbVelocity = _netVel;

                return ret;
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                stream.SendNext(transform.position);
                stream.SendNext(_player.Rb.velocity);
            }
        }
    }
}
