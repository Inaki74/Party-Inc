using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace LL
    {
        public class Mng_GameManager_LL : FiestaGameManager<Mng_GameManager_LL, float>
        {
            private float _startingSpeed = 4f;
            private float _finalSpeed = 15f;
            private float _timeToReachFinalSpeedInSeconds = 80f;

            [SerializeField] private float _movementSpeed;
            public float MovementSpeed
            {
                get
                {
                    return _movementSpeed;
                }
                set
                {
                    _movementSpeed = value;
                }
            }

            [SerializeField] private float _gravity;

            private float _currentGate;
            public float CurrentGate
            {
                get
                {
                    return _currentGate;
                }
                private set
                {
                    _currentGate = value;
                }
            }

            protected override void InStart()
            {
                //throw new System.NotImplementedException();
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                //throw new System.NotImplementedException();
            }

            public override void Init()
            {
                base.Init();

                Physics.gravity = new Vector3(0f, _gravity, 0f);

                Mono_ObstaclePassCheck_LL.onPlayerPassed += OnPlayerPassedGate;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_LL.onPlayerPassed -= OnPlayerPassedGate;
            }

            private void Update()
            {
                if (PhotonNetwork.IsConnectedAndReady && _startCountdown && !GameBegan)
                {
                    if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 1f)
                    {
                        GameBegan = true;
                    }
                }

                if (GameBegan)
                {
                    InGameTime += Time.deltaTime;

                    if (MovementSpeed <= _finalSpeed)
                    {
                        MovementSpeed = _startingSpeed + InGameTime * ((_finalSpeed - _startingSpeed) / _timeToReachFinalSpeedInSeconds);
                    }
                }
            }

            private void OnPlayerPassedGate()
            {
                CurrentGate++;
            }
        }
    }
}


// F = m*a
// a = F/m ->
// a = 100 / 4 -> 25 m/s*s
// a = 100 / 6 -> 16.6 m/s*s
// a = 90 / 4 -> 22.5 m/s*s

