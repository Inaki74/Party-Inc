using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using UnityEngine.UI;

namespace FiestaTime
{
    namespace DD
    {
        public class DemonstrationSequenceUI : MonoBehaviourPun
        {
            public Image[] feedbackIndicatorsImages;

            public GameObject[] feedbackIndicators;

            public GameObject holder;

            private bool startup = true;

            private void OnEnable()
            {
                if (!startup)
                {
                    holder.SetActive(true);
                    IndicatorFunctions.DisableAllIndicators(feedbackIndicators);
                    IndicatorFunctions.EnableIndicators(feedbackIndicators, GameManager.Current.amountOfMovesThisRound);
                    photonView.RPC("RPC_SendEnable", RpcTarget.Others, GameManager.Current.amountOfMovesThisRound);
                }
                startup = false;
            }

            private void OnDisable()
            {
                IndicatorFunctions.ResetColors(feedbackIndicatorsImages, new Color(1, 1, 1));
                holder.SetActive(false);
                photonView.RPC("RPC_SendDisable", RpcTarget.Others);
            }

            public void TriggerFeedbackIndicator(bool isRight, int moveNumber)
            {
                if (isRight)
                {
                    IndicatorFunctions.TriggerInputIndicator(feedbackIndicatorsImages[moveNumber], new Color(0f, 1f, 0f));
                }
                else
                {
                    IndicatorFunctions.TriggerInputIndicator(feedbackIndicatorsImages[moveNumber], new Color(1f, 0f, 0f));
                }
                photonView.RPC("RPC_SendIfRight", RpcTarget.Others, new object[] { isRight, moveNumber });
            }

            //TODO: PunRPCs dont work on children.

            [PunRPC]
            public void RPC_SendIfRight(object[] args)
            {
                if ((bool)args[0])
                {
                    IndicatorFunctions.TriggerInputIndicator(feedbackIndicatorsImages[(int)args[1]], new Color(0f, 1f, 0f));
                }
                else
                {
                    IndicatorFunctions.TriggerInputIndicator(feedbackIndicatorsImages[(int)args[1]], new Color(1f, 0f, 0f));
                }
            }

            [PunRPC]
            public void RPC_SendDisable()
            {
                IndicatorFunctions.ResetColors(feedbackIndicatorsImages, new Color(1, 1, 1));
                holder.SetActive(false);
            }

            [PunRPC]
            public void RPC_SendEnable(int amount)
            {
                holder.SetActive(true);
                IndicatorFunctions.DisableAllIndicators(feedbackIndicators);
                IndicatorFunctions.EnableIndicators(feedbackIndicators, amount);
            }
        }
    }
}


