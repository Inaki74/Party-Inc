using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [SerializeField]
    public Camera MainCamera { get; private set; }
    private PlayerInputManager inputManager;
    private Rigidbody Rb;
    private BoxCollider Bc;
    #endregion

    #region Instance Variables
    private Vector3 auxVector;
    private Vector3 lastPosition;

    public float movementSpeed;
    public float maxRangeOfMovement;

    private bool isMoving;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        Rb = GetComponent<Rigidbody>();
        Bc = GetComponent<BoxCollider>();
        MainCamera = FindObjectOfType<Camera>();

        SetLimits(Screen.width - Screen.width * 10/100);
        SetSpeed(Screen.width - Screen.width * 10/100);
    }

    private void Update()
    {
        isMoving = Mathf.Abs(inputManager.MovementDirection) > 0f;
    }

    private void FixedUpdate()
    {
        if (isMoving)
            MoveOnX(movementSpeed, inputManager.MovementDirection);
    }
    #endregion

    #region Physics Functions
    /// <summary>
    /// Moves the player on X according to input.
    /// </summary>
    private void MoveOnX(float velocity, int direction)
    {
        float newPosX = velocity * direction * Time.fixedDeltaTime;
        auxVector.Set(newPosX, 0, 0);
        if(Mathf.Abs(transform.position.x) < maxRangeOfMovement)
        {
            lastPosition = transform.position;
            transform.Translate(auxVector);
        }
        else
        {
            transform.position = lastPosition;
        }
    }
    #endregion

    private void SetLimits(float width)
    {
        maxRangeOfMovement = MainCamera.ScreenToWorldPoint(new Vector3(width, 0f, MainCamera.transform.position.z)).x * -1;
    }

    private void SetSpeed(float width)
    {
        movementSpeed = MainCamera.ScreenToWorldPoint(new Vector3(width, 0f, MainCamera.transform.position.z)).x * -1;
    }
}
