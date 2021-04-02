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
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNIN);
            }

            public void BtnNewPlayer()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNUP);
            }
        }
    }
}


