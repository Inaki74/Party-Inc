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
            [SerializeField] private GameObject _primaryObstacle;
            [SerializeField] private GameObject[] _secondaryObstacles;

            private void Start()
            {
                // Always extrapolate
                _interpolationBackTime = 0f;
                StartCoroutine(WaitForPlayerZ());
            }

            protected override void Extrapolate(State newest, float extrapTime)
            {
                ObstacleSync newestInfo = newest.info;

                Vector3 futurePos = newestInfo.position + Vector3.left * newestInfo.speed * extrapTime;

                transform.position = futurePos;
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

            private void OnCollisionEnter(Collision collision)
            {
                //if (!photonView.IsMine) return;

                if(collision.gameObject.tag == "Player")
                {
                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

                    rb.velocity = new Vector3(-Mng_GameManager_LL.Current.MovementSpeed, rb.velocity.y, 0f);

                    collision.gameObject.GetComponent<Mono_Player_Synchronizer_LL>().InformOfCollision(true);
                }
            }

            private void OnCollisionStay(Collision collision)
            {
                //if (!photonView.IsMine) return;

                if (collision.gameObject.tag == "Player")
                {
                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

                    rb.velocity = new Vector3(-Mng_GameManager_LL.Current.MovementSpeed, rb.velocity.y, 0f);

                    collision.gameObject.GetComponent<Mono_Player_Synchronizer_LL>().InformOfCollision(true);
                }
            }

            private void OnCollisionExit(Collision collision)
            {
                //if (!photonView.IsMine) return;

                if (collision.gameObject.tag == "Player")
                {
                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

                    rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

                    collision.gameObject.GetComponent<Mono_Player_Synchronizer_LL>().InformOfCollision(false);
                }
            }

            private IEnumerator WaitForPlayerZ()
            {
                yield return new WaitUntil(() => Mng_GameManager_LL.Current.MyPlayerZ != -1f);

                _primaryObstacle.transform.position = new Vector3(transform.position.x, transform.position.y, Mng_GameManager_LL.Current.MyPlayerZ);

                foreach (GameObject obst in _secondaryObstacles)
                {
                    if (Mng_GameManager_LL.Current.MyPlayerZ == obst.transform.position.z)
                    {
                        obst.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}


