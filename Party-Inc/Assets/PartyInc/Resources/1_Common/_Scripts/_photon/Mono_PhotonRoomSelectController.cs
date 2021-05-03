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
    public class Mono_PhotonRoomSelectController : MonoBehaviourPunCallbacks
    {
        private const int maxPlayers = 4;

        private List<RoomInfo> currentRoomList = new List<RoomInfo>();

        [SerializeField] private Text nameText;
        private string gameToJoin = "";

        #region Unity Callbacks

        public override void OnEnable()
        {
            base.OnEnable();

            //if(PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
            //    nameText.text = "Welcome " + PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.DisplayName + "!";

            SetPhoneOrientation();
            
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        private void SetPhoneOrientation()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            bool isInActiveScene = false;

            foreach (GameObject go in activeScene.GetRootGameObjects())
            {
                if (go.name == name)
                {
                    isInActiveScene = true;
                    break;
                }
            }

            if (isInActiveScene && (Screen.orientation == ScreenOrientation.Landscape || Screen.autorotateToLandscapeLeft || Screen.autorotateToLandscapeRight))
            {
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Screen.autorotateToPortrait = true;
            }
        }

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

        private IEnumerator GoToLobbyCo()
        {
            int[] scn = { (int)Stt_SceneIndexes.ROTATEDEVICE };
            int currScn = SceneManager.GetActiveScene().buildIndex;
            yield return StartCoroutine(Mng_SceneNavigationSystem.Current.LoadScenesAsyncAdditive(scn));

            Mng_SceneNavigationSystem.Current.ActivateLoadedScene(scn[0]);

            yield return new WaitForSeconds(4.5f);

            yield return StartCoroutine(Mng_SceneNavigationSystem.Current.DramaticSceneTransitionStartCo(0.1f));

            Mng_SceneNavigationSystem.Current.DeactivateLoadedScene(currScn);

            Mng_SceneNavigationSystem.Current.DramaticSceneTransitionEnd(0.1f);

            PhotonNetwork.LoadLevel(gameToJoin + "GameLobby");
        }

        #endregion

        #region PUN Callbacks

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("Fiesta Time/ RoomController: Left lobby.");

            Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
            Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.LAUNCHER_SIGNIN);
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

            StartCoroutine("GoToLobbyCo");
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
        }

        public void GoToClosetStore()
        {
            Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
            Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.AVATAR_CLOSE_STORE);
        }
    }
}
