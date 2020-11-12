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
        ///
        /// DEPRECATED
        /// </summary>
        public class Tile : MonoBehaviour
        {
            private const int TilesToDisable = 16;

            [SerializeField] private GameObject _middleObstaclePoint;

            private GameObject[] _obstacleGOs = new GameObject[3];

            public int[] Obstacles { get; private set; } = new int[3];
            public float ObstacleZAlignment { get; private set; }

            private int _tileCount = 0;
            private int _player;
            
            // Start is called before the first frame update
            void Start()
            {
                // First settings

                Obstacles[0] = 0;
                Obstacles[1] = 0;
                Obstacles[2] = 0;
            }

            private void OnEnable()
            {
                
            }

            private void OnDisable()
            {
                // Reset
                ObstacleZAlignment = 0;
                _tileCount = 0;

                for(int i = 0; i < 3; i++)
                {
                    if(_obstacleGOs[i] != null)
                    {
                        _obstacleGOs[i].transform.parent = ObstaclePoolManager.Current.ObstacleHolder.transform;
                        _obstacleGOs[i].SetActive(false);
                    }
                    
                    Obstacles[i] = 0;
                }
            }

            private void Awake()
            {
                //InvisibleTrolleyController.onPassedTile += OnTilePassed;
            }

            private void OnDestroy()
            {
                //InvisibleTrolleyController.onPassedTile -= OnTilePassed;
            }

            // Check if its time for the tile to be disabled
            private void OnTilePassed()
            {
                _tileCount++;

                if(_tileCount >= TilesToDisable)
                {
                    gameObject.SetActive(false);
                }
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
                        if (Obstacles[i] == 1)
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
                    Obstacles[i] = Random.Range(0, 4);
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
                    if(Obstacles[i] != 0)
                    {
                        GameObject obstacle = ObstaclePoolManager.Current.RequestObstacle(Obstacles[i]);
                        // Make obstacle child of Tile
                        obstacle.transform.parent = transform;
                        obstacle.transform.localPosition = _middleObstaclePoint.transform.localPosition + displacement + new Vector3(0f, 2f, ObstacleZAlignment);
                        _obstacleGOs[i] = obstacle;
                    }
                    else
                    {
                        _obstacleGOs[i] = null;
                    }
                    
                }
            }

            public void PersonalizeTile(int[] obs, float zAlign)
            {
                Obstacles = obs;
                ObstacleZAlignment = zAlign;

                PlaceObstacles();
            }

            public bool ReRandomize()
            {
                //Re randomize

                // We set the obstacle alignment randomly between -3 and 3
                ObstacleZAlignment = Random.Range(-3, 4);

                DecideObstacles();

                // We place the obstacles
                PlaceObstacles();

                return true;
            }

            public void SetTileCount(int set)
            {
                _tileCount = set;
            }
        }
    }
}


