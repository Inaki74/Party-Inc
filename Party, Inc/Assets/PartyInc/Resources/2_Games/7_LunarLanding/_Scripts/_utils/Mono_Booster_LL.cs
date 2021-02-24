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

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            private void OnTriggerEnter(Collider other)
            {
                if(other.gameObject.tag == "Player")
                {
                    Rigidbody theirRb = other.gameObject.GetComponent<Rigidbody>();
                    Mono_Player_Controller_LL theirController = other.gameObject.GetComponent<Mono_Player_Controller_LL>();
                    theirRb.velocity = theirRb.velocity + Vector3.right * _speedBoost;
                    theirController.Boosted = true;
                    StartCoroutine(ResetBoost(theirRb, theirController));
                }
            }

            private IEnumerator ResetBoost(Rigidbody theirRb, Mono_Player_Controller_LL theirController)
            {
                yield return new WaitForSeconds(_duration);

                theirRb.velocity = theirRb.velocity - Vector3.right * _speedBoost;
                theirController.Boosted = false;
            }
        }
    }
}



