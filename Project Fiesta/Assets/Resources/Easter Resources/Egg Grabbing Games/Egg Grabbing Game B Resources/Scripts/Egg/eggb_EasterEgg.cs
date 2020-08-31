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
    //private MeshRenderer Mr;
    private Animator anim;
    private Rigidbody Rb;
    public SphereCollider Sc;
    #endregion

    #region Physics Variables
    #endregion

    #region Boolean Variables
    private bool onlyHitOnce;
    private bool isMoving;
    private bool hitGround;
    private bool hitPlayer;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        
    }

    private void Awake()
    {
        Sc = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody>();


        SetEgg(eggType);
    }

    private void OnEnable()
    {
        Sc.enabled = true;
        Rb.useGravity = true;
    }

    private void OnDisable()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<eggb_Player>().GetIfStunned()) {
            Debug.Log("Collided player");
            Sc.enabled = false;
            Rb.useGravity = false;
            Rb.velocity = Vector3.zero;
            OnObtain();
        }

        if (collision.gameObject.tag == "Ground" || (collision.gameObject.tag == "Egg" && collision.rigidbody.position.y < 2f))
        {
            Debug.Log("Collided ground");
            Sc.enabled = false;
            Rb.useGravity = false;
            Rb.velocity = Vector3.zero;
            OnBreak();
        }
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
                //Mr.material = normalMaterial;
                scoreModifier = 1;
                break;
            case EggType.rotten:
                //Mr.material = rottenMaterial;
                scoreModifier = -1;
                break;
            case EggType.golden:
                //Mr.material = goldenMaterial;
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
        Debug.Log("Break");
        if (scoreModifier != -1)
            onSpawnEgg?.Invoke(scoreModifier);
        //Play OnBreak animation (specific to each egg though)
        anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, true);
    }

    public void FinishBreakAnimation()
    {
        anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, false);
        gameObject.SetActive(false);
    }

    ///// <summary>
    ///// Sets the vector in which the egg moves.
    ///// </summary>
    ///// <param name="v"></param>
    //public void SetDirectionVector(Vector3 v)
    //{
    //    directionVector = v;
    //}

    ///// <summary>
    ///// Checks if the egg hit the ground.
    ///// </summary>
    ///// <returns></returns>
    //private bool CheckIfHitGround()
    //{
    //    return Physics.Raycast(transform.position, Vector3.down, groundCollisionDistance, whatIsGround);
    //}

    ///// <summary>
    ///// Checks if the egg hit the player.
    ///// </summary>
    ///// <returns></returns>
    //private bool CheckIfHitPlayer()
    //{
    //    RaycastHit hitDown;

    //    if(Physics.Raycast(transform.position, Vector3.down, out hitDown, playerCollisionDistance))
    //    {
    //        if (hitDown.collider.gameObject.tag == "Player" && !hitDown.collider.gameObject.GetComponent<eggb_Player>().GetIfStunned())
    //        {
    //            return true;
    //        }
    //    }
        
    //    return false;
    //}
}


////have we moved more than our minimum extent? 
//Vector3 movementThisStep = transform.position - previousPosition;
//float movementSqrMagnitude = movementThisStep.sqrMagnitude;
 
//	   if (movementSqrMagnitude > sqrMinimumExtent) 
//		{ 
//	      float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
//        RaycastHit hitInfo; 
 
//	      //check for obstructions we might have missed 
//	      if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
//              {
//                 if (!hitInfo.collider)
//                     return;
 
//                 if (hitInfo.collider.isTrigger)
//                     hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);
 
//                 if (!hitInfo.collider.isTrigger)
//                     transform.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent; 
 
//              }
//	   } 
 
//	   previousPosition = transform.position; 