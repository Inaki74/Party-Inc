using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace PartyInc
{
    namespace SS
    {
        public struct PlayerSync
        {
            public Vector3 position;
            public Vector3 velocity;
            public bool duck;
        }

        [RequireComponent(typeof(Mono_Player_Controller_SS))]
        public class Mono_Player_Synchronizer_SS : MonoInt_SnapshotInterpolator<PlayerSync>
        {
            [SerializeField]
            private Mono_Player_Controller_SS _player;

            protected override void Extrapolate(State newest, float extrapTime)
            {
                PlayerSync newInfo = newest.info;

                transform.position = newInfo.position + newInfo.velocity * extrapTime;

                _player.UpperBody.SetActive(newInfo.duck);
            }

            protected override void Interpolate(State rhs, State lhs, float t)
            {
                PlayerSync rhsInfo = rhs.info;
                PlayerSync lhsInfo = lhs.info;

                transform.position = Vector3.Lerp(lhsInfo.position, rhsInfo.position, t);

                _player.UpperBody.SetActive(rhsInfo.duck);
            }

            protected override PlayerSync ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                Vector3 _netPos = (Vector3)stream.ReceiveNext();
                Vector3 _netVel = (Vector3)stream.ReceiveNext();
                bool _netDuck = (bool)stream.ReceiveNext();
                
                PlayerSync ret;
                ret.position = _netPos;
                ret.velocity = _netVel;
                ret.duck = _netDuck;

                return ret;
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                stream.SendNext(transform.position);
                stream.SendNext(_player.Direction);
                stream.SendNext(_player.UpperBody.activeInHierarchy);
            }

            // Start is called before the first frame update
            private void Awake()
            {
                if(_player == null)
                {
                    _player = GetComponent<Mono_Player_Controller_SS>();
                }
            }
        }
    }
}


