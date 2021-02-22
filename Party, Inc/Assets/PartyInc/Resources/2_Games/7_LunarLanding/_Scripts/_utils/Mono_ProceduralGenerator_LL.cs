using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ProceduralGenerator_LL : MonoSingleton<Mono_ProceduralGenerator_LL>
        {
            [SerializeField] private GameObject _obstacle;

            private GameObject _newest;

            private float _x = 7f;

            // Start is called before the first frame update
            void Start()
            {
                StartCoroutine(ObstaclesBaseCase());
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstacleDeathCount_LL.onObstacleDied += SpawnObstacle;
            }

            private void OnDestroy()
            {
                Mono_ObstacleDeathCount_LL.onObstacleDied -= SpawnObstacle;
            }

            private void SpawnObstacleBase(int deathCount)
            {
                SpawnObstacle();
                _newest.GetComponent<Mono_ObstacleDeathCount_LL>().CurrentDeathCount = deathCount;
            }

            private IEnumerator ObstaclesBaseCase()
            {
                yield return new WaitUntil(() => Mng_GameManager_LL.Current.MyPlayerZ != -1);

                for (int i = 0; i < 6f; i++)
                {
                    SpawnObstacleBase(i + 6);
                }
            }

            private void SpawnObstacle()
            {
                float y = Random.Range(-3f, 4f);
                _newest = Instantiate(_obstacle, new Vector3(_x, y, Mng_GameManager_LL.Current.MyPlayerZ), Quaternion.identity);
                _x += 7f;
            }
        }
    }
}



