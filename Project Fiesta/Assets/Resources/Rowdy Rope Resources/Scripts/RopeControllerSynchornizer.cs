using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace RR
    {
        [RequireComponent(typeof(RopeControllerM))]
        public class RopeControllerSynchornizer : MonoBehaviourPun, IPunObservable
        {
            [SerializeField]
            private RopeControllerM _ropeController;

            private bool _infoReceived;

            private double _interpolationBackTime = 0.15; //0.15 = 150ms
            private double _extrapolationLimit = 0.5;

            private State[] _bufferedState = new State[20];
            private int _timestampCount;

            private struct State
            {
                public double timestamp;
                public float angle;
                public float rotationSpeed;
            }

            private void Start()
            {
                if(_ropeController == null)
                {
                    _ropeController = GetComponent<RopeControllerM>();
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                if (_infoReceived)
                {
                    double interpTime = PhotonNetwork.Time - _interpolationBackTime;

                    if(interpTime < _bufferedState[0].timestamp)
                    {
                        // INTERPOLATE
                        for(int i = 0; i < _timestampCount; i++)
                        {
                            if(_bufferedState[i].timestamp <= interpTime || i == _timestampCount - 1)
                            {
                                // The state one slot newer (<100ms) than the best playback state
                                State rhs = _bufferedState[Mathf.Max(i - 1, 0)];
                                // The best playback state (closest to 100 ms old (default time))
                                State lhs = _bufferedState[i];

                                // Use the time between the two slots to determine if interpolation is necessary
                                double length = rhs.timestamp - lhs.timestamp;
                                float t = 0.0f;
                                // As the time difference gets closer to 100 ms t gets closer to 1 in
                                // which case rhs is only used
                                if (length > 0.0001)
                                {
                                    t = (float)((interpTime - lhs.timestamp) / length);
                                }
                                // if t=0 => lhs is used directly
                                _ropeController.angle = Mathf.LerpAngle(lhs.angle * Mathf.Rad2Deg, rhs.angle * Mathf.Rad2Deg, t) * Mathf.Deg2Rad;
                                _ropeController.rotationSpeed = rhs.rotationSpeed;

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

                                return;
                            }
                        }
                    }
                    else
                    {
                        // EXTRAPOLATE
                        State newest = _bufferedState[0];

                        float extrapTime = (float)(interpTime - newest.timestamp);

                        if(extrapTime < _extrapolationLimit)
                        {
                            _ropeController.rotationSpeed = newest.rotationSpeed;
                            _ropeController.angle = newest.angle + extrapTime * _ropeController.rotationSpeed;

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
                    }
                }
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting && PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("Sending");
                    stream.SendNext(_ropeController.angle);
                    stream.SendNext(_ropeController.rotationSpeed);
                }
                else
                {
                    _infoReceived = true;

                    float netAngle = (float)stream.ReceiveNext();
                    float netRotSpeed = (float)stream.ReceiveNext();

                    // Shift state and delete state 20
                    for (int i = _bufferedState.Length - 1; i >= 1; i--)
                    {
                        _bufferedState[i] = _bufferedState[i - 1];
                    }

                    State stt;
                    stt.angle = netAngle;
                    stt.timestamp = info.SentServerTime;
                    stt.rotationSpeed = netRotSpeed;
                    _bufferedState[0] = stt;

                    _timestampCount = Mathf.Min(_timestampCount + 1, _bufferedState.Length);

                    for(int i = 0; i < _timestampCount - 1; i++)
                    {
                        if(_bufferedState[i].timestamp < _bufferedState[i + 1].timestamp)
                        {
                            // State inconsistent
                            Debug.Log("INCONSISTENT STATE!");
                        }
                    }
                }
            }
        }
    }
}


