using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        public class GameManager : FiestaGameManager<GameManager, float>
        {
            [SerializeField] private float _movingSpeed;
            public float MovingSpeed {
                get
                {
                    return _movingSpeed;
                }
                private set
                {
                    _movingSpeed = value;
                }
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                //throw new System.NotImplementedException();
            }

            protected override void InStart()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}