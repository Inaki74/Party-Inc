using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    public class Mng_NetworkManager : MonoSingleton<Mng_NetworkManager>
    {
        [SerializeField] private GameObject _dcUI;

        private bool _authValuesReady;
        private bool _runOnce;

        private void Start()
        {
            _authValuesReady = false;

            if (!PhotonNetwork.IsConnected && !_runOnce)
            {
                _runOnce = true;
                StartCoroutine(Connect());
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

        /// <summary>
        /// Initializes the photon local player with custom Authentication.
        /// Using FirebaseAuth's User token 
        /// </summary>
        private void InitializePhotonPlayerWithFirebase()
        {
            StartCoroutine(AwaitForAuthInit());
        }

        private IEnumerator AwaitForAuthInit()
        {
            yield return new WaitUntil(() => PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth != null && PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null);

            InitializePlayer();
        }

        private async void InitializePlayer()
        {
            string uID = await PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.TokenAsync(true);

            Debug.Log("FirebaseAuth user token generated: " + uID);

            AuthenticationValues authValues = new AuthenticationValues();
            authValues.AuthType = CustomAuthenticationType.Custom;
            authValues.AddAuthParameter("user", uID);
            authValues.UserId = uID; // this is required when you set UserId directly from client and not from web service
            PhotonNetwork.AuthValues = authValues;

            _authValuesReady = true;
        }

        private IEnumerator Connect()
        {
            PartyFirebase.Fb_FirebaseManager.Current.InitializeService(InitializePhotonPlayerWithFirebase);

            yield return new WaitUntil(() => _authValuesReady);

            PhotonNetwork.ConnectUsingSettings();
        }

        #region PUN Callbacks

        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            base.OnCustomAuthenticationResponse(data);

            Debug.Log("Fiesta Time/ PhotonManager: Authentication response positive.");
        }

        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            base.OnCustomAuthenticationFailed(debugMessage);

            Debug.Log("Fiesta Time/ PhotonManager: Authentication response negative. " + debugMessage);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Fiesta Time/ PhotonManager: You have successfully connected to the server.");

            if (PhotonNetwork.NickName != PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.DisplayName)
            {
                PhotonNetwork.NickName = PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.DisplayName;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Fiesta Time/ PhotonManager: You have disconnected from the server. Cause: " + cause + " Retrying...");

            // Loads disconnected scene
            _dcUI.SetActive(true);
        }
        
        #endregion
    }
}


