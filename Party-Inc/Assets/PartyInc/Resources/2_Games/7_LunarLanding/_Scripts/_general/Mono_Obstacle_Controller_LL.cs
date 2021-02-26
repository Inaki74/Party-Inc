using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace LL
    {
        public struct ObstacleSync
        {
            public Vector3 position;
            public float speed;
        }

        public class Mono_Obstacle_Controller_LL : MonoInt_SnapshotInterpolator<ObstacleSync>
        {
            private void Start()
            {
                // Always extrapolate
                _interpolationBackTime = 0f;
            }

            protected override void OnDestroyOv()
            {
                Debug.Log("Mono_Obstacle_Controller_LL");
            }

            protected override void Extrapolate(State newest, float extrapTime)
            {
                ObstacleSync newestInfo = newest.info;

                transform.position = newestInfo.position + Vector3.left * newestInfo.speed * extrapTime;
            }

            protected override void Interpolate(State rhs, State lhs, float t)
            {
                ObstacleSync rhsInfo = rhs.info;
                ObstacleSync lhsInfo = lhs.info;

                transform.position = Vector3.Lerp(lhsInfo.position, rhsInfo.position, t);
            }

            protected override ObstacleSync ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                Vector3 netPos = (Vector3)stream.ReceiveNext();
                float netSpeed = (float)stream.ReceiveNext();

                ObstacleSync ret;
                ret.position = netPos;
                ret.speed = netSpeed;

                return ret;
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                stream.SendNext(transform.position);
                stream.SendNext(Mng_GameManager_LL.Current.MovementSpeed);
            }

            //// Update is called once per frame
            protected override void UpdateOv()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    transform.position += Vector3.left * Mng_GameManager_LL.Current.MovementSpeed * Time.deltaTime;
                }
            }
        }
    }
}


