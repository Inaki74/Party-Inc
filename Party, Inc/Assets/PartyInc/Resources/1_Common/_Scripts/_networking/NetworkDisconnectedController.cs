using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PlayInc
{
    /// <summary>
    /// Script in charge of the disconnected scene.
    /// </summary>
    public class NetworkDisconnectedController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject goBackButton;

        private float timeElapsed;

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();

            timeElapsed = 0f;

            goBackButton.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            timeElapsed += Time.deltaTime;

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }

            if(timeElapsed > 3f)
            {
                goBackButton.SetActive(true);
            }
        }

        #region PUN Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Fiesta Time/ DisconnectedController: Successfully reconnected!");
            // Reloads lobby
            SceneManager.LoadScene(1);
        }

        #endregion

        public void ReturnToLobby()
        {
            SceneManager.LoadScene(0);
        }
    }
}