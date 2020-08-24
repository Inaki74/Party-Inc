using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class eggb_PlayerInputManager : MonoBehaviour
{
    public float MovementDirection { get; private set; }
    private float widthMiddlePoint;

    void Start()
    {
        widthMiddlePoint = Screen.width / 2;
    }

    public void DoMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // TOUCH
            //if (CheckSideOfTouch(Touchscreen.current.position.ReadValue()))
            //{
            //    MovementDirection = 1;
            //}
            //else
            //{
            //    MovementDirection = -1;
            //}

            // PC OSX
            MovementDirection = ctx.ReadValue<float>();
        }

        if (ctx.canceled)
        {
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
