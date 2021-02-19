using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    /// <summary>
    /// Script in charge of the title screen as well as connecting to the server.
    /// </summary>
    public class Launcher : MonoBehaviourPunCallbacks
    {
        private string gameVersion = "1.0";

        #region UI

        [SerializeField] private InputField nameInput;
        [SerializeField] private Button startButton;
        [SerializeField] private Text connectingText;

        [SerializeField] private bool testingFrames;
        [SerializeField] private int frames;

        #endregion

        #region Unity Callbacks

        void Start()
        {
            Application.targetFrameRate = 90;

            if (testingFrames)
            {
                Application.targetFrameRate = frames;
            }

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
                startButton.interactable = false;
                connectingText.enabled = true;
            }
            else
            {
                startButton.interactable = true;
                connectingText.enabled = false;
            }
            
            if (PlayerPrefs.HasKey(Constants.NAME_KEY_NETWRK))
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString(Constants.NAME_KEY_NETWRK);
                nameInput.text = PlayerPrefs.GetString(Constants.NAME_KEY_NETWRK);
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

        #endregion

        #region PUN Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Fiesta Time/ Launcher: You have successfully connected to the server.");
            startButton.interactable = true;
            connectingText.enabled = false;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Fiesta Time/ Launcher: You have disconnected from the server. Cause: " + cause + " Retrying...");
            PhotonNetwork.ConnectUsingSettings();
            startButton.interactable = false;
            connectingText.enabled = true;
        }

        #endregion

        /// <summary>
        /// Function tied to the Input Field.
        /// Changes the player name in the database. (uses player prefs TODO: Change that).
        /// </summary>
        /// <param name="value"></param>
        public void OnChangeName(string value)
        {
            if (value != "" && value != null)
            {
                nameInput.text = value;
                PlayerPrefs.SetString(Constants.NAME_KEY_NETWRK, value);
                PhotonNetwork.NickName = value;
            }
        }

        /// <summary>
        /// Function tied to the start button.
        /// Connects the player to the master server, takes you to the game selection screen.
        /// </summary>
        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (PlayerPrefs.HasKey(Constants.NAME_KEY_NETWRK) && PlayerPrefs.GetString(Constants.NAME_KEY_NETWRK) != "" && nameInput.text != "")
                    SceneManager.LoadScene(1);
                else
                {
                    Debug.Log("Fiesta Time / Launcher: You havent set your name, set your name to proceed.");
                }
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
                Connect();
            }
        }
    }
}