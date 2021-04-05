using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_ForkController : MonoBehaviour
        {
            public void BtnReturningPlayer()
            {
                Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
                Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.LAUNCHER_SIGNIN);
            }

            public void BtnNewPlayer()
            {
                Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
                Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.LAUNCHER_SIGNUP);
            }
        }
    }
}


