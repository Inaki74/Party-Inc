using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PartyInc
{
    namespace LL
    {
        public class Mono_ObstacleDeathCount_LL : MonoBehaviour
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
                Mono_ObstaclePassCheck_LL.onPlayerPassed += PlayerPassed;

                _currentDeathCount = 6;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_LL.onPlayerPassed -= PlayerPassed;
            }

            private void PlayerPassed()
            {
                _currentDeathCount--;

                if(_currentDeathCount <= 0f)
                {
                    onObstacleDied.Invoke();
                    Destroy(gameObject);
                }
            }
        }
    }
}

