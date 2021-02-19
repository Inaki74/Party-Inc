using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PlayInc
{
    namespace SS
    {
        /// <summary>
        /// DEPRECATED
        /// </summary>
        public class Mng_TilePoolManager_SS : MonoSingleton<Mng_TilePoolManager_SS>
        {
            [SerializeField] private Transform _tileHolder;

            [SerializeField] private GameObject _tilePrefab;

            private List<GameObject> _tiles = new List<GameObject>();
            // Start is called before the first frame update
            void Start()
            {
                for(int i = 0; i < 80; i++)
                {
                    GenerateTile();
                }
            }

            private void GenerateTile()
            {
                //PhotonNetwork.Instantiate(_tilePrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);
                  GameObject newTile = Instantiate(_tilePrefab);
                  newTile.transform.parent = _tileHolder.transform;
                  newTile.SetActive(false);
                  _tiles.Add(newTile);
            }

            public GameObject RequestTile()
            {
                //Get Tile
                foreach (GameObject tile in _tiles)
                {
                    if (!tile.activeInHierarchy)
                    {
                        tile.SetActive(true);
                        return tile;
                    }
                }

                GenerateTile();
                return RequestTile();
            }
        }
    }
}
