using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mng_SceneNavigationSystem : MonoSingleton<Mng_SceneNavigationSystem>
    {
        public Stack<int> SceneStack { get; private set; } = new Stack<int>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


