using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace FiestaTime
{
    namespace CC
    {
        public class LogSpawner : MonoBehaviourPun
        {
            [SerializeField] private GameObject _largeLogPrefab;
            [SerializeField] private GameObject _mediumLogPrefab;
            [SerializeField] private GameObject _smallHorizLogPrefab;
            [SerializeField] private GameObject _smallVertLogPrefab;
            [SerializeField] private GameObject _vSmallHorizLogPrefab;
            [SerializeField] private GameObject _vSmallVertLogPrefab;

            [SerializeField] private float _spawnInterval;
            private float _currentTime = 0f;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            private void OnEnable()
            {
                PhotonNetwork.NetworkingClient.EventReceived += SpawnLog;

            }

            private void OnDisable()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= SpawnLog;
            }

            private void SpawnLog(EventData eventData)
            {
                if(eventData.Code == GameManager.SpawnLogEventCode && photonView.IsMine)
                {
                    string toSpawn = _largeLogPrefab.name;
                    int random = Random.Range(4, 5);

                    switch (random)
                    {
                        case 1:
                            toSpawn = _mediumLogPrefab.name;
                            break;
                        case 2:
                            toSpawn = _smallHorizLogPrefab.name;
                            break;
                        case 4:
                            toSpawn = _smallVertLogPrefab.name;
                            break;
                        case 3:
                            toSpawn = _vSmallHorizLogPrefab.name;
                            break;
                        case 5:
                            toSpawn = _vSmallVertLogPrefab.name;
                            break;
                        default:
                            break;
                    }

                    GameObject newLog = PhotonNetwork.Instantiate(toSpawn, transform.position, Quaternion.identity);
                    FallingLog log = newLog.GetComponent<FallingLog>();
                    log.StartWidth = Random.Range(-0.2f, 0.2f);

                    int side;
                    if(Random.Range(0, 1) == 0)
                    {
                        side = -1;
                    }
                    else
                    {
                        side = 1;
                    }
                    log.Angle = Random.Range(80f, 90f) * side;

                    newLog.GetComponent<PhotonView>().TransferOwnership(photonView.OwnerActorNr);
                }
            }
        }
    }
}


