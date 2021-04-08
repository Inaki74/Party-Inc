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

        private bool _authValuesReady;

        private bool _photonAuthComplete;
        public bool PhotonAuthComplete
        {
            get
            {
                return _photonAuthComplete;
            }
            private set
            {
                _photonAuthComplete = value;
            }
        }

        private bool _runOnce;

        private void Start()
        {
            Application.targetFrameRate = 60;

            _authValuesReady = false;
            _photonAuthComplete = false;

            if (!PhotonNetwork.IsConnected && !_runOnce)
            {
                _runOnce = true;
                StartCoroutine(Connect());
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

            StartCoroutine("JoinLobbyCo");
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("Fiesta Time/ PhotonManager: You have joined a lobby!");

            _photonAuthComplete = true;
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
            Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.LAUNCHER_SIGNIN);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Fiesta Time/ PhotonManager: You have disconnected from the server. Cause: " + cause + " Retrying...");

            // Loads disconnected scene
            _dcUI.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            StartCoroutine(ReturnToHubCo());
        }

        private IEnumerator ReturnToHubCo()
        {
            yield return StartCoroutine(Mng_SceneNavigationSystem.Current.LoadScenesAsyncAdditive(Mng_SceneNavigationSystem.Current.EssentialHubScenes));

            Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
            Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.GAME_LIST);
        }

        #endregion
    }
}


