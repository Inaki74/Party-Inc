using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkRoomSelectController : MonoBehaviourPunCallbacks
{
    private const int maxEggPlayers = 2;

    private List<RoomInfo> currentRoomList;

    [SerializeField] private Text nameText;
    private string gameJoining;
    private bool isConnecting;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = "Welcome " + PlayerPrefs.GetString(Constants.NAME_KEY_NETWRK) + "!";

        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine("JoinLobbyCo");
        }
    }

    private IEnumerator JoinLobbyCo()
    {
        if (!PhotonNetwork.InLobby)
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.JoinLobby());
        else
        {
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.LeaveLobby());
            PhotonNetwork.JoinLobby();
        }
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

    public void JoinEGG()
    {
        foreach(RoomInfo room in currentRoomList)
        {
            Debug.Log("Room: " + room.Name + ", Quantity: " + room.PlayerCount + ", Maximum allowed: " + room.MaxPlayers + ", is available ? " + room.IsOpen);
            if(room.PlayerCount < room.MaxPlayers && room.IsOpen)
            {
                PhotonNetwork.JoinRoom(room.Name);
                isConnecting = true;
                return;
            }
        }

        gameJoining = "EGG_";
        PhotonNetwork.CreateRoom(gameJoining + Random.Range(0, 10000), new RoomOptions { MaxPlayers = maxEggPlayers });
    }

    #region PUN Callbacks

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Fiesta Time/ RoomController: You have disconnected from the server. Cause: " + cause + ". Retrying...");
        // Loads disconnected scene
        SceneManager.LoadScene(2);
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

        PhotonNetwork.CreateRoom(gameJoining + Random.Range(0, 10000), new RoomOptions { MaxPlayers = maxEggPlayers });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Fiesta Time/ RoomController: Successfully joined room. Entering game...");

        PhotonNetwork.LoadLevel("EggGrabbingGameLobby");
        isConnecting = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Fiesta Time/ RoomController: Room creation failed, reason: " + message + " error code: " + returnCode + ". Try again later.");
    }

    #endregion

    public void ReturnToLobby()
    {
        SceneManager.LoadScene(0);
    }
}
