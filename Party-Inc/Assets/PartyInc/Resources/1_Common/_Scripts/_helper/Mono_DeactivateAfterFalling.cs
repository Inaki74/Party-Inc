﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mono_DeactivateAfterFalling : MonoBehaviour
    {
        [SerializeField] private float _distanceFallen;
        [SerializeField] private bool _destroyInstead;

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y < _distanceFallen)
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

        public void SetDistanceToDeactivate(float value)
        {
            _distanceFallen = value;
        }

        public void SetToDestroy(bool destroy)
        {
            _destroyInstead = destroy;
        }
    }
}

