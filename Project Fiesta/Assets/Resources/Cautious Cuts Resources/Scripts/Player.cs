﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FiestaTime
{
    namespace CC
    {

        public class Player : MonoBehaviour
        {
            [SerializeField] private TouchSlicer _touchSlicer;

            private List<RayhitSliceInfo> _logHits = new List<RayhitSliceInfo>();
            

            // Start is called before the first frame update
            void Start()
            {
                if(_touchSlicer == null)
                {
                    _touchSlicer = GetComponent<TouchSlicer>();
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (_touchSlicer.SliceThisRound)
                {
                    // We got our cut this round
                    _logHits = _touchSlicer.GetHits();
                    ProcessSlice();
                }
            }

            private void ProcessSlice()
            {
                RayhitSliceInfo start = _logHits.First();
                FallingLog theLog = start.objTransform.gameObject.GetComponent<FallingLog>();

                CalculateSliceScore(start, theLog);

                Plane cutter;
                GameObject[] slices = _touchSlicer.Slice(start.objTransform.gameObject, _logHits, false);

                CreatePosNegSlices(slices);

                _touchSlicer.ClearHits();
                _logHits.Clear();

                _touchSlicer.WaitForSliceTimeout();
            }

            private void CreatePosNegSlices(GameObject[] slices)
            {
                // Positive slice
                DeactivateAfterFalling d1 = slices[0].AddComponent<DeactivateAfterFalling>();
                d1.SetToDestroy(true);
                d1.SetDistanceToDeactivate(-45f);

                // Negative slice
                DeactivateAfterFalling d2 = slices[1].AddComponent<DeactivateAfterFalling>();
                d2.SetToDestroy(true);
                d2.SetDistanceToDeactivate(-45f);
            }

            private void CalculateSliceScore(RayhitSliceInfo start, FallingLog theLog)
            {
                int i = 0;
                Vector3 vAverage = Vector3.zero;
                float hAverage = 0f;

                Vector3 zero = start.objTransform.InverseTransformPoint(start.rayHit.point) + _logHits.First().objVelocity * Time.fixedDeltaTime;

                foreach (RayhitSliceInfo a in _logHits)
                {
                    i++;
                    Vector3 v = a.objTransform.InverseTransformPoint(a.rayHit.point) + a.objVelocity * Time.fixedDeltaTime;
                    Vector3 vx = v - zero;

                    vAverage += vx;
                    hAverage += v.y;
                }

                float finalHeight = hAverage / i;
                float finalAngle = Mathf.Atan(vAverage.normalized.y / vAverage.normalized.x) * Mathf.Rad2Deg;

                EvaluateSlice(theLog, finalHeight, finalAngle);
            }

            private void EvaluateSlice(FallingLog theLog, float height, float angle)
            {
                Debug.Log(angle);
                float logH = theLog.GetHeight();
                float logA = theLog.GetAngle();

                float percentageH = 0f;
                float rH = height - logH;
                float rHDiff = Mathf.Abs(height - logH);

                if(rH > 0)
                {
                    //Upper side
                    float distHToTop = Mathf.Abs(logH - FallingLog.MaximumMarkHeight);
                    percentageH = 100.00f - rHDiff * 100 / distHToTop;
                }
                else if (rH < 0)
                {
                    //Lower side
                    float distHToBottom = Mathf.Abs(logH - FallingLog.MinimumMarkHeight);
                    percentageH = 100.00f - rHDiff * 100 / distHToBottom;
                }
                else
                {
                    percentageH = 100.00f;
                }

                float percentageA = 0f;
                float rA = angle - logA;

                if(rA > 0f)
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MaximumMarkAngle;
                }
                else
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MinimumMarkAngle;
                }
                
                if(percentageA < 0)
                {
                    percentageA = 0f;
                }

                float finalPercentage = percentageA * 20 / 100 + percentageH * 80 / 100; // Height amounts to 80% of the final score

                Debug.Log("RESULTS FROM CUT: ");
                Debug.Log("HEIGHT PROXIMITY: " + percentageH.ToString("0.00") + "%");
                Debug.Log("ANGLE PROXIMITY: " + percentageA.ToString("0.00") + "%");
                Debug.Log("OVERALL PROXIMITY: " + finalPercentage.ToString("0.00") + "%");
            }
        }
    }
}


