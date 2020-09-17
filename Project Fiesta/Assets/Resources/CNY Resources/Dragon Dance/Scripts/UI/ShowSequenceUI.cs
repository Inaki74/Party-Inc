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
            [SerializeField] private RectTransform[] arrowImages;
            [SerializeField] private GameObject[] arrows;

            //[SerializeField] private Image rightArrowImage;
            //[SerializeField] private Image upArrowImage;
            //[SerializeField] private Image leftArrowImage;
            //[SerializeField] private Image downArrowImage;

            // Start is called before the first frame update
            void Start()
            {
                SetArrows();
            }

            private void OnEnable()
            {
                IndicatorFunctions.DisableAllIndicators(arrows);
                StartCoroutine(ShowSequence(GameManager.Current.amountOfMovesThisRound));
            }

            private void OnDisable()
            {
                IndicatorFunctions.DisableAllIndicators(arrows);
            }

            private void SetArrows()
            {
                for (int i = 0; i < GameManager.Current.sequenceMap.Length; i++)
                {
                    int rotation = 270;
                    if (GameManager.Current.sequenceMap[i] == 2) rotation = 0;
                    if (GameManager.Current.sequenceMap[i] == 3) rotation = 90;
                    if (GameManager.Current.sequenceMap[i] == 4) rotation = 180;

                    arrowImages[i].Rotate(new Vector3(0, 0, rotation));
                }
            }

            private IEnumerator ShowSequence(int elements)
            {
                yield return new WaitForSeconds(1f);

                for(int i = 0; i < elements; i++)
                {
                    arrows[i].SetActive(true);

                    yield return new WaitForSeconds(0.5f);
                }

                GameManager.Current.NotifyOfPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }
}


