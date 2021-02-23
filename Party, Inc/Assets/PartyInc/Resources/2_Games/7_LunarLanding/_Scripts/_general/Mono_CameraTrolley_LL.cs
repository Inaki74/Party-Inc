using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_CameraTrolley_LL : MonoBehaviourPun, IPunObservable
        {
            public float _extra;

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (photonView.Owner != null)
                {
                    if (stream.IsWriting && photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        stream.SendNext(_extra);
                    }
                    else
                    {
                        _extra = (float)stream.ReceiveNext();
                    }
                }
            }

            // Update is called once per frame
            void Update()
            {
                if(photonView.Owner == null)
                {
                    // It belongs to the scene
                    transform.position += Vector3.right * (Mng_GameManager_LL.Current.MovementSpeed) * Time.deltaTime;
                }
                else
                {
                    // It belongs to some player
                    transform.position += Vector3.right * (Mng_GameManager_LL.Current.MovementSpeed + _extra) * Time.deltaTime;
                }
            }
        }
    }
}



