using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        [ExecuteInEditMode]
        public class Mono_ProceduralGenerator_KK : MonoBehaviour
        {
            private float[] _perlinVector = new float[256];
            private float[] _xPositions = new float[256];

            [SerializeField] private float _amplitude;
            [SerializeField] private float _frequency;

            private float _seed;

            // Start is called before the first frame update
            void Start()
            {
                _seed = Time.time;
            }

            // Update is called once per frame
            void Update()
            {
                //if (Input.GetKeyDown(KeyCode.R))
                //{
                //    Debug.Log("REDO");
                    
                //}

                SimulateGeneration();

                if (Input.GetKeyDown(KeyCode.S))
                {
                    _seed = Time.time;
                }
            }

            private void SimulateGeneration()
            {
                for (int i = 0; i < 256; i++)
                {
                    _perlinVector[i] = Mathf.PerlinNoise(i * _frequency + _seed, 0f) * _amplitude;
                    _xPositions[i] = i * _frequency;
                }

                for(int i = 0; i< 255; i++)
                {
                    Debug.DrawLine(new Vector3(_xPositions[i], _perlinVector[i], 0f), new Vector3(_xPositions[i + 1], _perlinVector[i + 1], 0f), Color.green, Time.deltaTime);
                }
            }
        }
    }
}