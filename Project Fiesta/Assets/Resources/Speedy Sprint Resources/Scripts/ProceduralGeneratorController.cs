using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

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
        ///
        /// LAST PARAGRAPH IS DEPRECATED
        ///
        /// Remake: The procedural generator takes the sub-section prefabs and composes them into sections.
        /// Each section is composed of 5 sub-sections. Each sub-section is abstracted as a number, so as to send over the network. Therefore each section is a length 5 vector.
        /// The game is the infinite string of these sections, each different from one another.
        /// Sub-sections will be designed within Unity and stored as prefabs. There will be easy, medium and hard sub-sections.
        /// Sections will have certain number of easy, medium and hard sub-sections. These scale with the games length.
        /// Sections are then formed by code, randomly, with some rules in place (no two same patterns in a row, or such things).
        /// There will be a "SubsectionPool" which stores the entire sub-section, then they are placed in their respective lanes.
        /// The game will pre load like 3 sub-sections ahead and will un-load 2 sub-sections behind, per lane.
        /// The spacing of each sub-section is determined by the amount of tiles each sub-section occupies. Each tile is 10 units X and Z.
        /// </summary>
        public class ProceduralGeneratorController : MonoBehaviourPun
        {
            [SerializeField] private GameObject _laneOneSpawner;
            [SerializeField] private GameObject _laneTwoSpawner;
            [SerializeField] private GameObject _laneThreeSpawner;
            [SerializeField] private GameObject _laneFourSpawner;

            private int[] _nextSubsections = new int[5];
            private int[] _lastSubsections = new int[5];
            private int _easyThisRound = 5;
            private int _mediumThisRound = 0;
            private int _hardThisRound = 0;

            private float _currentZ;


            // Start is called before the first frame update
            void Start()
            {

                DecideSpawnerPositions();

                GenerateSubsectionRandomization(_easyThisRound, _mediumThisRound, _hardThisRound);
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

            private void GenerateSubsectionRandomization(int easy, int medium, int hard)
            {
                for(int i = 0; i < easy + medium + hard; i++)
                {
                    //_nextSubsections[i] = Random.Range
                }
            }

            private int TotalAmountOfSubsections(string difficulty)
            {
                Object[] all = Resources.LoadAll(GameManager.SubsectionsPath + difficulty, typeof(GameObject)).Cast<GameObject>().ToArray();
                Resources.UnloadUnusedAssets();
                return all.Length;
            }

            private void InstantiateFirstSections()
            {
                
            }

            private void InstantiateFirstTilesForLane(GameObject spawner)
            {
                
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


