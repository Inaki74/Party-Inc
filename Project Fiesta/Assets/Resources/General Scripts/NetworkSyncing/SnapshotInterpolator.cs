using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace FiestaTime
{
    /// <summary>
    /// A class that represents the interpolator/extrapolator through the network.
    /// This introduces the basic needs such as the buffering and other shenanigans you get to have on your game.
    /// I intend to do all optimization through here as well as its general to all Snapshot Interpolating.
    /// The type T are the values you want to sync across the network.
    /// You then implement each of the functions (Interpolate, Extrapolate, SendInfo and ReceiveInfo) according to your needs
    /// </summary>
    /// <typeparam name="T">Values you want to sync across the network.</typeparam>
    public abstract class SnapshotInterpolator<T> : MonoBehaviourPun, IPunObservable
    {
        private bool _infoReceived;

        private double _interpolationBackTime; //0.15 = 150ms
        private double _extrapolationLimit = 0.5;

        protected struct State
        {
            public double timestamp;
            public T info;
        }

        private State[] _bufferedState = new State[PhotonNetwork.SendRate * 2];
        private int _timestampCount;

        private void Awake()
        {
            _interpolationBackTime = (1d / PhotonNetwork.SendRate) + 0.015;
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

                if (_bufferedState[0].timestamp > interpTime)
                {
                    // INTERPOLATE
                    for (int i = 0; i < _timestampCount; i++)
                    {
                        if (_bufferedState[i].timestamp <= interpTime || i == _timestampCount - 1)
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
                            Interpolate(rhs, lhs, t);

                            return;
                        }
                    }
                }
                else
                {
                    // EXTRAPOLATE
                    State newest = _bufferedState[0];

                    float extrapTime = (float)(interpTime - newest.timestamp);

                    if (extrapTime < _extrapolationLimit)
                    {
                        Extrapolate(newest, extrapTime);
                    }
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                SendInformation(stream, info);
            }
            else if (stream.IsReading)
            {
                _infoReceived = true;

                T rInfo = ReceiveInformation(stream, info);

                // Shift state and delete state 20
                for (int i = _bufferedState.Length - 1; i >= 1; i--)
                {
                    _bufferedState[i] = _bufferedState[i - 1];
                }

                State stt;
                stt.timestamp = info.SentServerTime;
                stt.info = rInfo;
                _bufferedState[0] = stt;

                _timestampCount = Mathf.Min(_timestampCount + 1, _bufferedState.Length);

                for (int i = 0; i < _timestampCount - 1; i++)
                {
                    if (_bufferedState[i].timestamp < _bufferedState[i + 1].timestamp)
                    {
                        // State inconsistent
                        Debug.Log("INCONSISTENT STATE!");
                    }
                }
            }
        }

        protected abstract void Interpolate(State rhs, State lhs, float t);

        protected abstract void Extrapolate(State newest, float extrapTime);

        protected abstract void SendInformation(PhotonStream stream, PhotonMessageInfo info);

        protected abstract T ReceiveInformation(PhotonStream stream, PhotonMessageInfo info);
    }
}


