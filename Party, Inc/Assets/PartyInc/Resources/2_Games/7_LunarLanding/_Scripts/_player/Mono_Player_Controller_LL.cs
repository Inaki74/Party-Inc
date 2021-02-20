using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_Player_Controller_LL : MonoBehaviour
        {
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private Mono_Player_Input_LL _input;

            [SerializeField] private GameObject _top;
            [SerializeField] private GameObject _bottom;
            [SerializeField] private GameObject _spray;

            [SerializeField] private float _sprayStrength;

            private bool _touchingObstacle;

            // Update is called once per frame
            void Update()
            {
                if(!_touchingObstacle) transform.position += Vector3.right * Mng_GameManager_LL.Current.MovementSpeed * Time.deltaTime;

                if(_rb.velocity.x != 0)
                {
                    _rb.velocity = new Vector3(0f,_rb.velocity.y, 0f);
                }

                ProcessInput();
            }

            private void ProcessInput()
            {
                if (_input.SprayUpInput)
                {
                    _rb.AddForce(Vector3.down * _sprayStrength, ForceMode.Force);
                    SprayAnimation(true);
                }

                if (_input.SprayDownInput)
                {
                    _rb.AddForce(Vector3.up * _sprayStrength, ForceMode.Force);
                    SprayAnimation(false);
                }

            }

            private void SprayAnimation(bool topSide)
            {
                GameObject spray;

                if (topSide)
                {
                    spray = Instantiate(_spray, _top.transform.position, Quaternion.identity);
                }
                else
                {
                    spray = Instantiate(_spray, _bottom.transform.position, Quaternion.AngleAxis(180f, Vector3.right));
                }

                ParticleSystem pSys = spray.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = pSys.main;
                main.duration = Time.deltaTime;

                pSys.Play();
            }

            private void OnCollisionEnter(Collision collision)
            {
                if(collision.gameObject.tag == "Obstacle")
                {
                    _touchingObstacle = true;
                }
            }

            private void OnCollisionExit(Collision collision)
            {
                if (collision.gameObject.tag == "Obstacle")
                {
                    _touchingObstacle = false;
                }
            }
        }
    }
}



