using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mono_DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}


