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

            public float NextObstacleY { get; private set; }

            private float _x = 14f;
            public const float XDifference = 7f;
            public int BoosterCount { get; private set; } = 3;

            // Start is called before the first frame update
            void Start()
            {
                NextObstacleY = Random.Range(-3f, 3f);
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
                BoosterCount--;
                float y = NextObstacleY;
                NextObstacleY = Random.Range(-3f, 3f);
                _newest = Instantiate(_obstacle, new Vector3(_x, y, Mng_GameManager_LL.Current.MyPlayerZ), Quaternion.identity);
                _x += XDifference;
                if(BoosterCount == 0)
                {
                    BoosterCount = 3;
                }
            }
        }
    }
}



