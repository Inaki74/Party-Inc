using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace PartyInc
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

        private double _interpolationBackTime = (3d / PhotonNetwork.SendRate) + 0.05d; //0.15 = 150ms
        private double _extrapolationLimit = 0.5d;

        protected struct State
        {
            public double timestamp;
            public T info;
        }

        private State[] _bufferedState = new State[PhotonNetwork.SendRate * 2];
        private int _timestampCount;

        private int _debugEx;
        private int _debugIn;

        private void OnDestroy()
        {

            if (photonView.IsMine)
            {
                return;
            }
            Debug.Log("Interpolations: " + _debugIn + " Extrapolations: " + _debugEx);
            Debug.Log("A percentage of: " + (_debugEx * 100 / (_debugEx + _debugIn)) + " extrapolations.");
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                return;
            }

            if (_infoReceived)
            {
                double interpTime = PhotonNetwork.Time - _interpolationBackTime;

                //Debug.Log("ASDASDASD -1 " + _interpolationBackTime);
                //Debug.Log("ASDASDASD 0 " + PhotonNetwork.Time);
                //Debug.Log("ASDASDASD 1 " + _bufferedState[0].timestamp);
                //Debug.Log("ASDASDASD 2 " + interpTime);

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
                            _debugIn++;

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
                        _debugEx++;
                    }
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                SendInformation(stream, info);
            }
            else 
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

                //Debug.Log("New 0: " + stt.timestamp);
                //Debug.Log("LAG: " + (PhotonNetwork.Time - stt.timestamp));

                _timestampCount = Mathf.Min(_timestampCount + 1, _bufferedState.Length);

                for (int i = 0; i < _timestampCount - 1; i++)
                {
                    if (_bufferedState[i].timestamp < _bufferedState[i + 1].timestamp)
                    {
                        // State inconsistent
                        Debug.Log("INCONSISTENT STATE!");
                        var ordered = _bufferedState.OrderByDescending(state => state.timestamp);
                        _bufferedState = ordered.ToArray();
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


