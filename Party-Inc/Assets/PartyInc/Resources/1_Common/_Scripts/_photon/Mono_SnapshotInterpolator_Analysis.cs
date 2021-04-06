using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mono_SnapshotInterpolator_Analysis : MonoSingleton<Mono_SnapshotInterpolator_Analysis>
    {
        public float AcumulativeExtrapolatedTime { get; set; }
        public float AmountOfExtrapolations { get; set; }
        public float AcumulativeInterpolatedTime { get; set; }
        public float AmountOfInterpolations { get; set; }

        private void OnDestroy()
        {
            if (AmountOfExtrapolations + AmountOfInterpolations != 0)
            {
                Debug.Log("Interpolations: " + AmountOfInterpolations + " Extrapolations: " + AmountOfExtrapolations);
                Debug.Log("A percentage of: " + (AmountOfExtrapolations * 100 / (AmountOfExtrapolations + AmountOfInterpolations)) + " extrapolations.");
            }
        }
    }
}

