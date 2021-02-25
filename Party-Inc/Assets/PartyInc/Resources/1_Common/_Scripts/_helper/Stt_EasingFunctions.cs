using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public static class Stt_EasingFunctions
    {
        public static float SmoothInN(float t, int n)
        {
            float r = 1;

            for(int i = 0; i < n; i++)
            {
                r *= t;
            }

            return r;
        }

        public static float SmoothOutN(float t, int n)
        {
            float r = 1;
            float m = t - 1;

            for (int i = 0; i < n; i++)
            {
                r *= m;
            }

            return r;
        }
    }
}

