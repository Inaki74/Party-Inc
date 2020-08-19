using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGGA_RottenEgg
{
    public class RottenEgg : EasterEgg
    {
        protected override void Start()
        {
            base.Start();

            scoreModifier = -1;
            id = 1;
        }

    }
}

