using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        public struct PlayerSync
        {

        }

        public class PlayerSynchronizer : SnapshotInterpolator<PlayerSync>
        {
            protected override void Extrapolate(State newest, float extrapTime)
            {
                throw new System.NotImplementedException();
            }

            protected override void Interpolate(State rhs, State lhs, float t)
            {
                throw new System.NotImplementedException();
            }

            protected override PlayerSync ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                throw new System.NotImplementedException();
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                throw new System.NotImplementedException();
            }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}


