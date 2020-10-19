using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace RR
    {
        public class RopeLinkM : MonoBehaviour
        {
            // Transform of the xz plane.
            [SerializeField] private GameObject groundPlane;
            private RopeControllerM ropeController;
            [SerializeField] private SphereCollider Sc;

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

            public RopeLinkM Init(RopeControllerM rope, float rad, float z, int pos, float angle)
            {
                if (Sc == null) Sc = GetComponent<SphereCollider>();

                ropeController = rope;
                radius = rad;
                currentAngle = angle;
                transform.localPosition = new Vector3(radius, 0f, z);
                positionInArray = pos;

                distanceToGrounded = 2 * Sc.bounds.extents.y;

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
                if(collision.gameObject.tag == "Player")
                {
                    Debug.Log("TOuched you yo.");
                    // Kill player basically
                    collision.rigidbody.AddForce((transform.position - lastPosition) * ropeController.rotationSpeed, ForceMode.Impulse);
                }
            }

            private bool CheckIfWillBeGrounded()
            {
                // If this cycle is beneath the threshold or if the next cycle is beneath the threshold
                return transform.position.y + Mathf.Sin(currentAngle + Time.deltaTime * ropeController.rotationSpeed) * radius < -distanceToPlane + distanceToGrounded;
            }

            private void MoveLink()
            {
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
                        distanceGround =  -distanceToPlane + distanceToGrounded;

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

                if (currentAngle > Mathf.PI * 2)
                {
                    currentAngle = 0f;
                }

                if (currentAngle < 0)
                {
                    currentAngle = Mathf.PI * 2;
                }

                currentAngle += Time.deltaTime * ropeController.rotationSpeed;
            }
        }
    }
}


