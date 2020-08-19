using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGGA_GoldenEgg
{
    public class GoldenEgg : EGGA_EasterEgg.EasterEgg
    {
        protected override void Start()
        {
            base.Start();

            scoreModifier = 3;
            id = 2;
            fallingSpeed = 15.0f;
        }

    }
}

