using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Plane plane = new Plane(Vector3.right, Vector3.zero);

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float ent = 100.0f;
            if (plane.Raycast(ray, out ent))
            {
                Debug.Log("Plane Raycast hit at distance: " + ent);
                var hitPoint = ray.GetPoint(ent);

                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = hitPoint;
                Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
            }
            else
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        }
    }
}
