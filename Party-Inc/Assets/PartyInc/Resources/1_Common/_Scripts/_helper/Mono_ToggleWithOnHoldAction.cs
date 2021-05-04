using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace PartyInc
{
    public class Mono_ToggleWithOnHoldAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // COnsider Action List
        public Action OnHold;
        [SerializeField] private float _timeToActivate;

        private bool _runOnce = false;
        private bool _isPointerDown;
        private float _timeHeld;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
            _timeHeld = 0f;
            _runOnce = false;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_isPointerDown)
            {
                _timeHeld += Time.deltaTime;
            } 

            if(_timeHeld > _timeToActivate && !_runOnce)
            {
                OnHold();
                _runOnce = true;
            }
        }
    }
}