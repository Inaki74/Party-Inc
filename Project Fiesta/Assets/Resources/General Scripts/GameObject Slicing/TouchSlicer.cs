using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace FiestaTime
{
    public struct RayhitSliceInfo
    {
        public Vector3 objVelocity;
        public RaycastHit rayHit;
        public Transform objTransform;
        public Vector3 rayHitCameraPoint;
    }

    /// <summary>
    /// Now this! This is mine.
    /// An input MonoBehaviour which allows us to slice gameobjects and give us the hitpoints of those slices.
    /// </summary>
    ///
    [RequireComponent(typeof(LineRenderer))]
    public class TouchSlicer : MonoBehaviourPun
    {
        [SerializeField] private LineRenderer _lr;

        // Colors of the Line Renderer
        [SerializeField] private Color _c1 = Color.yellow;
        [SerializeField] private Color _c2 = Color.red;

        // The force in which object halves jump when sliced
        [SerializeField] private float _forceOnCut;

        // The layer mask of things we are slicing
        [SerializeField] private LayerMask _whatWeAreCuttingLayerMask;
        // The slices are much more precise using a plane in front of our camera.
        [SerializeField] private LayerMask _whatIsCameraPlaneLayerMask;

        // Time we have to wait to get next slice.
        [SerializeField] private float _timeForNextCut;

        private int _vertexCount;
        private int _maxVertexCount = 10;

        // If this is true, then you got a slice this round of slicing.
        public bool SliceThisRound { get;  private set; }
        // If this is true, then you can slice.
        public bool CanSlice { get; private set; }
        private List<RayhitSliceInfo> _hits = new List<RayhitSliceInfo>();

        public List<Vector3> watch = new List<Vector3>();

        public GameObject mark1;
        public GameObject mark2;
        public GameObject mark3;

        // Start is called before the first frame update
        void Start()
        {
            if (_lr == null)
            {
                _lr = GetComponent<LineRenderer>();
            }

            _lr.startColor = _c1;
            _lr.endColor = _c2;
            _lr.startWidth = 0f;
            _lr.endWidth = 0.3f;
            _lr.positionCount = 0;

            CanSlice = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

            if (Application.isMobilePlatform)
            {
                SliceInputMobile();
            }
            else
            {

            }
        }

        private void SliceInputMobile()
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.touches[0];

                RenderLine(t);

                if (CanSlice)
                {
                    GetSliceThisRound();
                }

                if (t.phase == TouchPhase.Ended)
                {
                    ResetLine();
                }
            }
        }

        private Plane DefinePlane(Transform logT, Vector3 startHp, Vector3 finishHp, Vector3 start, out Vector3 norm)
        {
            Plane ret = new Plane();

            // We draw a triangle from the camera and start and finish hitPoints
            Vector3 sideOne = start - startHp;
            Vector3 sideTwo = start - finishHp;

            Vector3 normal = Vector3.Cross(sideOne, sideTwo).normalized;

            Vector3 tNormal = ((Vector3)(logT.localToWorldMatrix.transpose * normal)).normalized;

            Vector3 tStart = logT.InverseTransformPoint(startHp);

            ret.SetNormalAndPosition(tNormal, tStart);

            var direction = Vector3.Dot(Vector3.up, tNormal);

            //Flip the plane so that we always know which side the positive mesh is on
            if (direction < 0)
            {
                ret = ret.flipped;
            }

            norm = normal;

            return ret;
        }

        private void GetSliceThisRound()
        {
            int posCount = _lr.positionCount;

            if (posCount == 1) return;

            Vector3[] lrPositions = new Vector3[posCount];
            _lr.GetPositions(lrPositions);

            for (int i = 1; i < posCount; i++)
            {
                Vector3 pos = lrPositions[i];
                Vector3 lastPos = lrPositions[i - 1];

                // If its the last point
                // Make the reverse process (to grab the end of the log)
                if (i == posCount - 1)
                {
                    pos = lrPositions[i - 1];
                    lastPos = lrPositions[i];
                }

                Vector3 newPos = lastPos;

                while (newPos != pos)
                {
                    bool cond1 = false;
                    bool cond2 = false;
                    RaycastHit hit;
                    if (CheckForHit(newPos, out hit))
                    {
                        Vector3 originalNewPos = newPos;
                        cond1 = i == posCount - 1 && originalNewPos == lastPos;
                        if (cond1)
                        {
                            //Debug.Log("Last point within");
                            pos = lrPositions[i];
                            lastPos = lrPositions[i - 1];

                            RaycastHit hit2;
                            newPos = pos;

                            Vector3 go = pos - (lastPos - pos) * Time.deltaTime;

                            while (CheckForHitTwo(go, out hit2))
                            {
                                MakeRayhitSliceInfo(hit2, go);
                                go = go - (lastPos - pos) * Time.deltaTime;
                            }
                        }

                        pos = lrPositions[i - 1];
                        lastPos = lrPositions[i];
                        cond2 = i == 1 && originalNewPos == lastPos;
                        if (cond2)
                        {
                            //Debug.Log("First point within");
                            RaycastHit hit2;
                            newPos = pos;

                            Vector3 go = pos - (lastPos - pos) * Time.deltaTime;

                            while (CheckForHitTwo(go, out hit2))
                            {
                                MakeRayhitSliceInfo(hit2, go);
                                go = go - (lastPos - pos) * Time.deltaTime;
                            }
                        }

                        if (!cond1 && !cond2)
                        {
                            MakeRayhitSliceInfo(hit, newPos);
                        }
                    }
                    else if (_hits.Count > 1)
                    {
                        // At the very least, i need 2 hits
                        SliceThisRound = true;
                        CanSlice = false;
                    }

                    if((cond1 || cond2) && _hits.Count > 1)
                    {
                        // At the very least, i need 2 hits
                        SliceThisRound = true;
                        CanSlice = false;
                    }

                    if (pos != newPos)
                    {
                        newPos = Vector3.MoveTowards(newPos, pos, Time.deltaTime);
                    } 
                }
            }
        }

        private bool CheckForHitTwo(Vector3 startPosition, out RaycastHit hit)
        {
            Ray r = new Ray(startPosition, Camera.main.transform.TransformDirection(Vector3.forward) * 100f);
            if (Physics.Raycast(r, out hit, 100f, _whatWeAreCuttingLayerMask))
            {

                Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue, 10f);
                return true;
            }
            else
            {

                Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.forward) * 100f, Color.cyan, 10f);
                return false;
            }
        }

        private bool CheckForHit(Vector3 startPosition, out RaycastHit hit)
        {
            bool rc1 = Physics.Raycast(startPosition, Camera.main.transform.TransformDirection(Vector3.forward), out hit, 100f, _whatWeAreCuttingLayerMask);
            
            if (rc1)
            {
                //if(rc1) Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.back) * hit.distance, Color.red, 0f);
                //if(rc2) Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 0f);
                return true;
            }
            else
            {
                //Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.back) * 100f, Color.green, 0f);
                //Debug.DrawRay(startPosition, Camera.main.transform.TransformDirection(Vector3.forward) * 100f, Color.green, 0f);
                return false;
            }
        }

        private bool CheckForPlane(Vector3 startPosition, out RaycastHit hit)
        {
            return Physics.Raycast(startPosition, Camera.main.transform.TransformDirection(Vector3.back), out hit, 100f, _whatIsCameraPlaneLayerMask);
        }

        private void MakeRayhitSliceInfo(RaycastHit hit, Vector3 origin)
        {
            RayhitSliceInfo info;

            RaycastHit hat;
            if (CheckForPlane(origin, out hat))
            {
                info.rayHitCameraPoint = hat.point;
            }
            else
            {
                info.rayHitCameraPoint = Vector3.zero;
            }

            info.objVelocity = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
            info.rayHit = hit;
            info.objTransform = hit.transform;
            
            _hits.Add(info);
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

        private IEnumerator CheckForSliceTimeoutCo(float time)
        {
            CanSlice = false;
            SliceThisRound = false;

            yield return new WaitForSeconds(time);

            CanSlice = true;
        }

        private void TransferChildrenToNewSlices(GameObject[] slices, GameObject originalObject, Vector3 start, Vector3 normal)
        {
            Plane def = new Plane(normal, start);

            Transform[] t = originalObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in t)
            {
                if (child.name == originalObject.name) continue;

                GameObject clone = Instantiate(child.gameObject, child.position, child.rotation);
                if (def.GetSide(child.position))
                {
                    // positive slices[0]
                    clone.transform.SetParent(slices[0].transform);
                }
                else
                {
                    // negative slices[1]
                    clone.transform.SetParent(slices[1].transform);
                }
                clone.transform.localScale = child.localScale;
                clone.layer = 0;
            }
        }

        public void WaitForSliceTimeout()
        {
            StartCoroutine(CheckForSliceTimeoutCo(_timeForNextCut));
        }

        public void ClearHits()
        {
            _hits.Clear();
        }

        /// <summary>
        /// Slice an object with the given RayhitSliceInfo.
        /// </summary>
        /// <param name="toSlice"></param>
        /// <param name="_theHits"></param>
        /// <param name="destroy">If set to true, the object is destroyed. Else the object is deactivated.</param>
        /// <param name="cPlane">The plane that defined the cut</param>
        /// <returns></returns>
        public GameObject[] Slice(GameObject toSlice, List<RayhitSliceInfo> theHits, bool destroy)
        {
            if (toSlice == null)
            {
                throw new System.Exception("No gameobject given to slice!");
            }

            if(theHits.Count == 0)
            {
                throw new System.Exception("No points given to slice!");
            }

            // We get the first hitPoint and the last hitPoint
            float minx = theHits.Min(info => info.rayHit.point.x);
            float maxx = theHits.Max(info => info.rayHit.point.x);
            RayhitSliceInfo minXInfo = theHits.First(info => info.rayHit.point.x == minx);
            RayhitSliceInfo maxXInfo = theHits.First(info => info.rayHit.point.x == maxx);

            Vector3 sHPoint = minXInfo.rayHit.point;
            Vector3 fHPoint = maxXInfo.rayHit.point;

            if(sHPoint == fHPoint)
            {
                fHPoint = theHits.Last(info => info.rayHit.point != fHPoint).rayHit.point;
            }

            //mark1.transform.position = sHPoint;
            //mark2.transform.position = fHPoint;

            // The start point is the average of the camera points
            // The camera points are the points straight from the hit points to the screen plane
            Vector3 start = (minXInfo.rayHitCameraPoint + maxXInfo.rayHitCameraPoint) / 2;

            SendLogSlice(toSlice.GetPhotonView().ViewID, sHPoint, fHPoint, start, destroy);

            return CompleteSlice(toSlice, sHPoint, fHPoint, start, destroy);
        }

        /// Slice an object with the internal RayhitSliceInfo
        /// </summary>
        /// <param name="toSlice"></param>
        /// <param name="destroy">If set to true, the object is destroyed. Else the object is deactivated.</param>
        /// <param name="cPlane">The plane that defined the cut</param>
        /// <returns></returns>
        public GameObject[] Slice(GameObject toSlice, bool destroy)
        {
            if(!SliceThisRound)
            {
                throw new System.Exception("No points given to slice! SliceThisRound is false!");
            }

            if(toSlice == null)
            {
                throw new System.Exception("No gameobject given to slice!");
            }

            // We get the first hitPoint and the last hitPoint
            float minx = _hits.Min(info => info.rayHit.point.x);
            float maxx = _hits.Max(info => info.rayHit.point.x);
            RayhitSliceInfo minXInfo = _hits.First(info => info.rayHit.point.x == minx);
            RayhitSliceInfo maxXInfo = _hits.First(info => info.rayHit.point.x == maxx);

            Vector3 sHPoint = minXInfo.rayHit.point;
            Vector3 fHPoint = maxXInfo.rayHit.point;

            if (sHPoint == fHPoint)
            {
                fHPoint = _hits.Last(info => info.rayHit.point != fHPoint).rayHit.point;
            }

            //mark1.transform.position = sHPoint;
            //mark2.transform.position = fHPoint;

            // The start point is the average of the camera points
            // The camera points are the points straight from the hit points to the screen plane
            Vector3 start = (minXInfo.rayHitCameraPoint + maxXInfo.rayHitCameraPoint) / 2;

            SendLogSlice(toSlice.GetPhotonView().ViewID, sHPoint, fHPoint, start, destroy);

            return CompleteSlice(toSlice, sHPoint, fHPoint, start, destroy);
        }

        private GameObject[] CompleteSlice(GameObject toSlice, Vector3 startHitPoint, Vector3 finalHitPoint, Vector3 cameraStart, bool destroy)
        {
            Debug.Log("COMPLETE SLICE");
            Vector3 norm;
            Plane cutter = DefinePlane(
                toSlice.transform,
                startHitPoint,
                finalHitPoint,
                cameraStart,
                out norm
            );

            Rigidbody toSliceRb = toSlice.GetComponent<Rigidbody>();

            GameObject[] slices = Tvtig.Slicer.Slicer.Slice(cutter, toSlice);

            GeneralHelperFunctions.DrawPlane(startHitPoint, norm);

            TransferChildrenToNewSlices(slices, toSlice, startHitPoint, norm);

            if (toSliceRb != null)
            {
                Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
                // Maintaining the velocity in which the object came
                rigidbody.velocity = toSliceRb.velocity;
                slices[0].GetComponent<Rigidbody>().velocity = toSliceRb.velocity;

                // The small jump when cutting
                Vector3 newNormal = norm + Vector3.up * _forceOnCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }

            if (destroy) Destroy(toSlice.gameObject);
            else toSlice.SetActive(false);

            CreatePosNegSlices(slices);

            return slices;
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

        public List<RayhitSliceInfo> GetHits()
        {
            if (!SliceThisRound)
            {
                throw new System.Exception("There must be a cut to get the hits! Check CutThisRound boolean property on TouchSlicer.cs .");
            }

            return _hits;
        }

        ////// NETWORKING
        ///

        private void SendLogSlice(int logId, Vector3 shp, Vector3 fhp, Vector3 cs, bool destroy)
        {
            Debug.Log("BEFORE RPC");
            photonView.RPC("RPC_SliceLog", RpcTarget.Others, logId,
                                                             shp.x, shp.y, shp.z,
                                                             fhp.x, fhp.y, fhp.z,
                                                             cs.x, cs.y, cs.z,
                                                             destroy);
        }

        [PunRPC]
        public void RPC_SliceLog(int logId,
                                 float shpx, float shpy, float shpz,
                                 float fhpx, float fhpy, float fhpz,
                                 float csx, float csy, float csz,
                                 bool destroy)
        {
            Debug.Log("WITHIN RPC");
            Vector3 shp = new Vector3(shpx, shpy, shpz);
            Vector3 fhp = new Vector3(fhpx, fhpy, fhpz);
            Vector3 cs = new Vector3(csx, csy, csz);

            GameObject found = null;
            PhotonView[] logs = FindObjectsOfType<PhotonView>();

            foreach(PhotonView log in logs)
            {
                if(log.ViewID == logId)
                {
                    found = log.gameObject;
                    break;
                }
            }
            Debug.Log("COMPLETE SLICE NET");
            CompleteSlice(found, shp, fhp, cs, destroy);
        }
    }
}


