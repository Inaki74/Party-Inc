using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_Booster_LL : MonoBehaviour
        {
            [SerializeField] private float _speedBoost;
            [SerializeField] private float _duration;

            private void OnTriggerEnter(Collider other)
            {
                if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<Mono_Player_Controller_LL>().photonView.IsMine)
                {
                    Rigidbody theirRb = other.gameObject.GetComponent<Rigidbody>();
                    Mono_Player_Controller_LL theirController = other.gameObject.GetComponent<Mono_Player_Controller_LL>();
                    theirRb.velocity = theirRb.velocity + Vector3.right * _speedBoost;
                    theirController.Boosted = true;
                    theirController.ResetBoost(_duration, _speedBoost);
                }
            }
        }
    }
}



