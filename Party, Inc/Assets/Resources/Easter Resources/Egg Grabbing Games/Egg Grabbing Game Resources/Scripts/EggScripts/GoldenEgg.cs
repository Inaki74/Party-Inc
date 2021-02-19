﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenEgg : EasterEgg
{
    protected override void Start()
    {
        base.Start();

        scoreModifier = 3;
        id = 2;
        fallingSpeed = 15.0f;
    }

}