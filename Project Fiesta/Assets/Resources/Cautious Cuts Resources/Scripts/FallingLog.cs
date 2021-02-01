using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace CC
    {
        [ExecuteInEditMode]
        [RequireComponent(typeof(Rigidbody))]
        public class FallingLog : MonoBehaviour
        {
            [SerializeField] private GameObject _empty;
            [SerializeField] private GameObject _hitPoint;

            [SerializeField] private Rigidbody _rb;

            [SerializeField] private GameObject _mark;

            [SerializeField] private float _angle; // Lets make it from -25.00 deg to 25.00 deg
            [SerializeField] private float _startHeight;
            public static float MaximumMarkHeight = 1f;
            public static float MinimumMarkHeight = -1f;
            public static float MaximumMarkAngle = 20f;
            public static float MinimumMarkAngle = -20f;

            private GameObject _nextEmpty;

            public bool IsMine { get; set; }
            public float LogLength { get; set; }

            // Start is called before the first frame update
            void Start()
            {
                if(_rb == null)
                {
                    _rb = GetComponent<Rigidbody>();
                }

                SetMark();
            }

            private void Update()
            {
                //SetMark();
            }

            private void OnDisable()
            {
                transform.rotation = Quaternion.identity;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            private void SetMark()
            {
                Transform mark = _mark.transform;

                mark.eulerAngles = transform.eulerAngles + new Vector3(0f, 0f, _angle);
                mark.position = transform.position + new Vector3(0f, _startHeight, 0f);
            }

            public void CreateEmpty()
            {
                GameObject t = Instantiate(_empty, transform.position, Quaternion.identity);
                t.transform.SetParent(transform);
                _nextEmpty = t;
            }

            public void SetHitpoint(Vector3 point)
            {
                GameObject p = Instantiate(_hitPoint, transform.position + point, Quaternion.identity);
                
                //p.transform.SetParent(_nextEmpty.transform);
            }

            public GameObject GetMark()
            {
                return _mark;
            } 

            public float GetHeight()
            {
                return _mark.transform.localPosition.y;
            }

            public float GetAngle()
            {
                return _angle;
            }
        }
    }
}


