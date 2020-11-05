using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        /// <summary>
        /// DEPRECATED
        /// </summary>
        public class ObstaclePoolManager : MonoSingleton<ObstaclePoolManager>
        {
            [SerializeField] private Transform _obstacleHolder;
            public Transform ObstacleHolder
            {
                get
                {
                    return _obstacleHolder;
                }

                private set
                {
                    _obstacleHolder = value;
                }
            }

            [SerializeField] private GameObject _jumpObstaclePrefab;
            [SerializeField] private GameObject _duckObstaclePrefab;
            [SerializeField] private GameObject _wallObstaclePrefab;

            private List<GameObject> _wObstacles = new List<GameObject>();
            private List<GameObject> _jObstacles = new List<GameObject>();
            private List<GameObject> _dObstacles = new List<GameObject>();
            // Start is called before the first frame update
            void Start()
            {
                for (int i = 0; i < 10; i++)
                {
                    GenerateObstacle(1);
                    GenerateObstacle(2);
                    GenerateObstacle(3);
                }
            }

            private void GenerateObstacle(int type)
            {
                //PhotonNetwork.Instantiate(_tilePrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);

                switch (type)
                {
                    case 1:
                        GameObject newWObstacle = Instantiate(_wallObstaclePrefab);
                        newWObstacle.transform.parent = _obstacleHolder.transform;
                        newWObstacle.SetActive(false);
                        _wObstacles.Add(newWObstacle);
                        break;
                    case 2:
                        GameObject newJObstacle = Instantiate(_jumpObstaclePrefab);
                        newJObstacle.transform.parent = _obstacleHolder.transform;
                        newJObstacle.SetActive(false);
                        _jObstacles.Add(newJObstacle);
                        break;
                    case 3:
                        GameObject newDObstacle = Instantiate(_duckObstaclePrefab);
                        newDObstacle.transform.parent = _obstacleHolder.transform;
                        newDObstacle.SetActive(false);
                        _dObstacles.Add(newDObstacle);
                        break;
                }
            }

            public GameObject RequestObstacle(int type)
            {
                switch (type)
                {
                    case 1:
                        //Get wall obstacle
                        foreach (GameObject obs in _wObstacles)
                        {
                            if (!obs.activeInHierarchy)
                            {
                                obs.SetActive(true);
                                return obs;
                            }
                        }
                        break;
                    case 2:
                        //Get jump obstacle
                        foreach (GameObject obs in _jObstacles)
                        {
                            if (!obs.activeInHierarchy)
                            {
                                obs.SetActive(true);
                                return obs;
                            }
                        }
                        break;
                    case 3:
                        //Get duck obstacle
                        foreach (GameObject obs in _dObstacles)
                        {
                            if (!obs.activeInHierarchy)
                            {
                                obs.SetActive(true);
                                return obs;
                            }
                        }
                        break;
                }

                GenerateObstacle(type);
                return RequestObstacle(type);
            }
        }
    }
}
