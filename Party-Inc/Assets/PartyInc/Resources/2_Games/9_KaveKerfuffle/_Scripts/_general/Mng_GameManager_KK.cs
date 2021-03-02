using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mng_GameManager_KK : MonoSingleton<Mng_GameManager_KK>
        {
            [SerializeField] private float _movementSpeed;
            public float MovementSpeed
            {
                get
                {
                    return _movementSpeed;
                }
                private set
                {
                    _movementSpeed = value;
                }
            }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}


