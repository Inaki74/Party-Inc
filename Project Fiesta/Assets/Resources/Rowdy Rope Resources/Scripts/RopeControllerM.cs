using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace RR
    {
        public class RopeControllerM : MonoBehaviour
        {


            [SerializeField] private GameObject linkPrefab;
            [SerializeField] private GameObject startPoint;
            [SerializeField] private GameObject endPoint;

            [SerializeField] private float separation = 1.4f;

            private RopeLinkM[] ropeLinks;

            private float distanceStartEnd;
            private float startingAngle;

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

                angle = Mathf.PI / 2;
                distanceStartEnd = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);

                startingAngle = GameManager.Current.startingAngle;
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
                    angle += Mathf.Abs(Time.deltaTime * rotationSpeed);

                    if(angle > Mathf.PI * 2)
                    {
                        // Round completed
                        GameManager.Current.RoundCompleted();
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

                    ropeAux.Add(GO.GetComponent<RopeLinkM>().Init(this, radius, z, i, 0f));

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
                            // rotationalSpeed += 0.2; ?
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


