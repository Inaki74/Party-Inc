using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ObstaclePassCheck_LL : MonoBehaviour
        {
            public delegate void ActionGates();
            public static event ActionGates onPlayerPassed;
            public static event ActionGates onGateRendered;
            public static event ActionGates onGateRenderedBaseCase;

            private Mono_ProceduralGenerator_LL _generator;

            [SerializeField] private Renderer _renderer;

            [SerializeField] private GameObject _raycastMark;

            [SerializeField] private LayerMask _whatIsPlayer;

            private bool _runOnce;
            private bool _seenOnce;

            private void Start()
            {
                if(_renderer == null)
                {
                    _renderer = GetComponent<MeshRenderer>();
                }

                _generator = FindObjectOfType<Mono_ProceduralGenerator_LL>();
            }

            // Update is called once per frame
            void Update()
            {
                if (HitsPlayer() && !_runOnce)
                {
                    _runOnce = true;
                    // Spawn next tube
                    onPlayerPassed.Invoke();
                }

                if (!PhotonNetwork.IsMasterClient) return;

                if (!_seenOnce)
                {
                    CheckIfVisible();
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

            private void CheckIfVisible()
            {
                if (_renderer.IsVisibleFrom(Camera.main))
                {
                    _seenOnce = true;
                    onGateRendered.Invoke();
                    if (_generator.BaseCase) onGateRenderedBaseCase.Invoke();
                }
            }
        }
    }
} 


