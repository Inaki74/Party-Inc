using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace CC
    {
        [ExecuteInEditMode]
        [RequireComponent(typeof(Rigidbody))]
        public class FallingLog : MonoBehaviourPun
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

            [SerializeField] private CapsuleCollider _cc;

            [SerializeField] private GameObject _empty;
            [SerializeField] private GameObject _hitPoint;

            [SerializeField] private GameObject _mark;

            [SerializeField] private bool _setMarkEditor;

            [SerializeField] private float _angle; // Lets make it from -25.00 deg to 25.00 deg
            [SerializeField] private float _startHeight;
            [SerializeField] private float _startWidth;

            public static float LargeBoundH = 2f;
            public static float MediumBoundH = 1.5f;
            public static float SmallBoundH = 1f;
            public static float VerySmallBoundH = 0.5f;

            public static float AngleBoundH = 25f; // -25deg - 25deg
            public static float AngleMaxBoundV = 90f; // -70deg - 70deg
            public static float AngleMinBoundV = 70f;

            public static float MaximumMarkHeight = 1f;
            public static float MinimumMarkHeight = -1f;
            public static float MaximumMarkWidth = 0.5f;
            public static float MinimumMarkWidth = -0.5f;
            public static float MaximumMarkAngleHorizontal = 50f;
            public static float MinimumMarkAngleHorizontal = -50f;
            public static float MaximumMarkAngleVertical = 110f;
            public static float MinimumMarkAngleVertical = 70f;

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
            public float StartHeight
            {
                get
                {
                    return _startHeight;
                }
                set
                {
                    _startHeight = value;
                }
            }
            public float StartWidth
            {
                get
                {
                    return _startWidth;
                }
                set
                {
                    _startWidth = value;
                }
            }
            public float Angle
            {
                get
                {
                    return _angle;
                }
                set
                {
                    _angle = value;
                }
            }
            public bool IsMine { get; private set; }
            public float LogLength { get; private set; }

            // Start is called before the first frame update
            void Start()
            {
                if(_cc == null)
                {
                    _cc = GetComponent<CapsuleCollider>();
                }

                if(!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    _cc.enabled = false;
                }

                SetMarkNet();
            }

            private void Update()
            {
                if(_setMarkEditor) SetMark();
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

            private void SetMarkNet()
            {
                SetMark();

                if (PhotonNetwork.IsConnected)
                {
                    // Apply over the network
                    photonView.RPC("RPC_SendMark", RpcTarget.Others, _startHeight, _startWidth, _angle);
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

            //// NETWORKING
            ///

            [PunRPC]
            public void RPC_SetActive(bool act)
            {
                gameObject.SetActive(act);
            }

            [PunRPC]
            public void RPC_SendMark(float height, float width, float angle)
            {
                _startHeight = height;
                _startWidth = width;
                _angle = angle;
                SetMark();
            }
        }
    }
}


