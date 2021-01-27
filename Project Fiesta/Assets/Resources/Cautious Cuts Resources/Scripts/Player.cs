using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FiestaTime
{
    namespace CC
    {

        public struct RayhitLogInfo
        {
            public Vector3 logVelocity;
            public RaycastHit rayHit;
            public Transform logTransform;
            public Vector3 rayHitCameraPoint;
        }

        /// <summary>
        /// This is the 'cutting' of the cutting system.
        /// </summary>
        public class Player : MonoBehaviour
        {
            [SerializeField] private LineRenderer _lr;

            [SerializeField] private Color _c1 = Color.yellow;
            [SerializeField] private Color _c2 = Color.red;

            [SerializeField] private LayerMask _whatIsLog;
            [SerializeField] private LayerMask _whatIsCameraPlane;

            [SerializeField] private float _forceOnCut;

            private bool _cutThisRound;

            private List<RayhitLogInfo> _logHits = new List<RayhitLogInfo>();
            

            private int _vertexCount;
            private int _maxVertexCount = 7;

            private bool _runOnce = false;

            // Start is called before the first frame update
            void Start()
            {
                if(_lr == null)
                {
                    _lr = GetComponent<LineRenderer>();
                }

                _lr.startColor = _c1;
                _lr.endColor = _c2;
                _lr.startWidth = 0f;
                _lr.endWidth = 0.3f;
                _lr.positionCount = 0;

                _cutThisRound = true;
            }

            // Update is called once per frame
            void Update()
            {
                if (Application.isMobilePlatform)
                {
                    TakeInputMobile();
                }
                else
                {

                }

                if (!_cutThisRound && _logHits.Count != 0)
                {
                    // We got our cut this round
                    ProcessCut();
                }
            }

            private void TakeInputMobile()
            {
                if(Input.touchCount > 0)
                {
                    Touch t = Input.touches[0];

                    RenderLine(t);

                    if (_cutThisRound)
                    {
                        GetCutsThisRound();
                    }

                    if (t.phase == TouchPhase.Ended)
                    {
                        ResetLine();
                    }
                }
            }

            private void ProcessCut()
            {
                int i = 0;
                Vector3 vAverage = Vector3.zero;
                float hAverage = 0f;

                RayhitLogInfo start = _logHits.First();
                Vector3 zero = start.logTransform.InverseTransformPoint(start.rayHit.point) + _logHits.First().logVelocity * Time.fixedDeltaTime;

                // Debugging ////////////////////////////////////////////////////////
                FallingLog theLog = start.logTransform.gameObject.GetComponent<FallingLog>();
                theLog.CreateEmpty();
                Debug.Log("START: " + zero.x + ", " + zero.y + ", " + zero.z);
                //////////////////////////////////////////////////////////////////
                foreach (RayhitLogInfo a in _logHits)
                {
                    i++;
                    Vector3 v = a.logTransform.InverseTransformPoint(a.rayHit.point) + a.logVelocity * Time.fixedDeltaTime;
                    Vector3 vx = v - zero;

                    vAverage += vx;
                    hAverage += v.y;

                    // Debugging  //////////////////////////////////////////////////////////////////
                    Debug.Log("HIT POSITION " + i + ": " + vx.x + ", " + vx.y + ", " + vx.z);
                    theLog.SetHitpoint(a.rayHit.point + a.logVelocity * Time.fixedDeltaTime - a.logTransform.position);
                    ////////////////////////////////////////////////////////////////////
                }
                float finalHeight = hAverage / i;
                float finalAngle = Mathf.Atan(vAverage.normalized.y / vAverage.normalized.x) * Mathf.Rad2Deg;

                CutLog(start.logTransform.gameObject);

                // Debugging //////////////////////////////////////////////////////////////////
                Debug.Log("USING A MODEL ON AVERAGES: ");
                Debug.Log("VECTOR AVERAGES: (" + vAverage.x + ", " + vAverage.y + ", " + vAverage.z + ")");
                Debug.Log("HEIGHT: " + finalHeight + " ANGLE: " + finalAngle);
                ////////////////////////////////////////////////////////////////////

                _logHits.Clear();

                StartCoroutine(CheckForTimeoutCo(0.5f));
            }

            private void CutLog(GameObject log)
            {
                // We get the first hitPoint and the last hitPoint
                float minx = _logHits.Min(info => info.rayHit.point.x);
                float maxx = _logHits.Max(info => info.rayHit.point.x);
                RayhitLogInfo minXInfo = _logHits.First(info => info.rayHit.point.x == minx);
                RayhitLogInfo maxXInfo = _logHits.First(info => info.rayHit.point.x == maxx);

                Vector3 sHPoint = minXInfo.rayHit.point;
                Vector3 fHPoint = maxXInfo.rayHit.point;

                // The start point is the average of the camera points
                // The camera points are the points straight from the hit points to the screen plane
                Vector3 start = (minXInfo.rayHitCameraPoint + maxXInfo.rayHitCameraPoint) /2;

                Vector3 norm;
                Plane cutter = DefinePlane(
                    log.transform,
                    minXInfo.rayHit.point,
                    maxXInfo.rayHit.point,
                    start,
                    out norm
                );

                GameObject[] slices = Tvtig.Slicer.Slicer.Slice(cutter, log);
                Destroy(log.gameObject);

                Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
                Vector3 newNormal = norm + Vector3.up * _forceOnCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }

            private Plane DefinePlane(Transform logT, Vector3 startHp, Vector3 finishHp, Vector3 start, out Vector3 norm)
            {
                Plane ret = new Plane();

                // We draw a triangle from the camera and start and finish hitPoints
                Vector3 sideOne = start - startHp;
                Vector3 sideTwo = start - finishHp;

                Debug.DrawLine(start, startHp, Color.black, 100f);
                Debug.DrawLine(start, finishHp, Color.black, 100f);

                Vector3 normal = Vector3.Cross(sideOne, sideTwo).normalized;

                Vector3 tNormal = ((Vector3)(logT.localToWorldMatrix.transpose * normal)).normalized;

                Vector3 tStart = logT.InverseTransformPoint(start);

                GeneralHelperFunctions.DrawPlane(tStart, tNormal);

                ret.SetNormalAndPosition(tNormal, tStart);

                var direction = Vector3.Dot(Vector3.up, tNormal);

                //Flip the plane so that we always know which side the positive mesh is on
                if (direction < 0)
                {
                    ret = ret.flipped;
                }

                norm = tNormal;

                return ret;
            }

            private void GetCutsThisRound()
            {
                int posCount = _lr.positionCount;

                if (posCount == 1) return;


                Vector3[] lrPositions = new Vector3[posCount];
                _lr.GetPositions(lrPositions);

                for (int i = 1; i < posCount; i++)
                {
                    //bool found = false;
                    Vector3 pos = lrPositions[i];
                    Vector3 lastPos = lrPositions[i - 1];

                    // If its the last point
                    // Make the reverse process (to grab the end of the log)
                    if(i == posCount - 1)
                    {
                        pos = lrPositions[i - 1];
                        lastPos = lrPositions[i];
                    }

                    Vector3 newPos = lastPos;

                    int f = 0;
                    bool isLoop = false;
                    while (newPos != pos && !isLoop)
                    {
                        RaycastHit hit;
                        if (CheckForLog(newPos, out hit))
                        {
                            MakeRayhitLogInfo(hit, newPos);
                        }
                        else if (_logHits.Count != 0)
                        {
                            _cutThisRound = false;
                        }

                        newPos = Vector3.MoveTowards(newPos, pos, Time.deltaTime);

                        isLoop = GeneralHelperFunctions.CheckForLoop(f);
                    }
                }
            }

            private IEnumerator CheckForTimeoutCo(float time)
            {
                _cutThisRound = false;

                yield return new WaitForSeconds(time); // THis wait will be the spawning of the next log

                _cutThisRound = true;

                //yield return new WaitForSeconds(time);

                
            }

            private bool CheckForLog(Vector3 startPosition, out RaycastHit hit)
            {
                //Debug.DrawLine(startPosition, Camera.main.transform.TransformPoint(Vector3.back), Color.red, 5f);

                if (Physics.Raycast(startPosition, Camera.main.transform.TransformDirection(Vector3.back), out hit, 100f, _whatIsLog))
                {
                    //Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.back) * hit.distance, Color.red, 0f);
                    return true;
                }
                else
                {
                    //Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.back) * 100f, Color.green, 0f);
                    return false;
                }
            }

            private bool CheckForPlane(Vector3 startPosition, out RaycastHit hit)
            {
                
                if (Physics.Raycast(startPosition, Camera.main.transform.TransformDirection(Vector3.back), out hit, 100f, _whatIsCameraPlane))
                {
                    Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.back) * hit.distance, Color.blue, 0f);
                    return true;
                }
                else
                {
                    Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.back) * 100f, Color.green, 0f);
                    return false;
                }
            }

            private void MakeRayhitLogInfo(RaycastHit hit, Vector3 origin)
            {
                RayhitLogInfo info;

                RaycastHit hat;
                if(CheckForPlane(origin, out hat))
                {
                    info.rayHitCameraPoint = hat.point;
                }
                else
                {
                    info.rayHitCameraPoint = Vector3.zero;
                }

                info.logVelocity = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
                info.rayHit = hit;
                info.logTransform = hit.transform;

                _logHits.Add(info);
            }

            private void RenderLine(Touch t)
            {
                // Create that sort of 'limited' rendering.
                if (_lr.positionCount == _maxVertexCount)
                {
                    // Moves all the front points to the back
                    for (int i = 1; i <= _lr.positionCount - 1; i++)
                    {
                        Vector3 position = _lr.GetPosition(i);
                        _lr.SetPosition(i - 1, position);
                    }

                    // Add the position to the end of the vector.
                    if (t.phase == TouchPhase.Moved)
                    {
                        Vector3 tPos = new Vector3(t.position.x, t.position.y, 5);
                        _lr.SetPosition(_maxVertexCount - 1, Camera.main.ScreenToWorldPoint(tPos));
                    }
                }
                else
                {
                    if (t.phase == TouchPhase.Moved)
                    {
                        _lr.positionCount = _vertexCount + 1;
                        Vector3 tPos = new Vector3(t.position.x, t.position.y, 5);
                        _lr.SetPosition(_vertexCount, Camera.main.ScreenToWorldPoint(tPos));
                        _vertexCount++;
                    }
                }
            }

            private void ResetLine()
            {
                _lr.positionCount = 0;
                _vertexCount = 0;
            }
        }
    }
}


