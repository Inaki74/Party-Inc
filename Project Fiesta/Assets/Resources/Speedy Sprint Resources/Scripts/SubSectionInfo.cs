using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        public class SubSectionInfo : MonoBehaviour
        {
            [Header("Place amount of tiles, no more no less")]
            [SerializeField] private int _amountOfTiles;
            public int AmountOfTiles
            {
                get
                {
                    return _amountOfTiles;
                }
                private set
                {
                    _amountOfTiles = value;
                }
            }

            private bool _terribleWorkaround;
            private float _terribleWorkaroundTime;

            private int _subsectionCount = 2;

            private void Update()
            {
                if(_terribleWorkaroundTime < 0)
                {
                    _terribleWorkaround = false;
                }
                else
                {
                    _terribleWorkaroundTime -= Time.deltaTime;
                }
            }

            private void Awake()
            {
                InvisibleTrolleyController.onPassedSection += OnPassedSection;
            }

            private void OnDestroy()
            {
                InvisibleTrolleyController.onPassedSection -= OnPassedSection;
            }

            private void OnEnable()
            {
                _subsectionCount = 2;

                _terribleWorkaround = true;
                _terribleWorkaroundTime = 2f;
            }

            private void OnDisable()
            {
                _subsectionCount = 2;
            }

            private void OnPassedSection()
            {
                if (_terribleWorkaround) return;

                _subsectionCount--;

                if (_subsectionCount == 0)
                {
                    transform.parent = SubsecPoolManager.Current.SubsectionHolder.transform;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}