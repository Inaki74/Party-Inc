using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mng_Reloader : MonoSingleton<Mng_Reloader>
        {
            public delegate void ReloadEvent();
            public static event ReloadEvent onReload;

            public void Reload()
            {
                onReload?.Invoke();
            }

            public override void Init()
            {
                base.Init();
            }
        }
    }
}


