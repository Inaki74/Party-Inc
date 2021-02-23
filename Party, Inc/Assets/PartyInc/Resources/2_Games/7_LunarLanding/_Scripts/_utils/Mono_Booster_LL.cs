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
                    theirRb.velocity = theirRb.velocity + Vector3.right * _speedBoost;
                    StartCoroutine(ResetBoost(theirRb));
                }
            }

            private IEnumerator ResetBoost(Rigidbody theirRb)
            {
                yield return new WaitForSeconds(_duration);

                theirRb.velocity = theirRb.velocity - Vector3.right * _speedBoost;
            }
        }
    }
}



