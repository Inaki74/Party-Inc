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
                if(other.tag == "Player")
                {
                    other.transform.position = _exitPortalPosition.transform.position;
                }
            }
        }
    }
}


