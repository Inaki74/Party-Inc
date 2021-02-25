using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mono_LinkPhys_Controller_RR : MonoBehaviour
{
    private Rigidbody Rb;
    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Rb.velocity.sqrMagnitude > 100f)
        {
            Rb.velocity = Vector3.zero;
        }

        if(Rb.angularVelocity.sqrMagnitude > 100f)
        {
            Rb.angularVelocity = Vector3.zero;
        }
    }
}
