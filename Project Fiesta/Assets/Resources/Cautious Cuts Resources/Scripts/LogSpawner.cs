using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace CC
    {
        public class LogSpawner : MonoBehaviour
        {
            [SerializeField] private float _spawnInterval;
            private float _currentTime = 0f;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                _currentTime += Time.deltaTime;

                if(_currentTime > _spawnInterval)
                {
                    _currentTime = 0f;
                    SpawnLog();
                }
            }

            private void SpawnLog()
            {
                GameObject log = LogPoolManager.Current.RequestLog();
                log.transform.position = transform.position;
            }
        }
    }
}


