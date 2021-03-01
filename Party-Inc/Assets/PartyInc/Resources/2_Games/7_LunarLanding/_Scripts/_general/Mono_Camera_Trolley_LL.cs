using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_Camera_Trolley_LL : MonoBehaviour
        {
            [SerializeField] private Mono_Camera_Synchronizer_LL _sync;
            [SerializeField] private Rigidbody _rb;
            public float Extra { get; set; }

            // Start is called before the first frame update
            void Start()
            {
                
            }

            // Update is called once per frame
            void FixedUpdate()
            {
                if (_sync.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber || !PhotonNetwork.IsConnected)
                {
                    // It belongs to this player, therefore its pushing it across the screen
                    _rb.velocity = Vector3.right * (Mng_GameManager_LL.Current.MovementSpeed + Extra);
                }
            }
        }
    }
}


