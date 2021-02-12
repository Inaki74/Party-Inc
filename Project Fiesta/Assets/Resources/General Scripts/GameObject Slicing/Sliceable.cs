using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

namespace FiestaTime
{
    namespace Tvtig.Slicer
    {
        /// <summary>
        /// This class and code belongs to Tvtig, aka Thavendren Naicker
        /// https://github.com/Tvtig/UnityLightsaber
        /// https://www.youtube.com/watch?v=BVCNDUcnE1o&ab_channel=Tvtig&t=266s
        /// </summary>
        ///
        public class Sliceable : MonoBehaviour
        {
            [SerializeField]
            private bool _isSolid = true;

            [SerializeField]
            private bool _reverseWindTriangles = false;

            [SerializeField]
            private bool _useGravity = false;

            [SerializeField]
            private bool _shareVertices = false;

            [SerializeField]
            private bool _smoothVertices = false;

            // Mine
            [SerializeField]
            private Vector3 _velocity;

            [SerializeField]
            private bool _slicesHaveColliders = false;

            [SerializeField]
            private bool _destroy = false;

            [SerializeField]
            private bool _deactivateAfterTime = false;

            [SerializeField]
            private float _timeOrDistanceToDeactivate = 0f;

            [SerializeField]
            private bool _slicesHaveConstantForce = false;

            [SerializeField]
            private Vector3 _slicesConstantForce = Vector3.zero;

            public bool IsSolid
            {
                get
                {
                    return _isSolid;
                }
                set
                {
                    _isSolid = value;
                }
            }

            public bool ReverseWireTriangles
            {
                get
                {
                    return _reverseWindTriangles;
                }
                set
                {
                    _reverseWindTriangles = value;
                }
            }

            public bool UseGravity
            {
                get
                {
                    return _useGravity;
                }
                set
                {
                    _useGravity = value;
                }
            }

            public bool ShareVertices
            {
                get
                {
                    return _shareVertices;
                }
                set
                {
                    _shareVertices = value;
                }
            }

            public bool SmoothVertices
            {
                get
                {
                    return _smoothVertices;
                }
                set
                {
                    _smoothVertices = value;
                }
            }

            //Mine

            public Vector3 Velocity
            {
                get
                {
                    return _velocity;
                }
                set
                {
                    _velocity = value;
                }
            }

            public bool SlicesHaveColliders
            {
                get
                {
                    return _slicesHaveColliders;
                }
                set
                {
                    _slicesHaveColliders = value;
                }
            }

            public bool Destroyy
            {
                get
                {
                    return _destroy;
                }
                set
                {
                    _destroy = value;
                }
            }

            public bool DeactivateAfterTime
            {
                get
                {
                    return _deactivateAfterTime;
                }
                set
                {
                    _deactivateAfterTime = value;
                }
            }

            public float TimeOrDistanceToDeactivate
            {
                get
                {
                    return _timeOrDistanceToDeactivate;
                }
                set
                {
                    _timeOrDistanceToDeactivate = value;
                }
            }

            public bool SlicesHaveConstantForce
            {
                get
                {
                    return _slicesHaveConstantForce;
                }
                set
                {
                    _slicesHaveConstantForce = value;
                }
            }

            public Vector3 SlicesConstantForce
            {
                get
                {
                    return _slicesConstantForce;
                }
                set
                {
                    _slicesConstantForce = value;
                }
            }

        }
    }
}

