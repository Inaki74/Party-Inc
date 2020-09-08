using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Photon.Pun;

public class eggb_PlayerInputManager : MonoBehaviourPun
{
    public int MovementDirection { get; private set; }
    private float widthMiddlePoint;

    public Text a;

    private int lastFrameCount;

    void Start()
    {
        widthMiddlePoint = Screen.width / 2;
        lastFrameCount = 0;
    }

    public void DoMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            int amountOfTouches = CountTouches();

            a.text = "" + Touchscreen.current.primaryTouch.phase.ReadValue().ToString();
            
            // TOUCH
            if (CheckSideOfTouch(Touchscreen.current.touches[amountOfTouches -1].position.ReadValue()))
            {
                MovementDirection = 1;
            }
            else
            {
                MovementDirection = -1;
            }


            // PC OSX
            //MovementDirection = (int)ctx.ReadValue<float>();

            if (amountOfTouches != lastFrameCount)
            {
                lastFrameCount = amountOfTouches;
            }
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

    private int CountTouches()
    {
        int ret = 0;

        foreach(var touch in Touchscreen.current.touches)
        {
            if(touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                ret++;
            }
        }

        return ret;
    }
}
