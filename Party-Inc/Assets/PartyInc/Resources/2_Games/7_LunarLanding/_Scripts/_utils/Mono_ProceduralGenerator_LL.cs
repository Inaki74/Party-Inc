using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ProceduralGenerator_LL : MonoSingleton<Mono_ProceduralGenerator_LL>
        {
            [SerializeField] private Mono_Camera_Synchronizer_LL _sync;
            [SerializeField] private GameObject _obstacle;

            private GameObject _newest;

            public float NextObstacleY { get; private set; }

            private float _x = 14f;
            public const float XDifference = 7f;
            private int _boosterCount = 3;

            // Start is called before the first frame update
            void Start()
            {
                StartCoroutine(WaitForSeed());
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
                _boosterCount--;
                float y = NextObstacleY;
                NextObstacleY = 0;// Random.Range(-3f, 3f);
                _newest = Instantiate(_obstacle, new Vector3(_x, y, Mng_GameManager_LL.Current.MyPlayerZ), Quaternion.identity);
                _newest.GetComponent<Mono_ObstacleBoosterPlacement_LL>().PlaceBooster(_boosterCount);
                _x += XDifference;
                if (_boosterCount == 0)
                {
                    _boosterCount = 3;
                }
                _newest.GetComponent<Mono_ObstacleDeathCount_LL>().CurrentDeathCount = deathCount;
            }

            private IEnumerator ObstaclesBaseCase()
            {
                yield return new WaitUntil(() => Mng_GameManager_LL.Current.MyPlayerZ != -1);

                for (int i = 0; i < 6f; i++)
                {
                    SpawnObstacleBase(i + 6);
                }
                //_x = 16.969f;
            }

            private void SpawnObstacle()
            {
                _boosterCount--;
                float y = NextObstacleY;
                NextObstacleY = 0;// Random.Range(-3f, 3f);
                _newest = Instantiate(_obstacle, new Vector3(_sync.SpawningPoint.transform.position.x, y, Mng_GameManager_LL.Current.MyPlayerZ), Quaternion.identity);
                _newest.GetComponent<Mono_ObstacleBoosterPlacement_LL>().PlaceBooster(_boosterCount);
                if(_boosterCount == 0)
                {
                    _boosterCount = 3;
                }
            }

            private IEnumerator WaitForSeed()
            {
                yield return new WaitUntil(() => Mng_GameManager_LL.Current.GotSeed);

                NextObstacleY = 0;// Random.Range(-3f, 3f);
                StartCoroutine(ObstaclesBaseCase());
            }
        }
    }
}



