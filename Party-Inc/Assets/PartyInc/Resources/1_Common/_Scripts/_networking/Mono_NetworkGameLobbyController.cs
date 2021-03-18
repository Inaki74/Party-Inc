using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


namespace PartyInc
{
    /// <summary>
    /// Its the class that controls all game lobbies.
    /// </summary>
    public class Mono_NetworkGameLobbyController : MonoBehaviourPunCallbacks
    {
        public float maxCountdownTime = 8f;
        public string gameSceneName;

        private PhotonView Pv;
        private ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
        private int[] playersInRoom = new int[4];

        [Header ("Player Name Texts, place them in order")]
        [SerializeField] private Text[] playerTexts;
        [SerializeField] private Text countdownText;

        private bool runOnce = true;

        private float currentCountdownTime;

        #region Unity Callbacks

        // Start is called before the first frame update
        void Start()
        {
            Pv = GetComponent<PhotonView>();

            currentCountdownTime = maxCountdownTime;

            PhotonNetwork.AutomaticallySyncScene = true;

            if (PhotonNetwork.IsMasterClient)
            {
                playerTexts[0].text = PhotonNetwork.NickName;
                
                playersInRoom[0] = PhotonNetwork.LocalPlayer.ActorNumber;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (currentCountdownTime < 0f && runOnce)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    runOnce = false;
                    PhotonNetwork.LoadLevel(gameSceneName);
                }
            }
            else
            {
                currentCountdownTime -= Time.deltaTime;
                countdownText.text = string.Format("{0:00}", currentCountdownTime);
            }
        }

        #endregion

        #region Button Functions

        public void BtnLeaveRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(Stt_SceneIndexes.HUB);
        }

        #endregion

        #region Private Functions

        // Deprecated
        //private void SetCustomProperties()
        //{
        //    int[] aux = new int[PhotonNetwork.CurrentRoom.PlayerCount];

        //    for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        //    {
        //        aux[i] = playersInRoom[i];
        //    }

        //    roomProps.Add("PlayerIDsList", aux);
        //    bool a = PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        //    if (a)
        //    {
        //        Debug.Log("Properties set appropiately.");
        //    }
        //}

        /// <summary>
        /// Updates the name texts in the UI.
        /// </summary>
        private void UpdateNames()
        {
            for (int i = 0; i < playersInRoom.Length; i++)
            {
                if (playersInRoom[i] != 0 && PhotonNetwork.CurrentRoom.Players.ContainsKey(playersInRoom[i]))
                {
                    playerTexts[i].text = PhotonNetwork.CurrentRoom.GetPlayer(playersInRoom[i]).NickName;
                }
                else if(i < playerTexts.Length)
                {
                    playerTexts[i].text = "Connecting...";
                }
            }
        }

        /// <summary>
        /// Updates the list of players after the Master Left.
        /// </summary>
        /// <param name="newMasterClient"></param>
        private void UpdatePlayerListMasterLeft(Photon.Realtime.Player newMasterClient)
        {
            int[] aux = new int[playersInRoom.Length];

            aux[0] = newMasterClient.ActorNumber;
            for (int i = 0; i < playersInRoom.Length; i++)
            {
                for (int j = 1; j < playersInRoom.Length; j++)
                {
                    if (playersInRoom[i] != 0 &&
                        playersInRoom[i] != newMasterClient.ActorNumber &&
                        playersInRoom[i] != PhotonNetwork.CurrentRoom.masterClientId &&
                        !aux.Contains(playersInRoom[i]))
                    {
                        aux[j] = playersInRoom[i];
                    }
                }
            }

            playersInRoom = aux;
        }

        /// <summary>
        /// Updates the list of players after a player left.
        /// </summary>
        /// <param name="otherPlayer"></param>
        private void UpdatePlayerListPlayerLeft(Photon.Realtime.Player otherPlayer)
        {
            int[] aux = new int[playersInRoom.Length];
            int k = 0;

            for (int i = 0; i < playersInRoom.Length; i++)
            {
                if (playersInRoom[i] != otherPlayer.ActorNumber)
                {
                    aux[k] = playersInRoom[i];
                    k++;
                }
            }

            playersInRoom = aux;
        }

        #endregion

        #region PUN Callbacks

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("Fiesta Time/" + PhotonNetwork.CurrentRoom.Name + "/ NetworkGameLobbyController: A player joined the room! Welcome " + newPlayer.NickName);
            
            if (PhotonNetwork.IsMasterClient)
            {
                playersInRoom[PhotonNetwork.CurrentRoom.PlayerCount - 1] = newPlayer.ActorNumber;
                Pv.RPC("RPC_SendTimer", RpcTarget.Others, currentCountdownTime);
                Pv.RPC("RPC_SendCurrentPlayers", RpcTarget.All, playersInRoom);
            }

        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("Fiesta Time/" + PhotonNetwork.CurrentRoom.Name + "/ NetworkGameLobbyController: Bye bye! " + otherPlayer.NickName);

            if (PhotonNetwork.IsMasterClient && !otherPlayer.IsMasterClient)
            {
                UpdatePlayerListPlayerLeft(otherPlayer);
                Pv.RPC("RPC_SendCurrentPlayers", RpcTarget.All, playersInRoom);
            }
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            Debug.Log("Fiesta Time/" + PhotonNetwork.CurrentRoom.Name + "/ NetworkGameLobbyController: The old Master Client is dead," +
                      " long live the Master Client: " + newMasterClient.NickName + "!");

            UpdatePlayerListMasterLeft(newMasterClient);
            UpdateNames();
        }

        #endregion

        #region PUNRPCs

        [PunRPC]
        public void RPC_SendTimer(float time)
        {
            currentCountdownTime = time;
        }

        [PunRPC]
        public void RPC_SendCurrentPlayers(int[] players)
        {
            playersInRoom = players;

            UpdateNames();
        }

        #endregion
    }
}


