using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_SprayParticles_KK : MonoBehaviour
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