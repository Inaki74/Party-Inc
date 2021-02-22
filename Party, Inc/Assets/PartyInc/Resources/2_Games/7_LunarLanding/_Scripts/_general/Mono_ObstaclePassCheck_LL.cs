using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ObstaclePassCheck_LL : MonoBehaviour
        {
            public delegate void ActionPlayerPassed();
            public static event ActionPlayerPassed onPlayerPassed;

            [SerializeField] private GameObject _raycastMark;

            [SerializeField] private LayerMask _whatIsPlayer;

            private bool _runOnce;

            // Update is called once per frame
            void Update()
            {
                if(HitsPlayer() && !_runOnce)
                {
                    _runOnce = true;
                    // Spawn next tube
                    onPlayerPassed.Invoke();
                }
            }

            private bool HitsPlayer()
            {
                float dist = 5f;
                Ray ray = new Ray(_raycastMark.transform.position, _raycastMark.transform.TransformDirection(Vector3.down));
                RaycastHit rayHit;
                if(Physics.Raycast(ray, out rayHit, dist, _whatIsPlayer))
                {
                    Debug.DrawRay(ray.origin, ray.direction * rayHit.distance, Color.red);
                    return true;
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * dist, Color.green);
                    return false;
                }
            }
        }
    }
} 


