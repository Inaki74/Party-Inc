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

        Hashtable playersReady = new Hashtable();

        #region Unity Callbacks

        void Start()
        {
            SetPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);

            if (PhotonNetwork.IsMasterClient) StartCoroutine(StartGameCo());
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

        #region Private Functions

        private void SetPlayerReady(int id)
        {
            if (!playersReady.ContainsKey(id))
            {
                playersReady.Add(id, true);
            }
            else
            {
                playersReady.Remove(id);
                playersReady.Add(id, true);
            }

            photonView.RPC("RPC_SendPlayerReady", RpcTarget.MasterClient, id);
        }

        /// <summary>
        /// The coroutine in charge of starting the game.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartGameCo()
        {
            yield return new WaitUntil(() => PlayersLoadedGame());

            PhotonNetwork.InstantiateRoomObject(gameManager.name, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Returns true if all players have loaded the game.
        /// </summary>
        /// <returns></returns>
        private bool PlayersLoadedGame()
        {
            foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                if (playersReady.ContainsKey(p.ActorNumber))
                {
                    if (!(bool)playersReady[p.ActorNumber])
                    {
                        return false;
                    }
                }
                else return false;
            }

            return true;
        }

        #endregion

        [PunRPC]
        public void RPC_SendPlayerReady(int id)
        {
            if (!playersReady.ContainsKey(id))
            {
                playersReady.Add(id, true);
            }
            else
            {
                playersReady.Remove(id);
                playersReady.Add(id, true);
            }
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