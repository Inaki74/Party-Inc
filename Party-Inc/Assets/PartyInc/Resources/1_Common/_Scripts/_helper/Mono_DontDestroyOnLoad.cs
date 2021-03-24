using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mono_DontDestroyOnLoad : MonoBehaviour
    {
        private void Start()
        {
            
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            //GameObject[] allGameobjects = FindObjectsOfType<GameObject>();

            //int found = 0;
            //foreach (GameObject go in allGameobjects)
            //{
            //    if (go.name == gameObject.name)
            //    {
            //        found++;
            //    }
            //}

            //if (found > 1)
            //{
            //    Debug.Log("Found copy of unique object" + gameObject.name);
            //    Destroy(gameObject);
            //}
        }
    }
}


