using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_CameraTrolley_LL : MonoBehaviour
        {
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                transform.position += Vector3.right * Mng_GameManager_LL.Current.MovementSpeed * Time.deltaTime;
            }
        }
    }
}



