using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        /// <summary>
        /// The class responsible for each Tile in the game.
        /// Its also the one responsible for placing the obstacles within its boundaries. It will generate the randomness behind this.
        /// Obstacles are: 0 nothing, 1 wall, 2 jump, 3 duck. There can never be 3 wall obstacles. More restrictions later.
        /// They are originated from a pool. So their life span is:
        ///     - Created, the first 15 or so tiles must respect some standard.
        ///     - Disabled, when disabled they should be reset.
        ///     - Enabled, when enabled they should rerandomize all of their settings.
        ///
        /// Their settings include: the line in which obstacles are generated (30% - 70% of the tiles length), which obstacles are generated.
        /// </summary>
        public class Tile : MonoBehaviour
        {
            [SerializeField] private GameObject _middleObstaclePoint;

            private int[] _obstacles = new int[3];
            private float _obstacleZAlignment;
            private int _player;
            
            // Start is called before the first frame update
            void Start()
            {
                // First settings

                // We set the obstacle alignment randomly between -3 and 3
                _obstacleZAlignment = Random.Range(-3, 4);

                DecideObstacles();

                // We place the obstacles
                PlaceObstacles();
            }

            private void OnEnable()
            {
                //Re randomize

                // We set the obstacle alignment randomly between -3 and 3
                _obstacleZAlignment = Random.Range(-3, 4);

                DecideObstacles();

                // We place the obstacles
                PlaceObstacles();
            }

            private void OnDisable()
            {
                // Reset
            }

            // Update is called once per frame
            void Update()
            {
                // Dont think ill use it.
            }

            private void DecideObstacles()
            {
                // First version, very simple.

                // We decide which obstacles we will place
                // First we set them randomly.
                RandomizeObstacles();

                // Then we make sure it meets standards.
                bool passes = false;

                while (!passes)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (_obstacles[i] == 1)
                        {
                            passes = passes || false;
                        }
                        else
                        {
                            passes = true;
                            continue;
                        }
                    }

                    // Rerandomize
                    RandomizeObstacles();
                }
            }

            private void RandomizeObstacles()
            {
                for (int i = 0; i < 3; i++)
                {
                    _obstacles[i] = Random.Range(0, 4);
                }
            }

            private void PlaceObstacles()
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 displacement = Vector3.zero;
                    if (i == 0)
                    {
                        displacement = new Vector3(-3f, 0f, 0f);
                    }
                    if (i == 2)
                    {
                        displacement = new Vector3(3f, 0f, 0f);
                    }
                    //Ask for obstacles
                    // GameObject obstacle = obstaclePool.GetObstacle(_obstacles[i]);
                    // Make obstacle child of Tile
                    // obstacle.localPosition = _middleObstaclePoint.localPosistion + displacement + new Vector3(0f, 0f, _obstacleZAlignment);
                }
            }
        }
    }
}


