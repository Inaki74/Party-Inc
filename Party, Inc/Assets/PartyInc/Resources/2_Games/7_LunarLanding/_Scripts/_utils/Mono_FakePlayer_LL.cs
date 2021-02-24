using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PartyInc
{
    namespace LL
    {
        public class Mono_FakePlayer_LL : MonoBehaviour
        {
            [SerializeField] private Rigidbody _rb;
            [SerializeField] private Mono_Camera_Synchronizer_LL _trolleySync;
            [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;

            private Transform _playerThatHasCamera;
            private float _originalX;
            private bool _lostCamera;


            // Update is called once per frame
            void FixedUpdate()
            {
                _originalX = _trolleySync.IrrelevantPoint.position.x;
                if (_lostCamera)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(_playerThatHasCamera.position.x, transform.position.y, 0f), Time.deltaTime * 10f);
                }
                else
                {
                    _rb.velocity = new Vector3(Mng_GameManager_LL.Current.MovementSpeed, 0f, 0f);
                }
            }

            public void LoseCamera(Transform player)
            {
                _playerThatHasCamera = player;
                _lostCamera = true;
            }

            public void RegainCamera()
            {
                _camera.Follow = transform;
                _playerThatHasCamera = null;
                _lostCamera = false;
                transform.position = Vector3.Lerp(transform.position, Vector3.right * _originalX, Time.deltaTime * 10f);
                _trolleySync.ChangingOwner = true;
                _trolleySync.photonView.TransferOwnership(0);
            }
        }
    }
}


