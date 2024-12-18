﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace SS
    {
        [ExecuteInEditMode]
        public class Mono_SubSection_SS : MonoBehaviour
        {
            public int AmountOfTiles { get; private set; }

            [SerializeField] private GameObject _ground;

            private bool _terribleWorkaround;
            private float _terribleWorkaroundTime;

            private int _subsectionCount = 3;

            private void Start()
            {
                PositionTiles();
            }

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
                Mono_InvisibleTrolley_SS.onPassedSection += OnPassedSection;

                var children = _ground.GetComponentsInChildren<Mono_Tile_SS>();

                AmountOfTiles = children.Length;

                _subsectionCount = 3;

                _terribleWorkaround = true;
                _terribleWorkaroundTime = 2f;
            }

            private void OnDestroy()
            {
                Mono_InvisibleTrolley_SS.onPassedSection -= OnPassedSection;
            }

            private void OnPassedSection()
            {
                if (_terribleWorkaround) return;

                _subsectionCount--;

                if (_subsectionCount == 0)
                {
                    Destroy(gameObject);
                }
            }

            private void PositionTiles()
            {
                var children = _ground.GetComponentsInChildren<Mono_Tile_SS>();
                float z = 0;

                foreach (var Tile in children)
                {
                    Tile.transform.position = transform.position + new Vector3(0f, 0.1f, z);
                    z += 10f;
                }
            }
        }
    }
}