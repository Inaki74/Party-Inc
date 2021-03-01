using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_ProceduralGenerator_KK : MonoSingleton<Mono_ProceduralGenerator_KK>
        {
            [SerializeField] private GameObject _obstacle;

            private GameObject _newest;

            private float _x = 14f;

            // Start is called before the first frame update
            void Start()
            {
                StartCoroutine(WaitForSeed());
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstacleDeathCount_KK.onObstacleDied += SpawnObstacle;
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
                    SpawnObstacleBase(i + 5);
                }
            }

            private void SpawnObstacle()
            {
                float y = Random.Range(-3f, 4f);
                _newest = Instantiate(_obstacle, new Vector3(_x, y, Mng_GameManager_KK.Current.MyPlayerZ), Quaternion.identity);
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
