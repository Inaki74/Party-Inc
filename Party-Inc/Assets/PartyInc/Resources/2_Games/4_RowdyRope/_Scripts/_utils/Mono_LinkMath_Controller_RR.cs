﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace RR
    {
        /// <summary>
        /// The link moves mathematically and has a big pre-condition: the ground and the rope are both parallel.
        /// </summary>
        public class Mono_LinkMath_Controller_RR : MonoBehaviour
        {
            // Transform of the xz plane.
            [SerializeField] private GameObject groundPlane;
            private Mono_RopeMath_Controller_RR ropeController;
            [SerializeField] private CapsuleCollider Cc;

            private int positionInArray;

            private float distanceToPlane;

            private Vector3 lastPosition;

            private bool isTouchingGround;
            private bool willTouchGround;
            private float distanceToGrounded;

            private float radius;
            private float currentAngle;
            private float firstGroundAngle;
            private float distanceGround;


            public Mono_LinkMath_Controller_RR Init(Mono_RopeMath_Controller_RR rope, float rad, float z, int pos, float angle)
            {
                if (Cc == null) Cc = GetComponent<CapsuleCollider>();

                ropeController = rope;
                radius = rad;
                currentAngle = angle;
                transform.localPosition = new Vector3(0f, radius, z);
                positionInArray = pos;

                distanceToGrounded = 3 * Cc.bounds.extents.y;

                distanceToPlane = Vector3.Distance(ropeController.gameObject.transform.position, groundPlane.transform.position);

                return this;
            }

            // Update is called once per frame
            void Update()
            {
                willTouchGround = CheckIfWillBeGrounded();

                distanceToPlane = Vector3.Distance(ropeController.gameObject.transform.position, groundPlane.transform.position);

                lastPosition = transform.position;

                MoveLink();
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.tag == "Player")
                {
                    // Kill player basically
                    collision.gameObject.GetComponent<Mono_Player_Controller_RR>().Die((transform.position - lastPosition) * ropeController.rotationSpeed);
                }
            }

            /// <summary>
            /// Checks if the link will be below ground on the next frame.
            /// </summary>
            /// <returns></returns>
            private bool CheckIfWillBeGrounded()
            {
                // If this cycle is beneath the threshold or if the next cycle is beneath the threshold
                return transform.position.y + Mathf.Sin(currentAngle + Time.deltaTime * ropeController.rotationSpeed) * radius < -distanceToPlane + groundPlane.GetComponent<MeshCollider>().bounds.extents.y + distanceToGrounded;
            }

            /// <summary>
            ///  Moves the link mathematically.
            /// </summary>
            private void MoveLink()
            {
                currentAngle = ropeController.angle;

                Vector3 nextPos = new Vector3(Mathf.Cos(currentAngle) * radius,
                                              Mathf.Sin(currentAngle) * radius,
                                              transform.localPosition.z);

                if (isTouchingGround)
                {
                    if ((ropeController.rotationSpeed > 0f && currentAngle > Mathf.PI - firstGroundAngle) ||
                        (ropeController.rotationSpeed < 0f && currentAngle < Mathf.PI - firstGroundAngle))
                    {
                        transform.localPosition = nextPos;
                        isTouchingGround = false;
                    }
                    else
                    {
                        transform.localPosition = new Vector3(Mathf.Cos(currentAngle) * radius,
                                                          distanceGround,
                                                          transform.localPosition.z);
                    }
                }
                else
                {
                    if (willTouchGround)
                    {
                        // Define distance ground
                        distanceGround =  -distanceToPlane + +groundPlane.GetComponent<MeshCollider>().bounds.extents.y + distanceToGrounded;

                        transform.localPosition = new Vector3(Mathf.Cos(currentAngle) * radius,
                                                          distanceGround,
                                                          transform.localPosition.z);
                        isTouchingGround = true;
                        firstGroundAngle = currentAngle;
                    }
                    else
                    {
                        transform.localPosition = nextPos;
                    }

                }
            }
        }
    }
}


