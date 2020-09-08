using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const string NAME_KEY_NETWRK = "PlayerName";
    public const string CHRCTR_KEY_NETWRK = "ChosenCharacter";
    public const string PLAYERPFB_ID_NETWRK = "pfb_PlayerHolder";
    public const string SPAWNERMANAGERPFB_ID_NETWRK = "pfb_EggSpawningManager";

    public const string BUNNY_NAME_CHRCTR = "EasterBunny";
    public const string SANTA_NAME_CHRCTR = "SantaClaus";

    public static Vector3 ONEPLAYER_MID_LANE = new Vector3(0f, 0f, 0f);
    public static Vector3 TWOPLAYER_MID_LANE_PLYRONE = new Vector3(-4f, 0f, 0f);
    public static Vector3 TWOPLAYER_MID_LANE_PLYRTWO = new Vector3(4f, 0f, 0f);
    public const string INT_MOVDIR_ANIM = "MoveDirection";
    public const string BOOL_STUNNED_ANIM = "isStunned";
    public const string BOOL_BROKENEGG_ANIM = "Broken";

    public const int XPOS_PLYRONESCORE_UI = 250;
    public const int XPOS_PLYRTWOSCORE_UI = -250;
}
