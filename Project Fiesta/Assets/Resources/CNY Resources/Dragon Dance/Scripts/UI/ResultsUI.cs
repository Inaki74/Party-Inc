using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace DD
    {
        public class ResultsUI : MonoBehaviour
        {
            [SerializeField] private GameObject healthHolder;

            [SerializeField] private RectTransform healthRect;

            private bool startup = true;

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
                if(!startup) healthHolder.SetActive(true);

                startup = false;
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
        }
    }
}


