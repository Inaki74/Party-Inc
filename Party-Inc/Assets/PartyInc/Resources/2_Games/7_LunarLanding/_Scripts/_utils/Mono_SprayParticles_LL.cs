using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_SprayParticles_LL : MonoBehaviour
        {
            [SerializeField] private ParticleSystem _pSys;

            // Update is called once per frame
            void Update()
            {
                if (!_pSys.isEmitting)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}


