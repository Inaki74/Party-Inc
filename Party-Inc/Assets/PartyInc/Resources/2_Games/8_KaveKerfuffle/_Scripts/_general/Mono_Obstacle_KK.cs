using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_Obstacle_KK : MonoBehaviour
        {
            [SerializeField] private Transform _upperPieceTransform;
            [SerializeField] private Transform _lowerPieceTransform;

            public void SetObstaclesOpening(float diff)
            {
                float uY;
                float lY;

                // This is terrible
                uY = 6.08f - diff;
                lY = -10.13f + diff;

                _upperPieceTransform.localPosition = new Vector3(0f, uY, 0f);
                _lowerPieceTransform.localPosition = new Vector3(0f, lY, 0f);
            }
        }
    }
}


