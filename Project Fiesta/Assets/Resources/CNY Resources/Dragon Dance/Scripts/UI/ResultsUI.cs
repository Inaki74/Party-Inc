using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        /// <summary>
        /// The UI controller of the UI in charge of the Results Sequence.
        /// </summary>
        public class ResultsUI : MonoBehaviourPun, IPunObservable
        {
            [SerializeField] private GameObject healthHolder;

            [SerializeField] private RectTransform healthRect;

            #region Unity Callbacks

            private void Awake()
            {
                Player.onWrongMove += WrongMove;
            }

            private void OnDestroy()
            {
                Player.onWrongMove -= WrongMove;
            }

            private void OnEnable()
            {
                healthHolder.SetActive(true);
            }

            private void OnDisable()
            {
                healthHolder.SetActive(false);
            }

            #endregion

            public void WrongMove(int health)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                int maxHealth = GameManager.Current.playersHealth;

                float scaleRatioSubstraction = 1f / maxHealth;

                int healthFromMax = maxHealth - health;

                float yAnchoredPos = scaleRatioSubstraction * healthFromMax * 25;

                healthRect.localScale = new Vector3(1f, 1f - scaleRatioSubstraction * healthFromMax, 1f);
                healthRect.anchoredPosition = new Vector2(-46.4f, -yAnchoredPos);
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(healthRect.localScale);
                    stream.SendNext(healthRect.anchoredPosition);
                    stream.SendNext(healthHolder.activeInHierarchy);
                }
                else
                {
                    healthRect.localScale = (Vector3)stream.ReceiveNext();
                    healthRect.anchoredPosition = (Vector2)stream.ReceiveNext();
                    healthHolder.SetActive((bool)stream.ReceiveNext());
                }
            }
        }
    }
}


