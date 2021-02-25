using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace PartyInc
{
    public struct RayhitSliceInfo
    {
        public int id;
        public Vector3 objVelocity;
        public RaycastHit rayHit;
        public Transform objTransform;
        public Vector3 rayHitCameraPoint;

        public override bool Equals(object obj)
        {
            RayhitSliceInfo other = (RayhitSliceInfo)obj;

            return rayHit.point.Equals(other.rayHit.point);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public override string ToString()
        {
            return "RayhitSliceInfo, ID: " + id +
                    ", Point: (" + rayHit.point.x + ", " + rayHit.point.y + ", " + rayHit.point.z + ")" +
                    ", object Velocity: (" + objVelocity.x + ", " + objVelocity.y + ", " + objVelocity.z + ")";
        }
    }

    /// <summary>
    /// Now this! This is mine.
    /// An input MonoBehaviour which allows us to slice gameobjects and give us the hitpoints of those slices.
    /// </summary>
    ///
    [RequireComponent(typeof(LineRenderer))]
    public class Mono_TouchSlicer_CC : MonoBehaviourPun
    {
        [SerializeField] private LineRenderer _lr;

        // Colors of the Line Renderer
        [SerializeField] private Color _c1 = Color.yellow;
        [SerializeField] private Color _c2 = Color.red;

        // The force in which object halves jump when sliced
        [SerializeField] private float _forceOnCut;

        // The slashing effects (if you want to add)
        [SerializeField] private GameObject _slashingParticles;

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

        private int _idCount;

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
                    // Remove repeated items
                    var distinct = _hits.Distinct();
                    _hits = distinct.ToList();

                    bool cond1 = false;
                    bool cond2 = false;
                    RaycastHit hit;
                    if (CheckForHit(newPos, out hit))
                    {
                        // This is needed for the very special case in which the only two points of the slicer are within the log.
                        Vector3 originalNewPos = newPos;
                        // Cond1 = "If this is the last point of the slicer and its within the log (the Raycast hits)"
                        cond1 = i == posCount - 1 && originalNewPos == lastPos;
                        if (cond1)
                        {
                            // Undo the reverse process from before (I want it to start at the last hit all the way to the end of the log).
                            pos = lrPositions[i];
                            lastPos = lrPositions[i - 1];
                            // This is so that the while loop ends afterwards.
                            newPos = pos;

                            // This raycasts in a direction endlessly until there is no hit on the log.
                            // It generates all the other points we need on the log
                            RaycastToEndOfLog(pos - (lastPos - pos) * Time.deltaTime, lastPos - pos);
                        }

                        // Undo the reversal within cond1 if statement (I want to go backwards here)
                        pos = lrPositions[i - 1];
                        lastPos = lrPositions[i];
                        // Cond2 = "If this is the first point of the slicer and its within the log (the Raycast hits)"
                        cond2 = i == 1 && originalNewPos == lastPos;
                        if (cond2)
                        {
                            // This is so that the while loop ends afterwards.
                            newPos = pos;

                            // This raycasts in a direction endlessly until there is no hit on the log.
                            // It generates all the other points we need on the log
                            RaycastToEndOfLog(pos - (lastPos - pos) * Time.deltaTime, lastPos - pos);
                        }

                        // If both cond1 and cond2 didn't happen, then its a normal case.
                        if (!cond1 && !cond2)
                        {
                            MakeRayhitSliceInfo(hit, newPos);
                        }
                    }
                    else if (_hits.Count > 1)
                    {
                        // At the very least, i need 2 different hits
                        // This ends the slicing
                        SliceThisRound = true;
                        CanSlice = false;
                    }

                    // If any of both cond1 and cond2 happened and I have more than one hit, end the slice
                    if((cond1 || cond2) && _hits.Count > 1)
                    {
                        // At the very least, i need 2 different hits
                        SliceThisRound = true;
                        CanSlice = false;
                    }

                    // Just in case if MoveTowards can change newPos somehow and not make it end when necessary.
                    if (pos != newPos)
                    {
                        newPos = Vector3.MoveTowards(newPos, pos, Time.deltaTime);
                    }

                }
            }
        }

        private void RaycastToEndOfLog(Vector3 go, Vector3 direction)
        {
            RaycastHit hit2;

            while (CheckForHitTwo(go, out hit2))
            {
                MakeRayhitSliceInfo(hit2, go);
                go = go - direction * Time.deltaTime;
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
            info.id = _idCount;
            _idCount++;
            
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

        private Plane DefinePlane(Transform logT, Vector3 startHp, Vector3 finishHp, Vector3 start, Vector3 velocity, out Vector3 norm)
        {
            Plane ret = new Plane();

            startHp = startHp + velocity;
            finishHp = finishHp + velocity;
            start = start + velocity;

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
                try
                {
                    fHPoint = theHits.Last(info => info.rayHit.point != fHPoint).rayHit.point;
                }

                catch(System.InvalidOperationException e)
                {
                    Debug.LogError("PartyInc/TouchSlicer: The points that we have are all the same, cant find a different point.");
                    DebugExtensions.PrintList(_hits);
                }
            }

            //mark1.transform.position = sHPoint;
            //mark2.transform.position = fHPoint;

            // The start point is the average of the camera points
            // The camera points are the points straight from the hit points to the screen plane
            Vector3 start = (minXInfo.rayHitCameraPoint + maxXInfo.rayHitCameraPoint) / 2;

            SendObjectSlice(toSlice, sHPoint, fHPoint, start, destroy);

            return CompleteSlice(toSlice, sHPoint, fHPoint, start, Vector3.zero, destroy);
        }

        /// Slice an object with the internal RayhitSliceInfo
        /// </summary>
        /// <param name="toSlice"></param>
        /// <param name="destroy">If set to true, the object is destroyed. Else the object is deactivated.</param>
        /// <param name="cPlane">The plane that defined the cut</param>
        /// <returns></returns>
        public GameObject[] Slice(GameObject toSlice, bool destroy)
        {
            return Slice(toSlice, _hits, destroy);
        }

        private GameObject[] CompleteSlice(GameObject toSlice, Vector3 startHitPoint, Vector3 finalHitPoint, Vector3 cameraStart, Vector3 differenceThruNet, bool destroy)
        {
            Vector3 norm;
            Plane cutter = DefinePlane(
                toSlice.transform,
                startHitPoint,
                finalHitPoint,
                cameraStart,
                differenceThruNet,
                out norm
            );

            Rigidbody toSliceRb = toSlice.GetComponent<Rigidbody>();

            GameObject[] slices = Tvtig.Slicer.Slicer.Slice(cutter, toSlice);

            DebugExtensions.DrawPlane(startHitPoint + differenceThruNet, norm);

            TransferChildrenToNewSlices(slices, toSlice, startHitPoint, norm);

            if (toSliceRb != null)
            {
                Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
                //// Maintaining the velocity in which the object came
                //rigidbody.velocity = velocity;
                //slices[0].GetComponent<Rigidbody>().velocity = velocity;

                // The small jump when cutting
                Vector3 newNormal = norm + new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0.8f) * _forceOnCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }

            if (destroy) Destroy(toSlice.gameObject);
            else toSlice.SetActive(false);

            CreatePosNegSlices(slices, toSlice.GetComponent<Tvtig.Slicer.Mono_Sliceable_CC>());

            if(_slashingParticles != null)
            {
                SpawnSlashingParticles(startHitPoint, finalHitPoint, startHitPoint + differenceThruNet);
            }
            
            return slices;
        }

        private void SpawnSlashingParticles(Vector3 start, Vector3 end, Vector3 slashPos)
        {
            Vector3 direction = end - start;

            float angle = Vector3.SignedAngle(new Vector3(-1f, 0f, direction.z), direction, Vector3.back);

            if (angle < 0f)
            {
                Instantiate(_slashingParticles, slashPos, Quaternion.Euler(angle + 90f, 90f, -90f));
            }
            else
            {
                Instantiate(_slashingParticles, slashPos, Quaternion.Euler(angle - 90f, 90f, -90f));
            }
        }

        private void CreatePosNegSlices(GameObject[] slices, Tvtig.Slicer.Mono_Sliceable_CC sliceableInfo)
        {
            // Positive slice
            GameObject pos = slices[0];
            GameObject neg = slices[1];

            if (sliceableInfo.DeactivateAfterTime)
            {
                //Time

                Mono_DeactivateAfterTime d1 = pos.AddComponent<Mono_DeactivateAfterTime>();
                d1.SetToDestroy(sliceableInfo.Destroyy);
                d1.SetTimeToDeactivate(sliceableInfo.TimeOrDistanceToDeactivate);

                // Negative slice
                Mono_DeactivateAfterTime d2 = neg.AddComponent<Mono_DeactivateAfterTime>();
                d2.SetToDestroy(sliceableInfo.Destroyy);
                d2.SetTimeToDeactivate(sliceableInfo.TimeOrDistanceToDeactivate);
            }
            else
            {
                //Falling
                Mono_DeactivateAfterFalling d1 = pos.AddComponent <Mono_DeactivateAfterFalling>();
                d1.SetToDestroy(sliceableInfo.Destroyy);
                d1.SetDistanceToDeactivate(sliceableInfo.TimeOrDistanceToDeactivate);

                Mono_DeactivateAfterFalling d2 = neg.AddComponent<Mono_DeactivateAfterFalling>();
                d2.SetToDestroy(sliceableInfo.Destroyy);
                d2.SetDistanceToDeactivate(sliceableInfo.TimeOrDistanceToDeactivate);
            }

            if (!sliceableInfo.SlicesHaveColliders)
            {
                StartCoroutine(DisableThemCo(pos, neg));
            }

            if (sliceableInfo.SlicesHaveConstantForce)
            {
                // Add more gravity to the pieces
                ConstantForce c1 = pos.AddComponent<ConstantForce>();
                ConstantForce c2 = neg.AddComponent<ConstantForce>();
                c1.force = sliceableInfo.SlicesConstantForce;
                c2.force = sliceableInfo.SlicesConstantForce;
            }
            
        }

        private IEnumerator DisableThemCo(GameObject pos, GameObject neg)
        {
            // Wait for a bit to get the physics rotations and good looking stuff
            yield return new WaitForSeconds(0.2f);

            pos.GetComponent<MeshCollider>().enabled = false;
            neg.GetComponent<MeshCollider>().enabled = false;
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

        private void SendObjectSlice(GameObject toSlice, Vector3 shp, Vector3 fhp, Vector3 cs, bool destroy)
        {
            object[] content = new object[] { toSlice.GetPhotonView().ViewID, shp, fhp, cs, toSlice.transform.position, destroy };
            photonView.RPC("RPC_SliceObject", RpcTarget.Others, content as object);
        }

        [PunRPC]
        public void RPC_SliceObject(object[] content, PhotonMessageInfo info)
        {
            Vector3 shp = (Vector3)content[1];
            Vector3 fhp = (Vector3)content[2];
            Vector3 cs = (Vector3)content[3];
            Vector3 px = (Vector3)content[4];

            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

            GameObject found = null;
            PhotonView[] objects = FindObjectsOfType<PhotonView>();

            foreach(PhotonView obj in objects)
            {
                if(obj.ViewID == (int)content[0])
                {
                    found = obj.gameObject;
                    break;
                }
            }

            if(found == null)
            {
                Debug.LogError("PartyInc/TouchSlicer: COULDNT FIND OBJECT. (DID YOU DESTROY IT?)");
            }

            Vector3 p = found.transform.position;

            CompleteSlice(found, shp, fhp, cs, p - px, (bool)content[5]);
        }
    }
}


