using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    /// <summary>
    /// Its the class in charge of the game selection.
    /// </summary>
    public class Mono_NetworkRoomSelectController : MonoBehaviourPunCallbacks
    {
        private const int maxPlayers = 4;

        private List<RoomInfo> currentRoomList = new List<RoomInfo>();

        [SerializeField] List<Button> _gameButtons = new List<Button>();

        [SerializeField] private Text nameText;
        private string gameToJoin = "";

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            nameText.text = "Welcome " + PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.DisplayName + "!";

            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        #region Join Functions

        public void JoinEGG()
        {
            if (gameToJoin == "")
                JoinARoom("EGG_", maxPlayers);
        }

        public void JoinDD()
        {
            if (gameToJoin == "")
                JoinARoom("DD_", maxPlayers);
        }

        public void JoinTT()
        {
            if (gameToJoin == "")
                JoinARoom("TT_", maxPlayers);
        }

        public void JoinRR()
        {
            if (gameToJoin == "")
                JoinARoom("RR_", maxPlayers);
        }

        public void JoinSS()
        {
            if (gameToJoin == "")
                JoinARoom("SS_", maxPlayers);
        }

        public void JoinCC()
        {
            if (gameToJoin == "")
                JoinARoom("CC_", maxPlayers);
        }

        public void JoinLL()
        {
            if (gameToJoin == "")
                JoinARoom("LL_", maxPlayers);
        }

        public void JoinAS()
        {
            if (gameToJoin == "")
                JoinARoom("AS_", maxPlayers);
        }

        public void JoinARoom(string gameJoining, int maxPlayersAllowed)
        {
            gameToJoin = gameJoining;

            foreach (RoomInfo room in currentRoomList)
            {
                Debug.Log("Room: " + room.Name + ", Quantity: " + room.PlayerCount + ", Maximum allowed: " + room.MaxPlayers + ", is available ? " + room.IsOpen);
                if (room.PlayerCount < room.MaxPlayers && room.IsOpen && IsGameImLookingFor(room.Name, gameJoining))
                {
                    PhotonNetwork.JoinRoom(room.Name);
                    return;
                }
            }


            PhotonNetwork.CreateRoom(gameJoining + Random.Range(0, 10000), new RoomOptions { MaxPlayers = (byte)maxPlayersAllowed });
        }

        private bool IsGameImLookingFor(string roomName, string gamePrefix)
        {
            return roomName.Contains(gamePrefix);
        }

        #endregion

        #region PUN Callbacks

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("Fiesta Time/ RoomController: Left lobby.");

            foreach (Button b in _gameButtons)
            {
                b.interactable = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {

        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Fiesta Time/ RoomController: Room list updated.");
            currentRoomList = roomList;
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Fiesta Time/ RoomController: Room created!");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Fiesta Time/ RoomController: Room creation failed, reason: " + message + " error code: " + returnCode + ". Trying again...");

            PhotonNetwork.CreateRoom(gameToJoin + Random.Range(0, 10000), new RoomOptions { MaxPlayers = maxPlayers });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Fiesta Time/ RoomController: Successfully joined room. Entering game...");

            PhotonNetwork.LoadLevel(gameToJoin + "GameLobby");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Fiesta Time/ RoomController: Room creation failed, reason: " + message + " error code: " + returnCode + ". Try again later.");
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("Fiesta Time/ RoomController: Left room.");
        }

        #endregion

        public void SignOut()
        {
            PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.SignOut();
            PhotonNetwork.LeaveLobby();
            SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNIN);
        }
    }
}
