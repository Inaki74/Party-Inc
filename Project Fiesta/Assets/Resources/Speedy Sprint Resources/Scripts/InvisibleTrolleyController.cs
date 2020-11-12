using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        /// <summary>
        /// The motor that spuns the game forward. Its speed and that of players is the same, always moving forward.
        /// The camera points at it, so in a way, the games speed feel is filtered through it.
        /// This speed, and its increase is decided by the GameManager (since players have the same speed).
        /// It sort of represents the middle line of the game.
        /// It alerts the game when we have gone through a plane.
        /// </summary>
        public class InvisibleTrolleyController : MonoBehaviour
        {
            public delegate void ActionPassedTile();
            public static event ActionPassedTile onPassedSection;

            private int _subsectionCount = 0;
            // Start is called before the first frame update
            void Start()
            {

            }

            private void OnTriggerEnter(Collider other)
            {
                if(other.tag == "CheckpointPlane")
                {
                    Debug.Log("AAAA");
                    _subsectionCount++;

                    if (_subsectionCount >= 5f)
                    {
                        _subsectionCount = 0;
                        onPassedSection?.Invoke();
                    }
                }
            }

            // Update is called once per frame
            void Update()
            {
                transform.position += Vector3.forward * GameManager.Current.MovingSpeed * Time.deltaTime;
            }
        }
    }
}


