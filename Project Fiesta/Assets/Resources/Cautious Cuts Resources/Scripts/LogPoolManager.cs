using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace CC
    {
        public class LogPoolManager : MonoSingleton<LogPoolManager>
        {
            [SerializeField] private Transform _logHolder;
            public Transform LogHolder
            {
                get
                {
                    return _logHolder;
                }

                private set
                {
                    _logHolder = value;
                }
            }

            [SerializeField] private GameObject _logPrefab;
            // TO BE SEEN:
            // [SerializeField] private GameObject _smallLogPrefab;
            // [SerializeField] private GameObject _bigLogPrefab;
            // [SerializeField] private GameObject _steelLogPrefab;

            private List<GameObject> _logs = new List<GameObject>();

            // Start is called before the first frame update
            void Start()
            {
                GenerateLog();
                GenerateLog();
                GenerateLog();
                GenerateLog();
            }

            /// <summary>
            /// Creates a new log.
            /// </summary>
            /// <param name="subsection"></param>
            private void GenerateLog()
            {
                GameObject newLog = Instantiate(_logPrefab);
                newLog.transform.parent = _logHolder.transform;
                newLog.SetActive(false);
                _logs.Add(newLog);
            }

            /// <summary>
            /// Get requested log
            /// </summary>
            /// <param name="difficulty"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public GameObject RequestLog()
            {
                //Get log
                foreach (GameObject sub in _logs)
                {
                    if (!sub.activeInHierarchy)
                    {
                        //Activate and return
                        sub.SetActive(true);
                        return sub;
                    }

                }

                // If we get here, its because we haven't got the log
                GenerateLog();
                return RequestLog();
            }
        }
    }
}


