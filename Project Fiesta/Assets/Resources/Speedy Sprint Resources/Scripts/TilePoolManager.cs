using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        public class TilePoolManager : MonoBehaviour
        {
            [SerializeField] private Transform _tileHolder;

            [SerializeField] private GameObject _tilePrefab;

            private List<GameObject> _tiles = new List<GameObject>();
            // Start is called before the first frame update
            void Start()
            {

            }

            private void GenerateTile()
            {
                  GameObject newTile = PhotonNetwork.Instantiate(_tilePrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);
                  newTile.transform.parent = _tileHolder.transform;
                  newTile.SetActive(false);
                  _tiles.Add(newTile);
            }
        }
    }
}
