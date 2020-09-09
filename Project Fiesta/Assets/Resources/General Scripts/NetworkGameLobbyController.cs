using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    public class NetworkGameLobbyController : MonoBehaviourPunCallbacks
    {
        private PhotonView Pv;

        [SerializeField] private Text playerOneText;
        [SerializeField] private Text playerTwoText;
        [SerializeField] private Text countdownText;

        private bool runOnce = true;

        private float maxCountdownTime = 8f;

        private float currentCountdownTime;

        // Start is called before the first frame update
        void Start()
        {
            Pv = GetComponent<PhotonView>();

            currentCountdownTime = maxCountdownTime;

            PhotonNetwork.AutomaticallySyncScene = true;

            PlayerPrefs.SetString(Constants.CHRCTR_KEY_NETWRK, Constants.BUNNY_NAME_CHRCTR);
            if (PhotonNetwork.IsMasterClient)
            {
                playerOneText.text = PhotonNetwork.NickName;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                playerTwoText.text = "Searching";
            }
            else
            {
                playerTwoText.text = PhotonNetwork.NickName;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (currentCountdownTime < 0f && PhotonNetwork.IsMasterClient && runOnce)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                runOnce = false;
                PhotonNetwork.LoadLevel("EggGrabbingGameB");
            }
            else
            {
                currentCountdownTime -= Time.deltaTime;
                countdownText.text = string.Format("{0:00}", currentCountdownTime);
            }
        }

        #region Button Functions

        public void BtnLeaveRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(1);
        }

        public void BtnChooseBunny()
        {
            PlayerPrefs.SetString(Constants.CHRCTR_KEY_NETWRK, Constants.BUNNY_NAME_CHRCTR); ;
        }

        public void BtnChooseSanta()
        {
            PlayerPrefs.SetString(Constants.CHRCTR_KEY_NETWRK, Constants.SANTA_NAME_CHRCTR);
        }

        #endregion

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("Fiesta Time/ EGG/ NetworkRoomController: A player joined the room! Welcome " + newPlayer.NickName);
            playerTwoText.text = newPlayer.NickName;

            if (PhotonNetwork.IsMasterClient)
            {
                Pv.RPC("RPC_SendTimer", RpcTarget.Others, currentCountdownTime);
                Pv.RPC("RPC_SendName", RpcTarget.Others, PhotonNetwork.NickName);
            }

        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("Fiesta Time/ EGG/ NetworkRoomController: Bye bye! " + otherPlayer.NickName);
            playerTwoText.text = "Searching...";
        }

        [PunRPC]
        public void RPC_SendTimer(float time)
        {
            currentCountdownTime = time;
        }

        [PunRPC]
        public void RPC_SendName(string name)
        {
            playerOneText.text = name;
        }
    }
}


