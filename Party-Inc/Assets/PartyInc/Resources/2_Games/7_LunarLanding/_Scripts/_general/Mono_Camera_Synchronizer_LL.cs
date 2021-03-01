using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    namespace LL
    {
        public struct CameraInfo
        {
            public Vector3 position;
            public Vector3 rbVelocity;
        }

        public class Mono_Camera_Synchronizer_LL : MonoBehaviourPun
        {
            [SerializeField] private Transform _irrelevantPoint;
            public Transform IrrelevantPoint
            {
                get
                {
                    return _irrelevantPoint;
                }
                private set
                {
                    _irrelevantPoint = value;
                }
            }

            [SerializeField] private Transform _irrelevantPointTwo;
            public Transform IrrelevantPointTwo
            {
                get
                {
                    return _irrelevantPointTwo;
                }
                private set
                {
                    _irrelevantPointTwo = value;
                }
            }

            [SerializeField] private Transform _spawningPoint;
            public Transform SpawningPoint
            {
                get
                {
                    return _spawningPoint;
                }
                private set
                {
                    _spawningPoint = value;
                }
            }

            public bool ChangingOwner { get; set; }

            private void Update()
            {
                _irrelevantPoint.transform.position = new Vector3(_irrelevantPointTwo.transform.position.x + Mng_GameManager_LL.Current.MaxX, _irrelevantPoint.transform.position.y, _irrelevantPoint.transform.position.z); ;
            }
        }
    }
}



