using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
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

            private void SetArrow(int sequence)
            {
                float rotation = 270;
                if (sequence == 2) rotation = 0;
                if (sequence == 3) rotation = 90;
                if (sequence == 4) rotation = 180;

                arrowImage.Rotate(new Vector3(0, 0, rotation));
            }

            private void ResetArrow()
            {
                arrowImage.rotation = new Quaternion(0, 0, 0, 1);
            }

            private IEnumerator ShowSequence(int elements)
            {
                for(int i = 0; i < elements; i++)
                {
                    arrowImage.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                    SetArrow(GameManager.Current.sequenceMap[i]);

                    arrow.SetActive(true);

                    StartCoroutine(AnimateArrowCo());

                    yield return new WaitForSeconds(1f);

                    ResetArrow();
                }
                arrow.SetActive(false);

                GameManager.Current.NotifyOfPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
            }

            private IEnumerator AnimateArrowCo()
            {
                for(int i = 0; i < 70; i++)
                {
                    arrowImage.localScale += new Vector3(0.00714285714f, 0.00714285714f, 0.00714285714f);

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
}


