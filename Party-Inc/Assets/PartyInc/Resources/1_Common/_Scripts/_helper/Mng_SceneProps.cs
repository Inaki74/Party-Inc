using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mng_SceneProps : MonoSingleton<Mng_SceneProps>
    {
        public Dictionary<string, object> SceneProps { get; set; } = new Dictionary<string, object>();

        public void SetProperty<T>(string key, object info)
        {
            if (SceneProps.ContainsKey(key))
            {
                SceneProps.Remove(key);
            }
            SceneProps.Add(key, (T)info);
        }
    }
}


