using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_SprayParticles_KK : MonoBehaviourPun, IPunInstantiateMagicCallback
        {
            [SerializeField] private ParticleSystem _pSys;

            public void OnPhotonInstantiate(PhotonMessageInfo info)
            {
                Transform myPlayer = transform;

                foreach(PhotonView pv in FindObjectsOfType<PhotonView>())
                {
                    if(pv.OwnerActorNr == info.Sender.ActorNumber && pv.gameObject.tag == "Player")
                    {
                        if(transform.rotation.Equals(Quaternion.AngleAxis(180f, Vector3.right)))
                        {
                            // bottom
                            myPlayer = pv.GetComponent<Mono_Player_Controller_KK>().Bottom.transform;
                        }
                        else
                        {
                            // top
                            myPlayer = pv.GetComponent<Mono_Player_Controller_KK>().Top.transform;
                        }
                    }
                }

                transform.position = myPlayer.position;

                ParticleSystem.MainModule main = _pSys.main;
                main.duration = Time.deltaTime;

                _pSys.Play();
            }

            // Update is called once per frame
            void Update()
            {
                if (!_pSys.isEmitting && photonView.IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }
}