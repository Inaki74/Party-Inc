using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggb_Player : MonoBehaviour
{
    #region Components
    [SerializeField]
    public Camera MainCamera { get; private set; }
    private eggb_PlayerInputManager InputManager;
    #endregion

    #region Instance Variables
    private Rail currentRail;
    private enum Rail
    {
        left,
        middle,
        right
    }

    private Vector3 railLeft;
    private Vector3 railMiddle;
    private Vector3 railRight;

    public float movementSpeed;
    public float maxRangeOfMovement;
    public float inputWaitTime;

    private bool isMoving;
    private bool runOnce;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        InputManager = GetComponent<eggb_PlayerInputManager>();
        MainCamera = FindObjectOfType<Camera>();

        railLeft = Constants.LEFT_LANE;
        railMiddle = Constants.MID_LANE;
        railRight = Constants.RIGHT_LANE;

        runOnce = true;

        currentRail = Rail.middle;
        transform.position = railMiddle;
    }

    private void Update()
    {
        isMoving = Mathf.Abs(InputManager.MovementDirection) > 0f;
    }

    private void FixedUpdate()
    {
        if (isMoving && runOnce)
        {
            runOnce = false;
            StartCoroutine(MoveToCo(movementSpeed, InputManager.MovementDirection));
        }
    }
    #endregion

    #region CoRoutine Functions
    /// <summary>
    /// Moves the player to the desired position
    /// </summary>
    private IEnumerator MoveToCo(float velocity, int direction)
    {
        Vector3 moveToVector;

        if(direction > 0f)
        {
            if (currentRail == Rail.left)
            {
                moveToVector = railMiddle;
                currentRail = Rail.middle;
            }
            else if (currentRail == Rail.middle)
            {
                moveToVector = railRight;
                currentRail = Rail.right;
            }
            else moveToVector = transform.position;
        }
        else
        {
            if (currentRail == Rail.right)
            {
                moveToVector = railMiddle;
                currentRail = Rail.middle;
            }
            else if (currentRail == Rail.middle)
            {
                moveToVector = railLeft;
                currentRail = Rail.left;
            }
            else moveToVector = transform.position;
        }

        StartCoroutine(SmoothMovementCo(moveToVector, velocity));
        yield return new WaitForSeconds(inputWaitTime);
        runOnce = true;
    }

    /// <summary>
    /// Provides soft movement to position v at a certain velocity.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="velocity"></param>
    /// <returns></returns>
    private IEnumerator SmoothMovementCo(Vector3 v, float velocity)
    {
        while (!CheckIfReachedPosition(v)) {
            transform.position = Vector3.MoveTowards(transform.position, v, velocity * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    private bool CheckIfReachedPosition(Vector3 v)
    {
        return transform.position == v;
    }
}
