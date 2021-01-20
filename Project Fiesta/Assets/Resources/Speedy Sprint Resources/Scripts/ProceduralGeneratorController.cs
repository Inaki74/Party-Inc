using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
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
        /// Each section is composed of 3 sub-sections. Each sub-section is abstracted as a number, so as to send over the network. Therefore each section is a length 3 vector.
        /// The game is the infinite string of these sections, each different from one another.
        /// Sub-sections will be designed within Unity and stored as prefabs. There will be easy, medium and hard sub-sections.
        /// Sections will have certain number of easy, medium and hard sub-sections. These scale with the games length.
        /// Sections are then formed by code, randomly, with some rules in place (no two same patterns in a row, or such things).
        /// There will be a "SubsectionPool" which stores the entire sub-section, then they are placed in their respective lanes.
        /// The game will pre load like 3 sections ahead and will un-load 2 sections behind, per lane.
        /// The spacing of each sub-section is determined by the amount of tiles each sub-section occupies. Each tile is 10 units X and Z.
        /// </summary>
        public class ProceduralGeneratorController : MonoBehaviourPunCallbacks
        {
            // Struct that represents a Sub-Section
            private struct SubSection
            {
                public int id;
                public char diff;
            }

            // The spawner objects where the sections will be generated.
            [SerializeField] private GameObject _laneOneSpawner;
            [SerializeField] private GameObject _laneTwoSpawner;
            [SerializeField] private GameObject _laneThreeSpawner;
            [SerializeField] private GameObject _laneFourSpawner;

            // The first subsection of every game
            [SerializeField] private GameObject _openingSubsection;

            // Options to test sub-sections
            [Tooltip("Activate to test a sub-section, this option will make the spawner spawn only this sub-section.")]
            [SerializeField] private bool _subSectionTest;
            [Header("E.g: M5 (Medium 5)")]
            [SerializeField] private string _subSectionToTest;

            // The next section to spawn and the last one spawned.
            [SerializeField] private int[] _nextSection = new int[3];
            [SerializeField] private int[] _lastSection = new int[3];

            // Amount of easy/medium/hard sub-sections this round.
            private int _easyThisRound = 3;
            private int _mediumThisRound = 0;
            private int _hardThisRound = 0;
            // Which round are we at.
            private int round = 0;

            // Total amount of easy/medium/hard sub-sections loaded.
            private int _totalEasy;
            private int _totalMedium;
            private int _totalHard;

            // The current Z coordinate we are spawning at.
            private float _currentZ;

            // A _runOnce boolean to run start once in update loop.
            private bool _started = false;

            // Start is called before the first frame update
            void Start()
            {
                _totalEasy = TotalAmountOfSubsections("Easy");
                _totalMedium = TotalAmountOfSubsections("Medium");
                _totalHard = TotalAmountOfSubsections("Hard");

                _nextSection[0] = -1;
            }

            // Update is called once per frame
            void Update()
            {
                if(!_started && GameManager.Current.GameBegan)
                {
                    _started = true;

                    if(PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnectedAndReady)
                    {
                        GenerateNextSection();
                    }
                }
            }

            private void Awake()
            {
                InvisibleTrolleyController.onPassedSection += GenerateNextSection;

                // We decide where to put the sub-sections (dependent on GameManager's PlayerCount).
                DecideSpawnerPositions();

                // We place the first sub-section.
                PlaceFirstSubsection();
            }

            private void OnDestroy()
            {
                InvisibleTrolleyController.onPassedSection -= GenerateNextSection;
            }

            /// <summary>
            /// Decides where the Section Spawners should be at. Dependant on how many players are in the game.
            /// </summary>
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
                        _laneOneSpawner.transform.position = new Vector3(-7f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.transform.position = new Vector3(7f, _laneTwoSpawner.transform.position.y, _laneTwoSpawner.transform.position.z);
                        _laneThreeSpawner.SetActive(false);
                        _laneFourSpawner.SetActive(false);
                        break;
                    case 3:
                        _laneOneSpawner.transform.position = new Vector3(-12f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.transform.position = new Vector3(0f, _laneTwoSpawner.transform.position.y, _laneTwoSpawner.transform.position.z);
                        _laneThreeSpawner.transform.position = new Vector3(12f, _laneThreeSpawner.transform.position.y, _laneThreeSpawner.transform.position.z);
                        _laneFourSpawner.SetActive(false);
                        break;
                    case 4:
                        _laneOneSpawner.transform.position = new Vector3(-18f, _laneOneSpawner.transform.position.y, _laneOneSpawner.transform.position.z);
                        _laneTwoSpawner.transform.position = new Vector3(-6f, _laneTwoSpawner.transform.position.y, _laneTwoSpawner.transform.position.z);
                        _laneThreeSpawner.transform.position = new Vector3(6f, _laneThreeSpawner.transform.position.y, _laneThreeSpawner.transform.position.z);
                        _laneFourSpawner.transform.position = new Vector3(18f, _laneFourSpawner.transform.position.y, _laneFourSpawner.transform.position.z);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            ///  Function that randomizes the upcoming section.
            /// </summary>
            /// <param name="easy">Amount of easy sub-sections</param>
            /// <param name="medium">Amount of medium sub-sections</param>
            /// <param name="hard">Amount of hard sub-sections</param>
            /// <returns></returns>
            private int[] GenerateSection(int easy, int medium, int hard)
            {
                int[] randomization = new int[3];

                for (int i = 0; i < easy; i++)
                {
                    randomization[i] = Random.Range(1, _totalEasy + 1);
                }

                for (int i = easy; i < easy + medium; i++)
                {
                    randomization[i] = Random.Range(1, _totalMedium + 1);
                }

                for (int i = easy + medium; i < easy+ medium + hard; i++)
                {
                    randomization[i] = Random.Range(1, _totalHard + 1);
                }

                return randomization;
            }

            /// <summary>
            /// Gets the amount of sub-section assets available for a certain difficulty.
            /// </summary>
            /// <param name="difficulty">The difficulty</param>
            /// <returns></returns>
            private int TotalAmountOfSubsections(string difficulty)
            {
                Object[] all = Resources.LoadAll(GameManager.SubsectionsPath + difficulty, typeof(GameObject)).Cast<GameObject>().ToArray();
                Resources.UnloadUnusedAssets();
                return all.Length;
            }

            /// <summary>
            /// Places the first sub-section on all lanes available.
            /// </summary>
            private void PlaceFirstSubsection()
            {
                PlaceFirstSubsectionInLane(_laneOneSpawner);

                if (_laneTwoSpawner.activeInHierarchy)
                {
                    PlaceFirstSubsectionInLane(_laneTwoSpawner);
                }

                if (_laneThreeSpawner.activeInHierarchy)
                {
                    PlaceFirstSubsectionInLane(_laneThreeSpawner);
                }

                if (_laneFourSpawner.activeInHierarchy)
                {
                    PlaceFirstSubsectionInLane(_laneFourSpawner);
                }

                _currentZ = 50;
            }

            /// <summary>
            /// Places the first subsection at a spawners transform.
            /// </summary>
            /// <param name="spawner">The spawner</param>
            private void PlaceFirstSubsectionInLane(GameObject spawner)
            {
                GameObject firstSubsection = Instantiate(_openingSubsection);
                firstSubsection.transform.position = new Vector3(spawner.transform.position.x, 0.1f, 0);
            }

            /// <summary>
            /// Places an entire section in all lanes.
            /// </summary>
            /// <param name="section">The section generated</param>
            private void PlaceSection(int[] section)
            {
                for(int i = 0; i < section.Length; i++)
                {
                    int subsec = section[i];
                    string diff = "";

                    if(i < _easyThisRound)
                    {
                        diff = "E";
                    }

                    if(i >= _easyThisRound && i < _easyThisRound + _mediumThisRound)
                    {
                        diff = "M";
                    }

                    if(i >= _easyThisRound + _mediumThisRound && i < _easyThisRound + _mediumThisRound + _hardThisRound)
                    {
                        diff = "H";
                    }

                    int tiles = PlaceSectionInLane(_laneOneSpawner, diff, subsec);

                    if (_laneTwoSpawner.activeInHierarchy)
                    {
                        PlaceSectionInLane(_laneTwoSpawner, diff, subsec);
                    }

                    if (_laneThreeSpawner.activeInHierarchy)
                    {
                        PlaceSectionInLane(_laneThreeSpawner, diff, subsec);
                    }

                    if (_laneFourSpawner.activeInHierarchy)
                    {
                        PlaceSectionInLane(_laneFourSpawner, diff, subsec);
                    }

                    _currentZ += 10 * tiles;
                }

                if (GameManager.Current.playerCount > 1 && PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("RPC_SendSection", RpcTarget.Others, section);
                }
            }

            /// <summary>
            /// Places a section in the corresponding spawners position. Makes a call to the SubsectionPoolManager to get the sub-section and then places it.
            /// </summary>
            /// <param name="spawner">THe spawner</param>
            /// <param name="diff">Subsection difficulty label</param>
            /// <param name="subsec">Subsection id</param>
            /// <returns></returns>
            private int PlaceSectionInLane(GameObject spawner, string diff, int subsec)
            {
                GameObject toPlace = SubsecPoolManager.Current.RequestSubsection(diff, subsec);
                
                toPlace.transform.position = new Vector3(spawner.transform.position.x, 0.1f, _currentZ);

                return toPlace.GetComponent<SubSectionInfo>().AmountOfTiles;
            }

            /// <summary>
            /// Function run by the PassedSection event. When we pass a section, we must generate the next one. This function does that.
            /// </summary>
            private void GenerateNextSection()
            {   if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;
                //Debug.Log("Generate Next Section");

                //If we are testing a subsection.
                if (_subSectionTest)
                {
                    SubsectionTest();
                    return;
                }

                // We are in normal mode
                // If its the first generation
                if (_nextSection[0] == -1)
                {
                    // First generation
                    _nextSection = GenerateSection(_easyThisRound, _mediumThisRound, _hardThisRound);

                    PlaceSection(_nextSection);

                    // Ideal to be 2 sections loaded ahead.
                    GenerateNextSection();
                }
                else
                {
                    // Nth generation
                    _lastSection = _nextSection;

                    // Ramp up the difficulty
                    ChangeDifficulty();

                    int tries = 0;

                    // The section generated must be acceptable, we give a maximum of 50 tries for the section to be generated successfully.
                    do
                    {
                        _nextSection = GenerateSection(_easyThisRound, _mediumThisRound, _hardThisRound);
                        tries++;
                    } while (!AcceptableSection(_nextSection, _lastSection) && tries <= 50);

                    PlaceSection(_nextSection);
                }
            }

            /// <summary>
            /// FUnction that evaluates if the section is acceptable.
            /// </summary>
            /// <param name="section"></param>
            /// <param name="lastSection"></param>
            /// <returns>True if acceptable</returns>
            private bool AcceptableSection(int[] section, int[] lastSection)
            {

                return true;

                bool condition1 = !OverlappedSections(section, lastSection);

                bool condition2 = !SameSubsectionsInOneSection(section);

                bool condition3 = !RepeatedSubsectionsInARowWithinTwoSections(section, lastSection);

                return condition1 && condition2 && condition3;
            }

            private bool RepeatedSubsectionsInARowWithinTwoSections(int[] section, int[] lastSection)
            {
                return true;
            }

            private bool SameSubsectionsInOneSection(int[] section)
            {
                if (_easyThisRound == _mediumThisRound && _mediumThisRound == _hardThisRound) return false;

                for (int i = 0; i < _easyThisRound; i++)
                {
                    for (int j = 0; j < _easyThisRound; j++)
                    {
                        if (section[i] == section[j])
                        {
                            return true;
                        }
                    }
                }

                // Checks overlap of medium
                for (int i = _easyThisRound; i < _easyThisRound + _mediumThisRound; i++)
                {
                    for (int j = _easyThisRound; j < _mediumThisRound + _easyThisRound; j++)
                    {
                        if (section[i] == section[j])
                        {
                            return true;
                        }
                    }
                }

                // Checks overlap of hard
                for (int i = _easyThisRound + _mediumThisRound; i < _easyThisRound + _mediumThisRound + _hardThisRound; i++)
                {
                    for (int j = _easyThisRound + _mediumThisRound; j < _easyThisRound + _mediumThisRound + _hardThisRound; j++)
                    {
                        if (section[i] == section[j])
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            private bool OverlappedSections(int[] section, int[] lastSection)
            {
                // Checks overlap of easy
                for(int i = 0; i < _easyThisRound; i++)
                {
                    for(int j = 0; j < _easyThisRound; j++)
                    {
                        if(section[i] == lastSection[j])
                        {
                            return true;
                        }
                    }
                }

                // Checks overlap of medium
                for (int i = _easyThisRound; i < _easyThisRound + _mediumThisRound; i++)
                {
                    for(int j = _easyThisRound; j < _mediumThisRound + _easyThisRound; j++)
                    {
                        if (section[i] == lastSection[j])
                        {
                            return true;
                        }
                    }
                }

                // Checks overlap of hard
                for (int i = _easyThisRound + _mediumThisRound; i < _easyThisRound + _mediumThisRound + _hardThisRound; i++)
                {
                    for (int j = _easyThisRound + _mediumThisRound; j < _easyThisRound + _mediumThisRound + _hardThisRound; j++)
                    {
                        if (section[i] == lastSection[j])
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Function that ramps up the difficulty.
            /// </summary>
            private void ChangeDifficulty()
            {
                round++;

                switch (round)
                {
                    case 1:
                        _easyThisRound = 2;
                        _mediumThisRound = 1;
                        _hardThisRound = 0;
                        break;
                    case 2:
                        _easyThisRound = 1;
                        _mediumThisRound = 2;
                        _hardThisRound = 0;
                        break;
                    case 3:
                        _easyThisRound = 0;
                        _mediumThisRound = 3;
                        _hardThisRound = 0;
                        break;
                    case 4:
                        _easyThisRound = 0;
                        _mediumThisRound = 2;
                        _hardThisRound = 1;
                        break;
                    case 5:
                        _easyThisRound = 0;
                        _mediumThisRound = 1;
                        _hardThisRound = 2;
                        break;
                    case 6:
                        _easyThisRound = 0;
                        _mediumThisRound = 0;
                        _hardThisRound = 3;
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Function that runs when we activate the testing boolean.
            /// </summary>
            private void SubsectionTest()
            {
                char diff = _subSectionToTest[0];
                int subSectionId = int.Parse(_subSectionToTest.Substring(1));
                if (diff == 'E')
                {
                    _easyThisRound = 3;
                    _mediumThisRound = 0;
                    _hardThisRound = 0;
                }
                else if (diff == 'M')
                {
                    _easyThisRound = 0;
                    _mediumThisRound = 3;
                    _hardThisRound = 0;
                }
                else
                {
                    _easyThisRound = 0;
                    _mediumThisRound = 0;
                    _hardThisRound = 3;
                }

                _nextSection[0] = subSectionId;
                _nextSection[1] = subSectionId;
                _nextSection[2] = subSectionId;

                if (_nextSection[0] == -1)
                {
                    PlaceSection(_nextSection);

                    GenerateNextSection();
                }
                else
                {
                    PlaceSection(_nextSection);
                }
            }


            public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
            {
                base.OnMasterClientSwitched(newMasterClient);
                //TODO: Set up the master client switch
                //newMasterClient.CustomProperties
            }

            [PunRPC]
            public void RPC_SendSection(int[] section)
            {
                PlaceSection(section);
            }
        }
    }
}


