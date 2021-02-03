using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    public class Test : MonoBehaviour
    {
        public Rigidbody rb;
        private static Vector3 start = new Vector3(0f, -2f, -5f);
        public Vector3 objective;
        public float speed;
        public float time;

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, objective, Time.deltaTime * speed);

            if(transform.position == objective)
            {
                StartCoroutine(Wait());
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);

            objective = start;

            yield return new WaitUntil(() => transform.position == objective);

            Destroy(gameObject);
        }
    }
}


