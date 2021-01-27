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

        }
    }
}

