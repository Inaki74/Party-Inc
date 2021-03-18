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
    /// Script in charge of the disconnected scene.
    /// </summary>
    public class Mono_NetworkDisconnectedController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject goBackButton;

        private float timeElapsed;

        public override void OnEnable()
        {
            base.OnEnable();

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
            gameObject.SetActive(false);
        }

        #endregion

        public void ReturnToLobby()
        {
            if(PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
            {
                SceneManager.LoadScene(Stt_SceneIndexes.HUB);
            }
            else
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNIN);
            }
            
        }
    }
}