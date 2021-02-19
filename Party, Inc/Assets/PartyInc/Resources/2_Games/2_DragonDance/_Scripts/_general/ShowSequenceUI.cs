using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace PlayInc
{
    namespace DD
    {
        /// <summary>
        /// Component in charge of the UI in the show sequence stage of the game.
        /// </summary>
        public class ShowSequenceUI : MonoBehaviour
        {
            [SerializeField] private RectTransform arrowImage;
            [SerializeField] private GameObject arrow;

            //[SerializeField] private Image rightArrowImage;
            //[SerializeField] private Image upArrowImage;
            //[SerializeField] private Image leftArrowImage;
            //[SerializeField] private Image downArrowImage;

            private void OnEnable()
            {
                StartCoroutine(ShowSequence(GameManager.Current.amountOfMovesThisRound));
            }

            /// <summary>
            /// Changes the arrows rotation to match that of the generated sequence.
            /// </summary>
            /// <param name="sequence"></param>
            private void SetArrow(int sequence)
            {
                float rotation = 270;
                if (sequence == 2) rotation = 0;
                if (sequence == 3) rotation = 90;
                if (sequence == 4) rotation = 180;

                arrowImage.Rotate(new Vector3(0, 0, rotation));
            }

            /// <summary>
            /// Resets the arrows position back to its original.
            /// </summary>
            private void ResetArrow()
            {
                arrowImage.rotation = new Quaternion(0, 0, 0, 1);
            }

            /// <summary>
            /// The coroutine that shows the players the sequence generated.
            /// </summary>
            /// <param name="elements"></param>
            /// <returns></returns>
            private IEnumerator ShowSequence(int elements)
            {
                for(int i = 0; i < elements; i++)
                {
                    arrowImage.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                    SetArrow(GameManager.Current.sequenceMap[i]);

                    arrow.SetActive(true);

                    StartCoroutine(UIFunctions.GrowthAnimationCo(arrowImage, 1, (70 * GameManager.Current.timeToSeeMove) / 100));

                    yield return new WaitForSeconds(GameManager.Current.timeToSeeMove);

                    ResetArrow();
                }
                arrow.SetActive(false);

                Debug.Log("ShowSequence finish");
                //GameManager.Current.NotifyOfRemotePlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
                GameManager.Current.NotifyOfLocalPlayerReady();
            }
        }
    }
}


