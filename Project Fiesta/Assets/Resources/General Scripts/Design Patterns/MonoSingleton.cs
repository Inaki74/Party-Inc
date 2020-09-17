using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
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
                    Debug.Log("No Game manager Instantiated");
                }

                return _current;
            }
        }

        private void Awake()
        {
            _current = (T)this;
            Init();
        }

        public virtual void Init()
        {

        }
    }
}

