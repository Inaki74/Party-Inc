using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mono_GameMetadata : MonoBehaviour
    {
        public bool WasLocalPlayerHighscore { get; set; }
        public float LocalPlayerHighscoreFloat { get; set; }
        public int LocalPlayerHighscoreInt { get; set; }
        public bool DescendingCondition { get; set; }
        public ScoreType ScoreType { get; set; }
        public string GameDisplayName { get; set; }
        public string GameDBName { get; set; }
        public float PlayerCount { get; set; }
        public int WinnerId { get; set; }
        public PlayerResults<int>[] PlayerResultsInt = new PlayerResults<int>[4];
        public PlayerResults<float>[] PlayerResultsFloat = new PlayerResults<float>[4];
    }
}


