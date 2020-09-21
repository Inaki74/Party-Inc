using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        public class ResultsUI : MonoBehaviourPun, IPunObservable
        {
            [SerializeField] private GameObject healthHolder;

            [SerializeField] private RectTransform healthRect;

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

            public void WrongMove(int health)
            {
                if (health == 2)
                {
                    healthRect.localScale = new Vector3(1f, 0.66f, 1f);
                    healthRect.anchoredPosition = new Vector2(-46.4f, -9f);
                }

                if (health == 1)
                {
                    healthRect.localScale = new Vector3(1f, 0.33f, 1f);
                    healthRect.anchoredPosition = new Vector2(-46.4f, -17f);
                }

                if (health == 0)
                {
                    healthRect.localScale = Vector3.zero;
                }

            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    //Debug.Log("Fiesta Time/ DD/ Results UI: Sending over.");

                    stream.SendNext(healthRect.localScale);

                    stream.SendNext(healthRect.anchoredPosition);
                    
                    stream.SendNext(healthHolder.activeInHierarchy);
                }
                else
                {
                    //Debug.Log("Fiesta Time/ DD/ Results UI: Received: ");
                    //Debug.Log("Dato 1:" + stream.ReceiveNext());
                    //Debug.Log("Dato 2:" + stream.ReceiveNext());
                    //Debug.Log("Dato 3:" + stream.ReceiveNext());

                    //stream.Count > 

                    healthRect.localScale = (Vector3)stream.ReceiveNext();
                    healthRect.anchoredPosition = (Vector2)stream.ReceiveNext();

                    healthHolder.SetActive((bool)stream.ReceiveNext());
                }
            }
        }
    }
}


