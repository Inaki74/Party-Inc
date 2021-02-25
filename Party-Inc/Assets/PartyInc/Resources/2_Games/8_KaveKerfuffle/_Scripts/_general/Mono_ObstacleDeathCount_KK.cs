using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PartyInc
{
    namespace KK
    {
        public class Mono_ObstacleDeathCount_KK : MonoBehaviour
        {
            public delegate void ActionObstacleDeath();
            public static event ActionObstacleDeath onObstacleDied;

            private int _currentDeathCount;

            [SerializeField] private int _deathCount;
            public int CurrentDeathCount
            {
                get
                {
                    return _currentDeathCount;
                }
                set
                {
                    _currentDeathCount = value;
                }
            }

            private void Start()
            {

            }

            private void Awake()
            {
                Mono_ObstaclePassCheck_KK.onGateRendered += GateRendered;

                _currentDeathCount = 6;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_KK.onGateRendered -= GateRendered;
            }

            private void GateRendered()
            {
                _currentDeathCount--;

                if (_currentDeathCount <= 0f)
                {
                    onObstacleDied.Invoke();
                    Destroy(gameObject);
                }
            }
        }
    }
}
