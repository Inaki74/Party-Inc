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
            public enum LogClass
            {
                Large,
                Medium,
                Small_Horizontal,
                VerySmall_Horizontal,
                Small_Vertical,
                VerySmall_Vertical
            }

            [SerializeField] private LogClass _logType;

            [SerializeField] private GameObject _empty;
            [SerializeField] private GameObject _hitPoint;

            [SerializeField] private Rigidbody _rb;

            [SerializeField] private GameObject _mark;

            [SerializeField] private bool _setMarkEditor;

            [SerializeField] private float _angle; // Lets make it from -25.00 deg to 25.00 deg
            [SerializeField] private float _startHeight;
            [SerializeField] private float _startWidth;

            public static float MaximumMarkHeight = 1f;
            public static float MinimumMarkHeight = -1f;
            public static float MaximumMarkWidth = 0.45f;
            public static float MinimumMarkWidth = -0.45f;
            public static float MaximumMarkAngle = 20f;
            public static float MinimumMarkAngle = -20f;

            private GameObject _nextEmpty;

            public LogClass LogType
            {
                get
                {
                    return _logType;
                }
                private set
                {
                    _logType = value;
                }
            }
            public bool IsMine { get; private set; }
            public float LogLength { get; private set; }

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
                if(_setMarkEditor) SetMark();
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

                if(LogType == LogClass.Large ||
                   LogType == LogClass.Medium ||
                   LogType == LogClass.Small_Horizontal ||
                   LogType == LogClass.VerySmall_Horizontal)
                {
                    mark.position = transform.position + new Vector3(0f, _startHeight, 0f);
                }

                if (LogType == LogClass.Small_Vertical ||
                    LogType == LogClass.VerySmall_Vertical)
                {
                    mark.position = transform.position + new Vector3(_startWidth, 0f, 0f);
                }
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

            public float GetWidth()
            {
                return _mark.transform.localPosition.x;
            }
        }
    }
}


