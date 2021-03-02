using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_Player_Controller_KK : MonoBehaviour
        {
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private Mono_Player_Input_KK _input;

            [SerializeField] private float _force;

            // Start is called before the first frame update
            void Start()
            {
                if(_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }
            }

            private void FixedUpdate()
            {
                if (_input.JumpInput)
                {
                    _input.JumpInput = false;
                    _rb.velocity = Vector3.zero;
                    _rb.AddForce(Vector3.up * _force, ForceMode.Impulse);
                }
            }

            // Update is called once per frame
            void Update()
            {
                transform.position += Vector3.right * Mng_GameManager_KK.Current.MovementSpeed * Time.deltaTime;

                
            }
        }
    }
}


