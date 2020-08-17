using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [SerializeField]
    private PlayerData playerData;
    public Camera MainCamera { get; private set; }
    private PlayerInputManager inputManager;
    private Rigidbody Rb;
    private BoxCollider Bc;
    #endregion

    #region Instance Variables
    private Vector3 auxVector;
    private Vector3 lastPosition;

    private bool isMoving;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        Rb = GetComponent<Rigidbody>();
        Bc = GetComponent<BoxCollider>();
        MainCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        isMoving = Mathf.Abs(inputManager.MovementDirection) > 0f;
    }

    private void FixedUpdate()
    {
        if (isMoving)
            MoveOnX(playerData.movementSpeed, inputManager.MovementDirection);
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
        if(Mathf.Abs(transform.position.x) < playerData.maxRangeOfMovement)
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
}
