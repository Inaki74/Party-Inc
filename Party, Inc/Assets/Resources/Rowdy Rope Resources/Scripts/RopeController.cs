using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FiestaTime
{
    /// <summary>
    /// Physics based rope controller, too unstable.
    /// At radius 3 movement -> breaks at speed = 5
    /// At radius 2 movement -> breaks at speed = 6
    /// At radius 1 movement -> breaks at about speed = 25
    /// </summary>
    public class RopeController : MonoBehaviour
    {
        [SerializeField] private GameObject ropeLink;
        [SerializeField] private GameObject startLink;
        [SerializeField] private GameObject endLink;
        [SerializeField] private GameObject hinge;

        private Rigidbody startRB;
        private FixedJoint startJoint;
        private Rigidbody endRB;
        private FixedJoint endJoint;

        private List<FixedJoint> ropeLinksJoints = new List<FixedJoint>();
        private List<Rigidbody> ropeLinksRbs = new List<Rigidbody>();
        private List<Transform> ropeLinksTrans = new List<Transform>();

        private float ropeLength;
        private float linkSeparation = 0.3f;
        private float angle = 0f;
        public float ropeSpeed = 0f;

        // Start is called before the first frame update
        void Start()
        {
            ropeLength = Vector3.Distance(startLink.transform.position, endLink.transform.position);
            startRB = startLink.GetComponent<Rigidbody>();
            startJoint = startLink.GetComponent<FixedJoint>();
            endRB = endLink.GetComponent<Rigidbody>();
            endJoint = endLink.GetComponent<FixedJoint>();

            InstantiateRope();
        }

        // Update is called once per frame
        void Update()
        {

            //startLink.transform.Rotate(new Vector3(0f, 2f, 3f)*2);
            //endLink.transform.Rotate(new Vector3(0f, 2f, 3f)*2);
        }

        private void FixedUpdate()
        {
            hinge.transform.localPosition = new Vector3(Mathf.Cos(angle) * 3, Mathf.Sin(angle) * 3, 0f);
            if (angle > Mathf.PI * 2)
            {
                angle = 0f;
            }
            angle += Time.fixedDeltaTime * ropeSpeed;
        }

        private void InstantiateRope()
        {
            int amountOfLinks = (int)(ropeLength / linkSeparation) - 1;
            if (amountOfLinks <= 0) return;

            // Create all the links
            for (int i = 0; i < amountOfLinks; i++)
            {
                GameObject link = Instantiate(ropeLink);

                ropeLinksTrans.Add(link.GetComponent<Transform>());
                ropeLinksJoints.Add(link.GetComponent<FixedJoint>());
                ropeLinksRbs.Add(link.GetComponent<Rigidbody>());
            }

            // Set the positions
            float z = linkSeparation;
            foreach (Transform t in ropeLinksTrans)
            {
                t.SetParent(transform);
                t.localPosition = new Vector3(0, 0, z);
                z += linkSeparation;
            }

            FixedJoint[] aux = ropeLinksJoints.ToArray();
            Rigidbody[] aux2 = ropeLinksRbs.ToArray();

            startJoint.connectedBody = aux2[0];

            // Link all joints
            for (int i = 0; i < amountOfLinks; i++)
            {
                if (i == 0)
                {
                    aux[i].connectedBody = startRB;
                }
                else
                {
                    aux[i].connectedBody = aux2[i - 1];
                }
            }

            endJoint.connectedBody = aux2[amountOfLinks - 1];
        }
    }
}

