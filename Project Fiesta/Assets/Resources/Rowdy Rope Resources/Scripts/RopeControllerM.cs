using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace RR
    {
        public class RopeControllerM : MonoBehaviour
        {
            public delegate void ActionLoopCompleted();
            public static event ActionLoopCompleted onLoopComplete;

            [SerializeField] private GameObject linkPrefab;
            [SerializeField] private GameObject startPoint;
            [SerializeField] private GameObject endPoint;

            [SerializeField] private float separation = 1.4f;

            private RopeLinkM[] ropeLinks;

            private float distanceStartEnd;
            private float startingAngle;
            private float speedIncreasePerRound = 0.2f; 

            private bool gameStarted;
            private bool firstRun;

            private bool chkpnt_Inverse;
            private bool chkpnt_Bursts;
            private bool chkpnt_Peak;
            private bool chkpnt_Death;

            public float angle;
            public float rotationSpeed = 1f;

            // Start is called before the first frame update
            void Start()
            {
                firstRun = true;
                gameStarted = false;

                distanceStartEnd = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);

                startingAngle = GameManager.Current.startingAngle;
                angle = startingAngle;
                rotationSpeed = 0f;
                InstantiateRope();
            }

            // Update is called once per frame
            void Update()
            {
                if (gameStarted)
                {
                    if (firstRun)
                    {
                        rotationSpeed = GameManager.Current.startingSpeed;
                        firstRun = false;
                    }

                    Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 5f), Color.red);
                    angle += Time.deltaTime * rotationSpeed;

                    if(angle > 5 * Mathf.PI /2 && rotationSpeed > 0f)
                    {
                        // Round completed
                        onLoopComplete?.Invoke();
                        angle = Mathf.PI/2;
                    }

                    if (angle < Mathf.PI/ 2 && rotationSpeed < 0f)
                    {
                        // Round completed
                        onLoopComplete?.Invoke();
                        angle = 5 * Mathf.PI / 2;
                    }
                }
                
            }

            private void Awake()
            {
                GameManager.onGameStart += OnGameStart;
                GameManager.onCheckpointReached += OnCheckPointReached;
            }

            private void OnDestroy()
            {
                GameManager.onGameStart -= OnGameStart;
                GameManager.onCheckpointReached -= OnCheckPointReached;
            }

            private void InstantiateRope()
            {
                // Instantiate the objects and store them in respective arrays.
                // Position objects
                float z = 0;
                int i = 0;
                List<RopeLinkM> ropeAux = new List<RopeLinkM>();
                while (z <= distanceStartEnd)
                {
                    GameObject GO = Instantiate(linkPrefab);

                    GO.transform.parent = transform;

                    float radius = Mathf.Pow(z, 2) / 40 - z / 2;

                    ropeAux.Add(GO.GetComponent<RopeLinkM>().Init(this, radius, z, i, startingAngle * Mathf.Deg2Rad));

                    z += separation;
                    i++;
                }

                ropeLinks = ropeAux.ToArray();
            }

            private void OnGameStart()
            {
                gameStarted = true;
            }

            private void OnCheckPointReached(int checkpoint)
            {
                switch (checkpoint)
                {
                    case 0:
                        // Mini Speed increase checkpoint
                        if (!chkpnt_Peak)
                        {
                            // Increase speed
                            // rotationalSpeed += speedIncreasePerRound; ?
                        }
                        break;
                    case 1:
                        // Inversion checkpoint
                        chkpnt_Inverse = true;
                        break;
                    case 2:
                        // Speed bursts checkpoint
                        chkpnt_Bursts = true;
                        break;
                    case 3:
                        // Speed peak
                        chkpnt_Peak = true;
                        break;
                    case 4:
                        // Death mode
                        chkpnt_Death = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}


