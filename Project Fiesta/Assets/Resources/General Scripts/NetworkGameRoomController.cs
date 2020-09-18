using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    public class NetworkGameRoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject gameManager;

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate(gameManager.name, Vector3.zero, Quaternion.identity);
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


        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("Fiesta Time/ EGG/ NetworkController: Bye bye! " + otherPlayer.NickName);
            // If a player left, the other player is alone
            // - We could cancel all processes and abandon the room.
            // - Or we could continue the game, as if nothing happened.
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Fiesta Time/ NetworkController: You have disconnected from the server. Cause: " + cause + " Retrying...");
            // If you disconnected abruptly we could:
            // - Keep it that way
            // - Or we could try to recover from the situation (dont know how to do it) TODO
        }

        #endregion
    }
}