using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class eggb_PlayerInputManager : MonoBehaviour
{
    public int MovementDirection { get; private set; }

    //private Player player;

    private float widthMiddlePoint;
    // Start is called before the first frame update
    void Start()
    {
        //player = GetComponent<Player>();
        widthMiddlePoint = Screen.width / 2;
    }

    public void DoMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Debug.Log("Start");
        }

        if (ctx.performed)
        {
            Debug.Log("Sustain");

            if (Touchscreen.current.delta.x.ReadValue() > 5f)
            {
                MovementDirection = 1;
            }
            else if (Touchscreen.current.delta.x.ReadValue() < -5f)
            {
                MovementDirection = -1;
            }
            //CheckSideOfTouch(Touchscreen.current.position.ReadValue()) || 
        }

        if (ctx.canceled)
        {
            Debug.Log("End");

            MovementDirection = 0;
        }
    }

    /// <summary>
    /// Checks if the touch was on the left or right side of the screen
    /// </summary>
    /// <param name="touchPosition"></param>
    /// <returns>True if the screen was touched in the right side, false if otherwise</returns>
    private bool CheckSideOfTouch(Vector3 touchPosition)
    {
        return touchPosition.x > widthMiddlePoint;
    }
}
