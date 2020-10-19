using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace RR
    {
        [RequireComponent(typeof(Rigidbody))]
        public class Player : MonoBehaviour
        {
            [SerializeField] private Rigidbody Rb;
            [SerializeField] private CapsuleCollider Cc;

            [SerializeField] private float jumpForce;
            [SerializeField] private float jumpingDrag;

            private float originalDrag;
            private float fallingDrag = -2;

            private bool jumpInput;
            private bool jumpPressed;
            private bool hasLost;
            private bool isGrounded;

            public LayerMask whatIsGround;
            private float distanceGround = 0.3f;
            // Start is called before the first frame update
            void Start()
            {
                if(Rb == null) Rb = GetComponent<Rigidbody>();
                if (Cc == null) Cc = GetComponent<CapsuleCollider>();

                jumpInput = false;
                hasLost = false;
                isGrounded = false;
                originalDrag = Rb.drag;
            }

            // Update is called once per frame
            void Update()
            {
                CheckForInput();
            }

            private void FixedUpdate()
            {
                isGrounded = CheckIfGrounded();
                if (!hasLost && jumpInput && isGrounded)
                {
                    Jump();
                }

                DecideDrag();
            }

            private void CheckForInput()
            {
                jumpInput = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);

                jumpPressed = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);

                if(Input.touchCount > 0)
                {
                    jumpInput = Input.touches[0].phase == TouchPhase.Began;

                    jumpPressed = Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary;
                }
            }

            private void Jump()
            {
                Rb.velocity = Vector3.up * jumpForce;
            }

            private bool CheckIfGrounded()
            {
                RaycastHit hit;

                if(Physics.Raycast(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y/10, 0f), transform.TransformDirection(Vector3.down), out hit, distanceGround, whatIsGround)) {
                    Debug.DrawRay(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), Color.red, hit.distance);
                    return true;
                }
                else
                {
                    Debug.DrawRay(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), Color.green, distanceGround);
                    return false;
                }
            }

            private void DecideDrag()
            {
                if (Rb.velocity.y < 0f)
                {
                    Rb.drag = fallingDrag;
                }
                else if (!hasLost && jumpPressed)
                {
                    Rb.drag = jumpingDrag;
                }
                else
                {
                    Rb.drag = originalDrag;
                }
            }
        }
    }
}
