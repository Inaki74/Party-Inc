using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenEgg : Mono_Egg_EGG
{
    protected override void Start()
    {
        base.Start();

        scoreModifier = 3;
        id = 2;
        fallingSpeed = 15.0f;
    }

}
