using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace AS
    {
        public class Mono_CameraTrolley_AS : MonoBehaviour
        {
            [SerializeField] private Cinemachine.CinemachineImpulseSource _shake;
            [SerializeField] private float _force;
            [SerializeField] private Vector3 _someSpeed;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                transform.position += Vector3.right * Mng_GameManager_AS.Current.MovementSpeed * Time.deltaTime;
            }
        }
    }
}
