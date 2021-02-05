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
                    if (_logHits.Count <= 1) return;
                    ProcessSlice();
                }
            }

            private IEnumerator WaitForAllSlicesCo(LogController theLogCon)
            {
                yield return new WaitUntil(() => GameManager.Current.Sliced.Count == GameManager.Current.playerCount);

                theLogCon.SendNextWave();
            }

            private void ProcessSlice()
            {
                RayhitSliceInfo start = _logHits.First();
                RayhitSliceInfo last = _logHits.Last();
                FallingLog theLog = start.objTransform.gameObject.GetComponent<FallingLog>();
                LogController theLogCon = start.objTransform.gameObject.GetComponent<LogController>();

                // Calculates the score
                Vector3 slashPos = CalculateSliceScore(start, theLog);

                // Trigger event where next wave is spawned immediately after
                // We send it if we are the master client and if every players log has been sliced
                if (PhotonNetwork.IsMasterClient)
                {
                    GameManager.Current.SendSliced();
                    StopCoroutine(WaitForAllSlicesCo(theLogCon));
                    StartCoroutine(WaitForAllSlicesCo(theLogCon));
                }
                else
                {
                    GameManager.Current.SendSliced();
                }

                // Cuts the game object and creates the slices
                // Manipulating the slices here wont affect through the network unless done specifically
                GameObject[] slices = _touchSlicer.Slice(start.objTransform.gameObject, _logHits, false);

                // Add more gravity to the pieces
                ConstantForce c1 = slices[0].AddComponent<ConstantForce>();
                ConstantForce c2 = slices[1].AddComponent<ConstantForce>();
                c1.force = Physics.gravity * 2;
                c2.force = Physics.gravity * 2;

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

                float posEv;
                float angEv;
                float finEv;
                ScoreEvaluator.EvaluateSlice(theLog, finalWidth, finalHeight, finalAngle, out posEv, out angEv, out finEv);

                onLogSlicedScore.Invoke(posEv, angEv, finEv);

                return zero;
            }

        }
    }
}


