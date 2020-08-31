using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class eggb_NetworkRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text playerOneText;
    [SerializeField] private Text playerTwoText;
    [SerializeField] private Text countdownText;

    private float countdownTime = 15f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString(Constants.CHRCTR_KEY_NETWRK, Constants.BUNNY_NAME_CHRCTR);
        if (PhotonNetwork.IsMasterClient)
        {
            playerOneText.text = PhotonNetwork.NickName;
        }
        playerTwoText.text = "Searching";
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = "" + countdownTime;
        }

        if(countdownTime < 0f)
        {
            PhotonNetwork.LoadLevel("EggGrabbingGameB");
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Button Functions

    public void BtnLeaveRoom()
    {
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
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Fiesta Time/ EGG/ NetworkRoomController: Bye bye! " + otherPlayer.NickName);
        playerTwoText.text = "Searching...";
    }
}
