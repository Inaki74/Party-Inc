using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace CC
    {
        [RequireComponent(typeof(Rigidbody))]
        public class FallingLog : MonoBehaviour
        {
            [SerializeField] private Rigidbody _rb;

            // Start is called before the first frame update
            void Start()
            {
                if(_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (transform.position.y < -40f)
                {
                    transform.SetParent(LogPoolManager.Current.LogHolder);
                    _rb.velocity = Vector3.zero;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}


