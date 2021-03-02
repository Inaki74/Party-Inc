using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace AS
    {
        public class Mono_ObstaclePassCheck_AS : MonoBehaviour
        {
            public delegate void ActionGates();
            public static event ActionGates onPlayerPassed;
            public static event ActionGates onGateRendered;

            [SerializeField] private Renderer _renderer;

            [SerializeField] private GameObject _raycastMark;

            [SerializeField] private LayerMask _whatIsPlayer;

            private bool _runOnce;
            private bool _seenOnce;

            private void Start()
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<MeshRenderer>();
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (!_seenOnce)
                {
                    CheckIfVisible();
                }

                if (HitsPlayer() && !_runOnce)
                {
                    _runOnce = true;
                    // Spawn next tube
                    onPlayerPassed.Invoke();
                }
            }

            private void OnEnable()
            {
                _seenOnce = false;
                _runOnce = false;
            }

            private void OnDisable()
            {
                _seenOnce = false;
                _runOnce = false;
            }

            private bool HitsPlayer()
            {
                float dist = 5f;
                Ray ray = new Ray(_raycastMark.transform.position, _raycastMark.transform.TransformDirection(Vector3.down));
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, dist, _whatIsPlayer))
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
                }
            }
        }
    }
}
