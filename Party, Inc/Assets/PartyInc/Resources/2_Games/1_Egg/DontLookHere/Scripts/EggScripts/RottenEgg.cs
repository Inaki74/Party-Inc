using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RottenEgg : Mono_Egg_EGG
{
    protected override void Start()
    {
        base.Start();

        scoreModifier = -1;
        id = 1;
    }

}
