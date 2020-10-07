using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace EGG
    {
        /// <summary>
        /// The actual spawner, in charge of spawning the eggs.
        /// </summary>
        public class EggSpawner : MonoBehaviourPun
        {
            //We might need to synchronize spawners with photon with IPunObservable

            private bool OK;
            private List<int> eggMapCol = new List<int>();
            private Vector3 spawningPosition;
            private EggPoolManager pool;
            private float waveInterval;

            #region Unity Callbacks
            void Start()
            {
                pool = FindObjectOfType<EggPoolManager>();
                spawningPosition = transform.position;
                OK = true;
                StartCoroutine("WaitForOKCo");
            }

            private void Update()
            {
                // If there are no more eggs available
                if (eggMapCol.Count == 0)
                {
                    // Ask if the game is not finished
                    if (!GameManager.Current.isGameFinished)
                    {
                        // Wait for the OK of the manager
                        StartCoroutine("WaitForOKCo");
                    }
                }
            }

            private void Awake()
            {
                GameManager.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                GameManager.onGameFinish -= OnGameFinish;
            }
            #endregion

            /// <summary>
            /// Co routine that waits for the manager OK.
            /// </summary>
            /// <returns></returns>
            private IEnumerator WaitForOKCo()
            {
                while (!OK)
                {
                    yield return new WaitForEndOfFrame();
                }

                StartCoroutine("SpawningCo");

                OK = false;
            }

            /// <summary>
            /// The Spawning Co Routine
            /// </summary>
            /// <returns></returns>
            private IEnumerator SpawningCo()
            {
                foreach (int egg in eggMapCol)
                {
                    SpawnEgg(egg);
                    yield return new WaitForSeconds(waveInterval);
                }
                eggMapCol.Clear();
            }

            /// <summary>
            /// Spawns egg of type t, 0 for normal, 1 for Rotten, 2 for golden, -1 for void.
            /// </summary>
            /// <param name="t">Type of egg.</param>
            private void SpawnEgg(int t)
            {
                if (t == -1) return;

                if (pool == null)
                {
                    pool = FindObjectOfType<EggPoolManager>();
                }

                GameObject egg = pool.RequestEgg(t);
                egg.transform.position = spawningPosition;
            }

            /// <summary>
            /// Sets the intervals between each egg spawn.
            /// </summary>
            /// <param name="interval"></param>
            public void SetIntervals(float interval)
            {
                waveInterval = interval;
            }

            /// <summary>
            /// Sets the egg spawning routine.
            /// </summary>
            /// <param name="routine"></param>
            public void SetRoutine(List<int> routine)
            {
                OK = true;
                eggMapCol = routine;
            }

            public void OnGameFinish()
            {
                StopAllCoroutines();
            }
        }
    }
}
