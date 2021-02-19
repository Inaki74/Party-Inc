using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayInc
{
    public class Mono_DeactivateAfterTime : MonoBehaviour
    {
        [SerializeField] private float _timeToDie;
        private float _currentTime;
        [SerializeField] private bool _destroyInstead;

        // Update is called once per frame
        void Update()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime >= _timeToDie)
            {
                if (_destroyInstead)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void SetTimeToDeactivate(float value)
        {
            _timeToDie = value;
        }

        public void SetToDestroy(bool destroy)
        {
            _destroyInstead = destroy;
        }
    }
}


