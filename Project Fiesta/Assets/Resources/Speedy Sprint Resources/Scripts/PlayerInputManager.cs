using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        public class PlayerInputManager : MonoBehaviour
        {
            public bool JumpInput { get; private set; }
            public bool JumpInputPressed { get; private set; }
            public bool DuckInput { get; private set; }
            public bool MoveInput { get; private set; }
            public int MoveDirection { get; private set; }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            { 
                MoveInput = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveDirection = -1;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveDirection = 1;
                }

                JumpInput = Input.GetKeyDown(KeyCode.UpArrow);

                JumpInput = Input.GetKey(KeyCode.UpArrow);

                DuckInput = Input.GetKeyDown(KeyCode.DownArrow);
            }
        }
    }
}


