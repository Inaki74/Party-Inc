using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace SS
    {
        /// <summary>
        /// The motor that spuns the game forward. Its speed and that of players is the same, always moving forward.
        /// The camera points at it, so in a way, the games speed feel is filtered through it.
        /// This speed, and its increase is decided by the Mng_GameManager_SS (since players have the same speed).
        /// It sort of represents the middle line of the game.
        /// It alerts the game when we have gone through a plane.
        /// </summary>
        public class Mono_InvisibleTrolley_SS : MonoBehaviour
        {
            public delegate void ActionPassedTile();
            public static event ActionPassedTile onPassedSection;

            [SerializeField] private LayerMask _checkpointLayerMask;
            private bool _passedSection = true;

            private int _subsectionCount = 0;

            private int _count = 0;

            private void Start()
            {
                Debug.Log("SUB-SECTION PASSED: " + 0 + " TIME: " + 0 + " MOVING SPEED: " + 10);
            }

            // Update is called once per frame
            private void Update()
            {
                transform.position += Vector3.forward * Mng_GameManager_SS.Current.MovingSpeed * Time.deltaTime;
            }

            private void FixedUpdate()
            {
                CheckForCheckpoints();
            }

            private void CheckForCheckpoints()
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 20f, 1<<9))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green, 0f);
                    if (hit.distance < 2f && _passedSection)
                    {
                        _passedSection = false;
                        _subsectionCount++;

                        _count++;
                        Debug.Log("SUB-SECTION PASSED: " + _count + " TIME: " + Mng_GameManager_SS.Current.InGameTime + " MOVING SPEED: " + Mng_GameManager_SS.Current.MovingSpeed);

                        if (_subsectionCount >= 2)
                        {
                            _subsectionCount = 0;
                            onPassedSection?.Invoke();
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20f, Color.red, 0f);
                    _passedSection = true;
                }
            }
        }
    }
}


