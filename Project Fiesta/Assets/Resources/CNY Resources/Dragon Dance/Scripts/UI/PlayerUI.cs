using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        /// <summary>
        /// The UI controller in charge of the players UI pieces.
        /// </summary>
        public class PlayerUI : MonoBehaviourPun, IPunObservable
        {
            [SerializeField] private Player myPlayer;
            [SerializeField] private DemonstrationSequenceUI demonstrationSequenceUI;
            [SerializeField] private ResultsUI resultsUI;

            #region Unity Callbacks

            void Start()
            {
                if(myPlayer == null)
                    myPlayer = GetComponentInParent<Player>();

                if(demonstrationSequenceUI == null)
                    demonstrationSequenceUI = GetComponentInChildren<DemonstrationSequenceUI>();

                if(resultsUI == null)
                    resultsUI = GetComponentInChildren<ResultsUI>();
            }

            private void Awake()
            {
                GameManager.onNextPhase += OnPhaseTransit;
                Player.onShowMove += OnMoveShown;
            }

            private void OnDestroy()
            {
                GameManager.onNextPhase -= OnPhaseTransit;
                Player.onShowMove -= OnMoveShown;
            }

            #endregion

            /// <summary>
            /// Event function that triggers when a move must be shown.
            /// </summary>
            /// <param name="isRight"></param>
            /// <param name="moveNumber"></param>
            private void OnMoveShown(bool isRight, int moveNumber)
            {
                //demoSeqUI should be attached to each player individually, furthermore whether they made the right or wrong choice
                //should be visible to everyone and so its independent to everyone, therefore it must be synchronized over the network.
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                demonstrationSequenceUI.TriggerFeedbackIndicator(isRight, moveNumber);
            }

            /// <summary>
            /// The function triggered by the phase changing. Acts on everything
            /// </summary>
            /// <param name="nextPhase"></param>
            private void OnPhaseTransit(int nextPhase)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                // 0 -> entered showSeq, 1 -> entered playerInput, 2 -> entered demoSeq, 3 -> entered results
                if (myPlayer.hasLost && nextPhase != 3) return;

                switch (nextPhase)
                {
                    case 2:
                        // Activate all pieces of demonstration phase
                        demonstrationSequenceUI.enabled = true;
                        resultsUI.enabled = true;

                        break;
                    case 3:
                        // Deactivate all pieces of demonstration phase
                        demonstrationSequenceUI.enabled = false;
                        resultsUI.enabled = false;

                        break;
                    case 4:
                        // Game finished
                        break;
                    default:
                        break;
                }
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    //Debug.Log("Fiesta Time/ DD/ Player UI: Sending over.");

                    stream.SendNext(demonstrationSequenceUI.enabled);

                    stream.SendNext(resultsUI.enabled);
                }
                else
                {
                    //Debug.Log("Fiesta Time/ DD/ Player UI: Received.");

                    demonstrationSequenceUI.enabled = (bool)stream.ReceiveNext();

                    resultsUI.enabled = (bool)stream.ReceiveNext();
                }
            }
        }
    }

}


