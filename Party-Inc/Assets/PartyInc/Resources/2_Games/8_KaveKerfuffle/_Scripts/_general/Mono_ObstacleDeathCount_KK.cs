using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PartyInc
{
    namespace KK
    {
        public class Mono_ObstacleDeathCount_KK : MonoBehaviour
        {
            public delegate void ActionObstacleDeath();
            public static event ActionObstacleDeath onObstacleDied;

            private int _currentDeathCount;
            private bool _enabledThisFrame;

            [SerializeField] private int _deathCount;
            public int CurrentDeathCount
            {
                get
                {
                    return _currentDeathCount;
                }
                set
                {
                    _currentDeathCount = value;
                }
            }

            public bool IsBase { get; set; } = true;

            private void Start()
            {

            }

            private void Awake()
            {
                Mono_ObstaclePassCheck_KK.onGateRendered += GateRendered;
            }

            private void Update()
            {
                if (_enabledThisFrame)
                {
                    _enabledThisFrame = false;
                }
            }

            private void OnEnable()
            {
                _currentDeathCount = 6;
                _enabledThisFrame = true;
            }

            private void OnDisable()
            {
                //_currentDeathCount = 6;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_KK.onGateRendered -= GateRendered;
            }

            private void GateRendered()
            {
                if (!gameObject.activeInHierarchy || _enabledThisFrame) return;

                _currentDeathCount--;

                if (_currentDeathCount <= 0f)
                {
                    onObstacleDied.Invoke();
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
