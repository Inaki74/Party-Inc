using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The falling egg, in charge of itself.
/// </summary>
public class eggb_EasterEgg : MonoBehaviour
{
    #region Events
    public delegate void ActionObtain(int score);
    public static event ActionObtain onObtainEgg;

    public delegate void ActionSpawn(int score);
    public static event ActionSpawn onSpawnEgg;
    #endregion

    #region Egg Specifics
    private int scoreModifier;
    public EggType eggType;
    public enum EggType
    {
        normal,
        rotten,
        golden
    }
    #endregion

    #region Inspector Assignables
    public LayerMask whatIsGround;

    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material rottenMaterial;
    [SerializeField] private Material goldenMaterial;
    #endregion

    #region Components
    private MeshRenderer Mr;
    #endregion

    #region Physics Variables
    private float fallingAcceleration;
    private Vector3 directionVector;
    private float fallingSpeed;
    private float playerCollisionDistance;
    private float groundCollisionDistance;
    #endregion

    #region Boolean Variables
    private bool isMoving;
    private bool hitGround;
    private bool hitPlayer;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        Mr = GetComponent<MeshRenderer>();

        SetEgg(eggType);

        fallingAcceleration = 9.8f;
        fallingSpeed = 0f;
        playerCollisionDistance = 0.3f;
        groundCollisionDistance = 0.3f;
        directionVector = Vector3.down;
    }

    private void Update()
    {
        hitGround = CheckIfHitGround();
        hitPlayer = CheckIfHitPlayer();
        fallingSpeed += fallingAcceleration * Time.deltaTime;

        if (hitPlayer)
        {
            OnObtain();
        }

        if (hitGround)
        {
            OnBreak();
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.Translate(directionVector * fallingSpeed * Time.deltaTime);
        }
    }

    private void OnEnable()
    {
        isMoving = true;
    }

    private void OnDisable()
    {
        isMoving = false;
        fallingSpeed = 0f;
    }
    #endregion

    /// <summary>
    /// Sets the eggs settings depending of which type of egg it is.
    /// </summary>
    /// <param name="t"></param>
    private void SetEgg(EggType t)
    {
        switch (t)
        {
            case EggType.normal:
                Mr.material = normalMaterial;
                scoreModifier = 1;
                break;
            case EggType.rotten:
                Mr.material = rottenMaterial;
                scoreModifier = -1;
                break;
            case EggType.golden:
                Mr.material = goldenMaterial;
                scoreModifier = 3;
                break;
        }
    }

    /// <summary>
    /// What happens when an egg is obtained.
    /// </summary>
    private void OnObtain()
    {
        if (scoreModifier != -1)
            onSpawnEgg?.Invoke(scoreModifier);
        //Add a score of scoreModifier
        onObtainEgg?.Invoke(scoreModifier);
        //Play OnObtain animation (specific to each egg though)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// What happens when an egg is broken (reaches the ground without the player touching it).
    /// </summary>
    private void OnBreak()
    {
        if (scoreModifier != -1)
            onSpawnEgg?.Invoke(scoreModifier);
        //Play OnBreak animation (specific to each egg though)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the vector in which the egg moves.
    /// </summary>
    /// <param name="v"></param>
    public void SetDirectionVector(Vector3 v)
    {
        directionVector = v;
    }

    /// <summary>
    /// Checks if the egg hit the ground.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfHitGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCollisionDistance, whatIsGround);
        //return transform.position.y < collisionDistance;
    }

    /// <summary>
    /// Checks if the egg hit the player.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfHitPlayer()
    {
        RaycastHit hitDown;

        if(Physics.Raycast(transform.position, Vector3.down, out hitDown, playerCollisionDistance))
        {
            if (hitDown.collider.gameObject.tag == "Player" && !hitDown.collider.gameObject.GetComponent<eggb_Player>().GetIfStunned())
            {
                return true;
            }
        }
        
        return false;
    }
}
