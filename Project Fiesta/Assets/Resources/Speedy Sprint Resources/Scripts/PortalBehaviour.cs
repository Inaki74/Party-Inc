using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        public class PortalBehaviour : MonoBehaviour
        {
            [SerializeField] private GameObject _exitPortalPosition;


            private void OnTriggerEnter(Collider other)
            {
                Debug.Log("HIT PORTAL");

                if(other.tag == "Player")
                {
                    Debug.Log("PROTAL: Teleporting Player.");
                    other.transform.position = _exitPortalPosition.transform.position;
                }
            }
        }
    }
}


