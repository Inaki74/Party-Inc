using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace FiestaTime
{
    namespace CC
    {

        public class Player : MonoBehaviourPun
        {
            public delegate void ActionSliceLog(float p, float a, float t);
            public static event ActionSliceLog onLogSlicedScore;

            [SerializeField] private MeshRenderer _mr;
            [SerializeField] private Material _mineMaterial;
            [SerializeField] private GameObject _slashingParticles;

            [SerializeField] private TouchSlicer _touchSlicer;

            private List<RayhitSliceInfo> _logHits = new List<RayhitSliceInfo>();


            // Start is called before the first frame update
            void Start()
            {
                if (_touchSlicer == null)
                {
                    _touchSlicer = GetComponent<TouchSlicer>();
                }

                if (_mr == null)
                {
                    _mr = GetComponent<MeshRenderer>();
                }

                if (photonView.IsMine || !PhotonNetwork.IsConnected)
                {
                    _mr.material = _mineMaterial;
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
                RayhitSliceInfo last = _logHits.Last();
                FallingLog theLog = start.objTransform.gameObject.GetComponent<FallingLog>();

                // Calculates the score
                Vector3 slashPos = CalculateSliceScore(start, theLog);

                // Cuts the game object and creates the slices
                GameObject[] slices = _touchSlicer.Slice(start.objTransform.gameObject, _logHits, false);

                SpawnSlashingParticles(theLog, start.rayHit.point, last.rayHit.point, slashPos);

                // Cleanup
                _touchSlicer.ClearHits();
                _logHits.Clear();
                _touchSlicer.WaitForSliceTimeout();
            }

            private void SpawnSlashingParticles(FallingLog theLog, Vector3 start, Vector3 end, Vector3 slashPos)
            {
                Vector3 direction = end - start;

                float angle = Vector3.SignedAngle(new Vector3(-1f, 0f, direction.z), direction, Vector3.back);

                if (angle < 0f)
                {
                    PhotonNetwork.Instantiate(_slashingParticles.name, theLog.transform.localToWorldMatrix.MultiplyPoint(slashPos), Quaternion.Euler(angle + 90f, 90f, -90f));
                }
                else
                {
                    PhotonNetwork.Instantiate(_slashingParticles.name, theLog.transform.localToWorldMatrix.MultiplyPoint(slashPos), Quaternion.Euler(angle - 90f, 90f, -90f));
                }
            }

            private Vector3 CalculateSliceScore(RayhitSliceInfo start, FallingLog theLog)
            {
                int i = 0;
                Vector3 vAverage = Vector3.zero;
                float hAverage = 0f;
                float wAverage = 0f;

                Vector3 zero = start.objTransform.InverseTransformPoint(start.rayHit.point);

                // Get averages of all hits
                foreach (RayhitSliceInfo a in _logHits)
                {
                    i++;
                    Vector3 v = a.objTransform.InverseTransformPoint(a.rayHit.point);
                    Vector3 vx = v - zero;

                    vAverage += vx;
                    hAverage += v.y;
                    wAverage += v.x;
                }

                float finalHeight = hAverage / i;
                float finalWidth = wAverage / i;
                float finalAngle = Mathf.Atan(vAverage.normalized.y / vAverage.normalized.x) * Mathf.Rad2Deg;

                EvaluateSlice(theLog, finalWidth, finalHeight, finalAngle);

                return zero;
            }

            private void EvaluateSlice(FallingLog theLog, float width, float height, float angle)
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
                    float percentageA = EvaluateAngle(logA, angle);

                    float finalPercentage = percentageA * 20 / 100 + percentageH * 80 / 100; // Height amounts to 80% of the final score
                    onLogSlicedScore.Invoke(percentageH, percentageA, finalPercentage);
                }
                else
                {
                    // VERTICAL
                    angle = Mathf.Abs(angle);
                    float logW = theLog.GetWidth();
                    float percentageW = EvaluateWidth(logW, width);
                    float percentageA = EvaluateAngle(logA, angle);

                    float finalPercentage = percentageA * 80 / 100 + percentageW * 20 / 100; // Width amounts to 20% of the final score
                    onLogSlicedScore.Invoke(percentageW, percentageA, finalPercentage);
                }
            }

            private float EvaluateWidth(float logW, float width)
            {
                float dw = Mathf.Abs(logW - width);
                float distWToLeft = Mathf.Abs(logW - FallingLog.MinimumMarkWidth);
                float distWToRight = Mathf.Abs(logW - FallingLog.MaximumMarkWidth);

                if(distWToLeft >= distWToRight)
                {
                    return 100.00f - dw * 100 / distWToLeft;
                }
                else
                {
                    return 100.00f - dw * 100 / distWToRight;
                }
            }

            private float EvaluateHeight(float logH, float height)
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

            private float EvaluateAngle(float logA, float angle)
            {
                float percentageA = 0f;
                float rA = angle - logA;

                if (rA > 0f)
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MaximumMarkAngle;
                }
                else
                {
                    percentageA = 100.00f - rA * 100 / FallingLog.MinimumMarkAngle;
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


