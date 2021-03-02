using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_ProceduralGenerator_KK : MonoSingleton<Mono_ProceduralGenerator_KK>
        {
            private GameObject _newest;

            private float _x = 14f;

            private const float _startingSeparation = 16.21f;
            private const float _finalSeparation = 12.7f;
            private const float _timeToReachFinalSeparationInSeconds = 30f;

            private float _currentSeparation;

            // Start is called before the first frame update
            void Start()
            {
                _currentSeparation = _startingSeparation;
                StartCoroutine(WaitForSeed());
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstacleDeathCount_KK.onObstacleDied += SpawnObstacle;
            }

            private void Update()
            {
                if(_currentSeparation > _finalSeparation)
                {
                    _currentSeparation = _startingSeparation + Mng_GameManager_KK.Current.InGameTime * ((_finalSeparation - _startingSeparation) / _timeToReachFinalSeparationInSeconds);
                }
            }

            private void OnDestroy()
            {
                Mono_ObstacleDeathCount_KK.onObstacleDied -= SpawnObstacle;
            }

            private void SpawnObstacleBase(int deathCount)
            {
                SpawnObstacle();
                _newest.GetComponent<Mono_ObstacleDeathCount_KK>().CurrentDeathCount = deathCount;
            }

            private IEnumerator ObstaclesBaseCase()
            {
                yield return new WaitUntil(() => Mng_GameManager_KK.Current.MyPlayerZ != -1);

                for (int i = 0; i < 6f; i++)
                {
                    SpawnObstacleBase(i + 6);
                }
            }

            private void SpawnObstacle()
            {
                float y = Random.Range(-3f, 3f);
                _newest = Mng_ObstaclePoolManager_KK.Current.RequestObstacle();
                _newest.transform.position = new Vector3(_x, y, Mng_GameManager_KK.Current.MyPlayerZ);

                // We do it with starting separation since the obstacles spawn with the starting separation.

                if(_currentSeparation <= _finalSeparation)
                {
                    _currentSeparation = _finalSeparation;
                }

                float randSep = Random.Range(_startingSeparation, _currentSeparation);
                float diff = (_startingSeparation - randSep)/2;

                _newest.GetComponent<Mono_Obstacle_KK>().SetObstaclesOpening(diff);
                _x += 7f;
            }

            private IEnumerator WaitForSeed()
            {
                yield return new WaitUntil(() => Mng_GameManager_KK.Current.GotSeed);
                
                StartCoroutine(ObstaclesBaseCase());
            }
        }
    }
}
