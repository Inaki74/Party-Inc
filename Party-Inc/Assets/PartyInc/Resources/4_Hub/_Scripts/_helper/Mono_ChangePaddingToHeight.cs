using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_ChangePaddingToHeight : MonoBehaviour
        {
            [SerializeField] GridLayoutGroup _gridLayout;

            // Start is called before the first frame update
            void Start()
            {
                // This is the function I found (The line function between (1337, 350) and (2346, 125)
                _gridLayout.padding.bottom = Mathf.RoundToInt(-(Screen.height * 225 / 1102) + (11825 / 19));
            }
        }

    }
}

