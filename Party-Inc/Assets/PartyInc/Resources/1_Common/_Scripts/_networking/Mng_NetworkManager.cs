using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    public class Mng_NetworkManager : MonoSingleton<Mng_NetworkManager>
    {
        [SerializeField] private GameObject _dcUI;

        public override void Init()
        {
            base.Init();

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
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


        #region PUN Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Fiesta Time/ Launcher: You have successfully connected to the server.");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Fiesta Time/ Launcher: You have disconnected from the server. Cause: " + cause + " Retrying...");

            
            // Loads disconnected scene
            _dcUI.SetActive(true);
        }
        
        #endregion
    }
}


