using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace PartyInc
{
    namespace DD
    {
        /// <summary>
        /// The UI controller in charge of the players UI pieces.
        /// </summary>
        public class Mono_Player_UI_DD : MonoBehaviourPun, IPunObservable
        {
            [SerializeField] private Mono_Player_Controller_DD myPlayer;
            [SerializeField] private Mono_DemonstrationSequence_UI_DD demonstrationSequenceUI;
            [SerializeField] private Mono_Results_UI_DD resultsUI;

            #region Unity Callbacks

            void Start()
            {
                if(myPlayer == null)
                    myPlayer = GetComponentInParent<Mono_Player_Controller_DD>();

                if(demonstrationSequenceUI == null)
                    demonstrationSequenceUI = GetComponentInChildren<Mono_DemonstrationSequence_UI_DD>();

                if(resultsUI == null)
                    resultsUI = GetComponentInChildren<Mono_Results_UI_DD>();
            }

            private void Awake()
            {
                Mng_GameManager_DD.onNextPhase += OnPhaseTransit;
                Mono_Player_Controller_DD.onShowMove += OnMoveShown;
            }

            private void OnDestroy()
            {
                Mng_GameManager_DD.onNextPhase -= OnPhaseTransit;
                Mono_Player_Controller_DD.onShowMove -= OnMoveShown;
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


