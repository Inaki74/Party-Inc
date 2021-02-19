using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        [ExecuteInEditMode]
        public class SubSectionInfo : MonoBehaviour
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
                InvisibleTrolleyController.onPassedSection += OnPassedSection;

                var children = _ground.GetComponentsInChildren<Tile>();

                AmountOfTiles = children.Length;

                _subsectionCount = 3;

                _terribleWorkaround = true;
                _terribleWorkaroundTime = 2f;
            }

            private void OnDestroy()
            {
                InvisibleTrolleyController.onPassedSection -= OnPassedSection;
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
                var children = _ground.GetComponentsInChildren<Tile>();
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