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
                    GameObject newLog = PhotonNetwork.Instantiate(_largeLogPrefab.name, transform.position, Quaternion.identity);
                    //newLog.GetComponent<PhotonView>().TransferOwnership(0);
                    Debug.Log(photonView.OwnerActorNr);
                    newLog.GetComponent<PhotonView>().TransferOwnership(photonView.OwnerActorNr);
                }
            }
        }
    }
}


