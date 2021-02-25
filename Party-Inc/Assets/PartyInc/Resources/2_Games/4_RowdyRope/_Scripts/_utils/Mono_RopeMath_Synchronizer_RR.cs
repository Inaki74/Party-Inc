using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace RR
    {
        public struct AngleSync
        {
            public float angle;
            public float rotationSpeed;
        }

        [RequireComponent(typeof(Mono_RopeMath_Controller_RR))]
        public class Mono_RopeMath_Synchornizer_RR : MonoInt_SnapshotInterpolator<AngleSync>
        {
            [SerializeField]
            private Mono_RopeMath_Controller_RR _ropeController;

            private void Start()
            {
                if(_ropeController == null)
                {
                    _ropeController = GetComponent<Mono_RopeMath_Controller_RR>();
                }
            }

            protected override void Interpolate(State rhs, State lhs, float t)
            {
                AngleSync rhsInfo = rhs.info;
                AngleSync lhsInfo = lhs.info;

                _ropeController.angle = Mathf.LerpAngle(lhsInfo.angle * Mathf.Rad2Deg, rhsInfo.angle * Mathf.Rad2Deg, t) * Mathf.Deg2Rad;
                _ropeController.rotationSpeed = rhsInfo.rotationSpeed;

                if (_ropeController.angle > Mathf.PI * 2)
                {
                    float angleDiff = _ropeController.angle - Mathf.PI * 2;
                    _ropeController.angle = angleDiff;
                }

                if (_ropeController.angle < 0f)
                {
                    float angleDiff = Mathf.Abs(_ropeController.angle);
                    _ropeController.angle = Mathf.PI * 2 - angleDiff;
                }
            }

            protected override void Extrapolate(State newest, float extrapTime)
            {
                AngleSync newestInfo = newest.info;

                _ropeController.rotationSpeed = newestInfo.rotationSpeed;
                _ropeController.angle = newestInfo.angle + extrapTime * _ropeController.rotationSpeed;

                if (_ropeController.angle > Mathf.PI * 2)
                {
                    float angleDiff = _ropeController.angle - Mathf.PI * 2;
                    _ropeController.angle = angleDiff;
                }

                if (_ropeController.angle < 0f)
                {
                    float angleDiff = Mathf.Abs(_ropeController.angle);
                    _ropeController.angle = Mathf.PI * 2 - angleDiff;
                }
            }

            protected override void SendInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                stream.SendNext(_ropeController.angle);
                stream.SendNext(_ropeController.rotationSpeed);
            }

            protected override AngleSync ReceiveInformation(PhotonStream stream, PhotonMessageInfo info)
            {
                float netAngle = (float)stream.ReceiveNext();
                float netRotSpeed = (float)stream.ReceiveNext();

                AngleSync ret;
                ret.angle = netAngle;
                ret.rotationSpeed = netRotSpeed;

                return ret;
            }
        }
    }
}


