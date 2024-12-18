﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace PartyInc
{
    namespace CC
    {
        public class Mono_Log_Spawner_CC : MonoBehaviourPun
        {
            [SerializeField] private GameObject _largeLogPrefab;
            [SerializeField] private GameObject _mediumLogPrefab;
            [SerializeField] private GameObject _smallHorizLogPrefab;
            [SerializeField] private GameObject _smallVertLogPrefab;
            [SerializeField] private GameObject _vSmallHorizLogPrefab;
            [SerializeField] private GameObject _vSmallVertLogPrefab;

            [SerializeField] private float _spawnInterval;
            private float _currentTime = 0f;

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
                if(eventData.Code == Constants.SpawnLogEventCode && photonView.IsMine)
                {
                    object[] data = (object[])eventData.CustomData;

                    // Get data
                    float windowTime = (float)data[0];
                    int logType = (int)data[1];
                    float markPos = (float)data[2];
                    float markAngle = (float)data[3];
                    double photonSendTime = (double)data[4];

                    // Generate log
                    // Decide log type
                    string toSpawn = DecideLogType(logType);

                    // Spawn log
                    GameObject newLog = PhotonNetwork.Instantiate("_logs/" + toSpawn, transform.position, Quaternion.identity);
                    Mono_Log_Info_CC log = newLog.GetComponent<Mono_Log_Info_CC>();
                    Mono_Log_Controller_CC logCon = newLog.GetComponent<Mono_Log_Controller_CC>();

                    // Decide mark
                    log.StartWidth = markPos;
                    log.StartHeight = markPos;
                    log.Angle = markAngle;

                    // Transfer ownership
                    newLog.GetComponent<PhotonView>().TransferOwnership(photonView.OwnerActorNr);

                    // Apply time things
                    logCon.WaitTime = (float)photonSendTime + 0.4f;
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
                        return _vSmallHorizLogPrefab.name;
                    case 5:
                        return _vSmallVertLogPrefab.name;
                    default:
                        throw new System.Exception("PartyInc/CC/DecideLogType: Wrong log type arrived");
                }
            }
        }
    }
}


