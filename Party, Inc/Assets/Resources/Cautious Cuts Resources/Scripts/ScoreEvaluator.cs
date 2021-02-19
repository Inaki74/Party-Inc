using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace CC
    {
        public class ScoreEvaluator
        {
            public static void EvaluateSlice(FallingLog theLog, float width, float height, float angle, out float posEv, out float angEv, out float finEv)
            {
                float logA = theLog.GetAngle();

                if (theLog.LogType == FallingLog.LogClass.Large ||
                    theLog.LogType == FallingLog.LogClass.Medium ||
                    theLog.LogType == FallingLog.LogClass.Small_Horizontal ||
                    theLog.LogType == FallingLog.LogClass.VerySmall_Horizontal)
                {
                    // HORIZONTAL
                    float logH = theLog.GetHeight();
                    float percentageH = EvaluateHeight(logH, height);
                    float percentageA = EvaluateAngleHorizontal(logA, angle);

                    if (percentageA < 0f)
                    {
                        percentageA = 0f;
                    }

                    if (percentageH < 0f)
                    {
                        percentageH = 0f;
                    }

                    float finalPercentage = percentageA * 20 / 100 + percentageH * 80 / 100; // Height amounts to 80% of the final score

                    // If the cut was vertical on a horizontal log, then you should get 0 points dude, thats terrible
                    if ((Mathf.Abs(angle) >= 70f && Mathf.Abs(angle) <= 90f) || (Mathf.Abs(angle) <= -70f && Mathf.Abs(angle) >= -90f))
                    {
                        finalPercentage = 0f;
                    }

                    posEv = percentageH;
                    angEv = percentageA;
                    finEv = finalPercentage;
                }
                else
                {
                    // VERTICAL
                    float logW = theLog.GetWidth();
                    float percentageW = EvaluateWidth(logW, width);
                    float percentageA = EvaluateAngleVertical(logA, angle);

                    if (percentageA < 0f)
                    {
                        percentageA = 0f;
                    }

                    if (percentageW < 0f)
                    {
                        percentageW = 0f;
                    }

                    float finalPercentage = percentageA * 80 / 100 + percentageW * 20 / 100; // Width amounts to 20% of the final score

                    posEv = percentageW;
                    angEv = percentageA;
                    finEv = finalPercentage;
                }
            }

            private static float EvaluateWidth(float logW, float width)
            {
                float dw = Mathf.Abs(logW - width);
                float distWToLeft = Mathf.Abs(logW - FallingLog.MinimumMarkWidth);
                float distWToRight = Mathf.Abs(logW - FallingLog.MaximumMarkWidth);

                if (distWToLeft >= distWToRight)
                {
                    return 100.00f - dw * 100 / distWToLeft;
                }
                else
                {
                    return 100.00f - dw * 100 / distWToRight;
                }
            }

            private static float EvaluateHeight(float logH, float height)
            {
                float dh = Mathf.Abs(logH - height);
                float distHToBottom = Mathf.Abs(logH - FallingLog.MinimumMarkHeight);
                float distHToTop = Mathf.Abs(logH - FallingLog.MaximumMarkHeight);

                if (distHToBottom >= distHToTop)
                {
                    return 100.00f - dh * 100 / distHToBottom;
                }
                else
                {
                    return 100.00f - dh * 100 / distHToTop;
                }
            }

            private static float EvaluateAngleHorizontal(float logA, float angle)
            {
                float percentageA = 0f;
                float rA = angle - logA;

                if (rA > 0f)
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MaximumMarkAngleHorizontal;
                }
                else
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MinimumMarkAngleHorizontal;
                }

                if (percentageA < 0)
                {
                    percentageA = 0f;
                }

                return percentageA;
            }

            private static float EvaluateAngleVertical(float logA, float angle)
            {
                // Change the negative angles to positive versions -70 -> 110, -90 -> 90
                // Slice angles have a range of -90 - 90 degrees
                if (logA < 0f)
                {
                    logA = logA + 180f;
                }

                if (angle < 0f)
                {
                    angle = angle + 180f;
                }

                float percentageA = 0f;
                float rA = angle - logA;

                if (rA > 0f)
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MaximumMarkAngleVertical;
                }
                else
                {
                    percentageA = 100.00f + rA * 100 / FallingLog.MinimumMarkAngleVertical;
                }

                if (percentageA < 0)
                {
                    percentageA = 0f;
                }

                return percentageA;
            }
        }
    }
}


