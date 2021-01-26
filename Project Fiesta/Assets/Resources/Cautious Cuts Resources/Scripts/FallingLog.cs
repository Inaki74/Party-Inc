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

            [SerializeField] private GameObject _mark;

            [SerializeField] public GameObject _start;
            [SerializeField] public GameObject _finish;

            [SerializeField] private float _angle; // Lets make it from -25.00 deg to 25.00 deg
            [SerializeField] private float _startHeight; // from -0.70 to 0.70
            [SerializeField] private float _lastHeight;

            public bool IsMine { get; set; }
            public float LogLength { get; set; }

            // Start is called before the first frame update
            void Start()
            {
                if(_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }

                LogLength = Vector3.Distance(_start.transform.position, _finish.transform.position);

                SetMark();
            }

            // Update is called once per frame
            void Update()
            {
                LogLength = Vector3.Distance(_start.transform.position, _finish.transform.position);
                if (transform.position.y < -40f)
                {
                    transform.SetParent(LogPoolManager.Current.LogHolder);
                    _rb.velocity = Vector3.zero;
                    gameObject.SetActive(false);
                }
            }

            private void OnEnable()
            {
                LogLength = Vector3.Distance(_start.transform.position, _finish.transform.position);
            }

            private void SetMark()
            {
                Transform mark = _mark.transform;

                mark.eulerAngles = transform.eulerAngles + new Vector3(0f, 0f, _angle);
                mark.position = transform.position + new Vector3(0f, _startHeight, 0f);
            }
        }
    }
}


