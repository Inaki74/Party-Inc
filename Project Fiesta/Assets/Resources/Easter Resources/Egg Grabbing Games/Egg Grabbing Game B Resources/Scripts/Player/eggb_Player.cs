using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggb_Player : MonoBehaviour
{
    #region Components
    [SerializeField]
    public Camera MainCamera { get; private set; }
    private eggb_PlayerInputManager InputManager;
    private BoxCollider Bc;
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

    public Vector3 moveToVector;

    private bool isMoving;
    private bool runOnce;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        InputManager = GetComponent<eggb_PlayerInputManager>();
        MainCamera = FindObjectOfType<Camera>();
        Bc = GetComponent<BoxCollider>();

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
        if (runOnce)
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
    private IEnumerator MoveToCo(float velocity, float direction)
    {
        if (direction > 0f)
        {
            moveToVector = railRight;
        }
        else if (direction < 0f)
        {
            moveToVector = railLeft;
        }
        else moveToVector = railMiddle;

        Bc.enabled = false;
        while (!CheckIfReachedPosition(moveToVector))
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToVector, velocity * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Bc.enabled = true;
        yield return new WaitForEndOfFrame();
        runOnce = true;
    }

    /// <summary>
    /// Provides soft movement to position v at a certain velocity.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="velocity"></param>
    /// <returns></returns>
    //private IEnumerator SmoothMovementCo(Vector3 v, float velocity)
    //{
        
    //}
    #endregion

    private bool CheckIfReachedPosition(Vector3 v)
    {
        return transform.position.x == v.x;
    }
}
