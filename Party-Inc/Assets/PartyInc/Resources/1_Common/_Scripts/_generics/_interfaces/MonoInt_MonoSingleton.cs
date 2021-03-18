using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    /// <summary>
    /// The monobehaviourPun singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviourPunCallbacks where T : MonoSingleton<T>
    {
        private static T _current;
        public static T Current
        {
            get
            {
                if (_current == null)
                {
                    Debug.Log("No " + typeof(T) + " Instantiated");
                }

                return _current;
            }
        }

        private void Awake()
        {
            T[] objs = FindObjectsOfType<T>();

            if(objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            Debug.Log(typeof(T) + " Instantiated");
            _current = (T)this;
            Init();
        }

        public virtual void Init()
        {

        }
    }
}

