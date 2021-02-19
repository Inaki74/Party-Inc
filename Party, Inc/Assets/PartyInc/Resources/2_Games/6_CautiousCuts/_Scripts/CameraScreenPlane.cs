using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace CC
    {
        public class CameraScreenPlane : MonoBehaviour
        {
            [SerializeField] private GameObject _plane;
            public GameObject Plane
            {
                get
                {
                    return _plane;
                }
                set
                {
                    _plane = value;
                }
            }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}


