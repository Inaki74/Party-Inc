using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        /// <summary>
        /// Responsible for Generating the games tiles.
        /// Obstacles will be generated for each lane available along with its tiles (this simulates infinity).
        /// First we place the tiles -> Then we place obstacles if there is one, this obstacle is placed at a certain percentage of the tile (30%-70% of the planes length) and is managed by each tile.
        /// We see roughly 13 tiles at one time. With movement, that can amount to 15 tiles. So there should be 20 tiles already loaded. Plus three tiles that will be unloaded once they
        /// reach the end of the line (to save memory, TilePool? ObstaclePool?).
        /// </summary>
        public class ProceduralGeneratorController : MonoBehaviourPun
        {
            private Tile[,] _tiles; // 20 4
            [SerializeField] private GameObject _laneOneSpawner;
            [SerializeField] private GameObject _laneTwoSpawner;
            [SerializeField] private GameObject _laneThreeSpawner;
            [SerializeField] private GameObject _laneFourSpawner;

            private float _currentZ;


            // Start is called before the first frame update
            void Start()
            {
                _tiles = new Tile[20,GameManager.Current.playerCount];  /// Dont know if ill need it

                DecideSpawnerPositions();

                InstantiateFirstTiles();
            }

            // Update is called once per frame
            void Update()
            {

            }

            private void Awake()
            {
                InvisibleTrolleyController.onPassedTile += GenerateNextTiles;
            }

            private void OnDestroy()
            {
                InvisibleTrolleyController.onPassedTile -= GenerateNextTiles;
            }

            private void DecideSpawnerPositions()
            {
                switch (GameManager.Current.playerCount)
                {
                    case 1:
                        _laneOneSpawner.transform.position = new Vector3(0f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.SetActive(false);
                        _laneThreeSpawner.SetActive(false);
                        _laneFourSpawner.SetActive(false);
                        break;
                    case 2:
                        _laneOneSpawner.transform.position = new Vector3(-5f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.transform.position = new Vector3(5f, _laneTwoSpawner.transform.position.y, _laneTwoSpawner.transform.position.z);
                        _laneThreeSpawner.SetActive(false);
                        _laneFourSpawner.SetActive(false);
                        break;
                    case 3:
                        _laneOneSpawner.transform.position = new Vector3(-10f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.transform.position = new Vector3(0f, _laneTwoSpawner.transform.position.y, _laneTwoSpawner.transform.position.z);
                        _laneThreeSpawner.transform.position = new Vector3(10f, _laneThreeSpawner.transform.position.y, _laneThreeSpawner.transform.position.z);
                        _laneFourSpawner.SetActive(false);
                        break;
                    case 4:
                        _laneOneSpawner.transform.position = new Vector3(-15f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.transform.position = new Vector3(-5f, _laneTwoSpawner.transform.position.y, _laneTwoSpawner.transform.position.z);
                        _laneThreeSpawner.transform.position = new Vector3(5f, _laneThreeSpawner.transform.position.y, _laneThreeSpawner.transform.position.z);
                        _laneFourSpawner.transform.position = new Vector3(15f, _laneFourSpawner.transform.position.y, _laneFourSpawner.transform.position.z);
                        break;
                    default:
                        break;
                }
            }

            private void InstantiateFirstTiles()
            {
                InstantiateFirstTilesForLane(_laneOneSpawner);

                if (_laneTwoSpawner.activeInHierarchy)
                {
                    InstantiateFirstTilesForLane(_laneTwoSpawner);
                }

                if (_laneThreeSpawner.activeInHierarchy)
                {
                    InstantiateFirstTilesForLane(_laneThreeSpawner);
                }

                if (_laneFourSpawner.activeInHierarchy)
                {
                    InstantiateFirstTilesForLane(_laneFourSpawner);
                }
            }

            private void InstantiateFirstTilesForLane(GameObject spawner)
            {
                float z = -10;
                for (int i = 0; i < 15; i++)
                {
                    GameObject tile = TilePoolManager.Current.RequestTile();
                    tile.transform.position = new Vector3(spawner.transform.position.x, 0.1f, z);
                    tile.GetComponent<Tile>().SetTileCount(15 - i);

                    z += 10;
                }

                _currentZ = z;
            }

            private void InstantiateTileForLane(Tile template, GameObject spawner, float z)
            {
                GameObject tile = TilePoolManager.Current.RequestTile();
                tile.GetComponent<Tile>().PersonalizeTile(template.Obstacles, template.ObstacleZAlignment);
                tile.transform.position = new Vector3(spawner.transform.position.x, 0.1f, z);
            }

            /// <summary>
            /// When instantiating tiles:
            /// We create/enable the tile.
            /// THe tile generates its settings.
            /// We must wait for them to finish.
            /// We place the tile.
            /// We send the settings across the network.
            /// This Generator and all others across the network all create/request three (playerCount - 1) more tiles with the same settings.
            /// </summary>

            private void GenerateNextTiles()
            {
                Debug.Log("Generate Next tile");
                GameObject tile = TilePoolManager.Current.RequestTile();
                StartCoroutine(GenerateTilesCo(tile, tile.GetComponent<Tile>()));
            }

            private IEnumerator GenerateTilesCo(GameObject tileGo, Tile tile)
            {
                bool finishedGeneration = tile.ReRandomize();

                yield return new WaitUntil(() => finishedGeneration);

                tileGo.transform.position = new Vector3(_laneOneSpawner.transform.position.x, 0.1f, _currentZ);

                if (_laneTwoSpawner.activeInHierarchy)
                {
                    InstantiateTileForLane(tile, _laneTwoSpawner, _currentZ);
                }

                if (_laneThreeSpawner.activeInHierarchy)
                {
                    InstantiateTileForLane(tile, _laneThreeSpawner, _currentZ);
                }

                if (_laneFourSpawner.activeInHierarchy)
                {
                    InstantiateTileForLane(tile, _laneFourSpawner, _currentZ);
                }

                _currentZ += 10;

                if(GameManager.Current.playerCount > 1)
                {
                    photonView.RPC("RPC_SendTile", RpcTarget.Others, tile.Obstacles, tile.ObstacleZAlignment);
                }
            }

            [PunRPC]
            public void RPC_SendTile(int[] obs, float zAlign)
            {
                Tile tile = new Tile();
                tile.PersonalizeTile(obs, zAlign);

                InstantiateTileForLane(tile, _laneOneSpawner, _currentZ);

                if (_laneTwoSpawner.activeInHierarchy)
                {
                    InstantiateTileForLane(tile, _laneTwoSpawner, _currentZ);
                }

                if (_laneThreeSpawner.activeInHierarchy)
                {
                    InstantiateTileForLane(tile, _laneThreeSpawner, _currentZ);
                }

                if (_laneFourSpawner.activeInHierarchy)
                {
                    InstantiateTileForLane(tile, _laneFourSpawner, _currentZ);
                }

                _currentZ += 10;
            }
        }
    }
}


