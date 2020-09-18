using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        public class PlayerUI : MonoBehaviourPun, IPunObservable
        {
            private Player myPlayer;
            private DemonstrationSequenceUI demonstrationSequenceUI;
            private ResultsUI resultsUI;

            // Start is called before the first frame update
            void Start()
            {
                myPlayer = GetComponentInParent<Player>();

                demonstrationSequenceUI = GetComponentInChildren<DemonstrationSequenceUI>();
                demonstrationSequenceUI.enabled = false;

                resultsUI = GetComponentInChildren<ResultsUI>();
                resultsUI.enabled = false;
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

            private void OnMoveShown(bool isRight, int moveNumber)
            {
                //demoSeqUI should be attached to each player individually, furthermore whether they made the right or wrong choice
                //should be visible to everyone and so its independent to everyone, therefore it must be synchronized over the network.
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                demonstrationSequenceUI.TriggerFeedbackIndicator(isRight, moveNumber);
            }

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
                    stream.SendNext(demonstrationSequenceUI.enabled);
                    stream.SendNext(resultsUI.enabled);
                }
                else
                {
                    demonstrationSequenceUI.enabled = (bool)stream.ReceiveNext();
                    resultsUI.enabled = (bool)stream.ReceiveNext();
                }
            }
        }
    }

}


