using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public static class Mono_Rescaler
        {
            public static Vector3 RescaleValueToAspectRatio(float originalAspectRatio, Vector3 toScale)
            {
                float currentAspect = (float)Screen.width / (float)Screen.height;

                return toScale * currentAspect / originalAspectRatio;
            }

            public static int RescaleValueToHeightRatio(int originalHeight, int toScale)
            {
                int currentHeight = Screen.height;

                return Mathf.RoundToInt(toScale * originalHeight / currentHeight);
            }
        }
    }
}


