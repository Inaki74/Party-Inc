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

            private void Awake()
            {
                PhotonNetwork.NetworkingClient.EventReceived += SpawnLog;

            }

            private void OnDestroy()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= SpawnLog;
            }

            private void SpawnLog(EventData eventData)
            {
                if(eventData.Code == GameManager.SpawnLogEventCode && photonView.IsMine)
                {
                    object[] data = (object[])eventData.CustomData;

                    // Get data
                    float waitTime = (float)data[0];
                    float windowTime = (float)data[1];
                    int logType = (int)data[2];
                    float markPos = (float)data[3];
                    float markAngle = (float)data[4];
                    double photonSendTime = (double)data[5];

                    // Generate log
                    // Decide log type
                    string toSpawn = DecideLogType(logType);

                    // Spawn log
                    GameObject newLog = PhotonNetwork.Instantiate(toSpawn, transform.position, Quaternion.identity);
                    FallingLog log = newLog.GetComponent<FallingLog>();
                    LogController logCon = newLog.GetComponent<LogController>();

                    // Decide mark
                    log.StartWidth = markPos;
                    log.StartHeight = markPos;
                    log.Angle = markAngle;

                    // Transfer ownership
                    newLog.GetComponent<PhotonView>().TransferOwnership(photonView.OwnerActorNr);

                    // Apply time things
                    logCon.WaitTime = waitTime + (float)photonSendTime + 0.4f;
                    logCon.WindowTime = windowTime;
                }
            }

            private string DecideLogType(int logType)
            {
                switch (logType)
                {
                    case 0:
                        return _largeLogPrefab.name;
                    case 1:
                        return _mediumLogPrefab.name;
                    case 2:
                        return _smallHorizLogPrefab.name;
                    case 3:
                        return _smallVertLogPrefab.name;
                    case 4:
                        return  _vSmallHorizLogPrefab.name;
                    case 5:
                        return _vSmallVertLogPrefab.name;
                    default:
                        throw new System.Exception("FiestaTime/CC/DecideLogType: Wrong log type arrived");
                }
            }
        }
    }
}


