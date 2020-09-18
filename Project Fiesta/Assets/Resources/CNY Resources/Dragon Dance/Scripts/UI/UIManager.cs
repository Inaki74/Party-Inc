using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        public class UIManager : MonoBehaviour
        {
            [SerializeField] private GameObject showSequenceUIHolder;

            //private InputManager
            [SerializeField] private GameObject playerInputUIHolder;
            private PlayerInputUI playerInputUI;

            [SerializeField] private GameObject resultsUIHolder;
            //private ResultsUI resultsUI;

            private Player myPlayer;

            // Start is called before the first frame update
            void Start()
            {
                playerInputUI = playerInputUIHolder.GetComponent<PlayerInputUI>();
                FindMyPlayer();
                GameManager.Current.NotifyOfPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
            }

            private void Awake()
            {
                GameManager.onNextPhase += OnPhaseTransit;
                InputManager.onMoveMade += OnInputTaken;
            }

            private void OnDestroy()
            {
                GameManager.onNextPhase -= OnPhaseTransit;
                InputManager.onMoveMade -= OnInputTaken;
            }

            private void FindMyPlayer()
            {
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (g.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        myPlayer = g.GetComponent<Player>();
                    }
                }
            }

            private void OnInputTaken(int number)
            {
                playerInputUI.TriggerInputIndicator(number);
            }

            private void OnPhaseTransit(int nextPhase)
            {
                // 0 -> entered showSeq, 1 -> entered playerInput, 2 -> entered demoSeq, 3 -> entered results
                if (myPlayer.hasLost && nextPhase != 3) return;

                switch (nextPhase)
                {
                    case 0:
                        // Deactivate all pieces of Results phase (in player.cs)
                        //resultsUIHolder.SetActive(false);

                        // Activate all pieces of Showing Sequence phase
                        showSequenceUIHolder.SetActive(true);

                        break;
                    case 1:
                        // Deactivate all pieces of Showing Sequence phase
                        showSequenceUIHolder.SetActive(false);

                        // Activate all pieces of Input Phase phase
                        playerInputUIHolder.SetActive(true);

                        break;
                    case 2:
                        // Deactivate all pieces of Input phase
                        playerInputUIHolder.SetActive(false);

                        // Activate all pieces of demonstration phase (moved to playerUI)
                        //demonstrationSequenceUI.enabled = true;
                        //resultsUI.enabled = true;

                        break;
                    case 3:
                        // Deactivate all pieces of demonstration phase (moved to playerUI)
                        //demonstrationSequenceUI.enabled = false;
                        //resultsUI.enabled = false;

                        // Activate all pieces of Results phase (in player.cs)
                        //resultsUIHolder.SetActive(true);

                        break;
                    case 4:
                        // Game finished
                        break;
                    default:
                        Debug.Log("ITS NOT POSSIBLEEEEE!");
                        break;
                }
            }
        }
    }
}

