using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkRoomSelectController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = "Welcome " + PlayerPrefs.GetString(Constants.NAME_KEY_NETWRK) + "!";

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Still connected");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region PUN Callbacks

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Fiesta Time/ RoomController: You have disconnected from the server. Cause: " + cause + " Retrying...");
        // Loads disconnected scene
        SceneManager.LoadScene(2);
    }

    #endregion

    public void ReturnToLobby()
    {
        SceneManager.LoadScene(0);
    }
}
