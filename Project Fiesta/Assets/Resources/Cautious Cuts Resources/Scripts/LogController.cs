using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fiesta_windowTime
{
    namespace CC
    {
        public class LogController : MonoBehaviour
        {
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private GameObject _feetMarker;

            private Vector3 _startPos;
            private Vector3 _whereToGo;
            private Vector3 _differenceFeetCenter;
            private float _speed;

            private float _windowTime;
            public float WindowTime
            {
                get
                {
                    return _windowTime;
                }
                set
                {
                    _windowTime = value;
                }
            }

            // Start is called before the first frame update
            void Start()
            {
                if (_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }

                _startPos = transform.position;
                _speed = 40f;
                _whereToGo = new Vector3(transform.position.x, 0.5f, transform.position.z);
                _differenceFeetCenter = transform.position - _feetMarker.transform.position;

                WindowTime = 1f;

                _whereToGo += _differenceFeetCenter;
            }

            // Update is called once per frame
            void Update()
            {
                transform.position = Vector3.MoveTowards(transform.position, _whereToGo, Time.deltaTime * _speed);

                if (transform.position == _whereToGo)
                {
                    Debug.Log("EE");
                    StartCoroutine(Wait());
                }
            }

            private IEnumerator Wait()
            {
                yield return new WaitForSeconds(WindowTime);

                _whereToGo = _startPos;

                yield return new WaitUntil(() => transform.position == _whereToGo);

                Destroy(gameObject);
            }

            private void OnEnable()
            {
                //photonView.RPC("RPC_SetActive", RpcTarget.Others, true);
            }

            private void OnDisable()
            {
                //photonView.TransferOwnership(0);
                transform.rotation = Quaternion.identity;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;

                //photonView.RPC("RPC_SetActive", RpcTarget.Others, false);
            }
        }
    }
}