using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace KK
    {
        public class Mono_CameraTrolley_KK : MonoBehaviour
        {
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                transform.position += Vector3.right * Mng_GameManager_KK.Current.MovementSpeed * Time.deltaTime;
            }
        }
    }
}


