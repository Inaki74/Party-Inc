using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace SS
    {
        public class Mono_PortalController_SS : MonoBehaviour
        {
            [SerializeField] private GameObject _exitPortalPosition;


            private void OnTriggerEnter(Collider other)
            {
                if(other.tag == "Player")
                {
                    other.transform.position = _exitPortalPosition.transform.position;
                }
            }
        }
    }
}


