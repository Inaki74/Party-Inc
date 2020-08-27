using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggb_Player : MonoBehaviour
{
    #region Components
    [SerializeField]
    public Camera MainCamera { get; private set; }
    private eggb_PlayerInputManager InputManager;
    private MeshRenderer Mr;
    private BoxCollider Bc;
    #endregion

    public float movementSpeed;
    public float stunTime;
    

    #region Instance Variables
    private Vector3 railLeft;
    private Vector3 railMiddle;
    private Vector3 railRight;

    private Vector3 moveToVector;

    private bool runOnce;
    private bool isStunned;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        InputManager = GetComponent<eggb_PlayerInputManager>();
        MainCamera = FindObjectOfType<Camera>();
        Bc = GetComponent<BoxCollider>();
        Mr = GetComponent<MeshRenderer>();

        railLeft = Constants.LEFT_LANE;
        railMiddle = Constants.MID_LANE;
        railRight = Constants.RIGHT_LANE;

        runOnce = true;
        isStunned = false;

        transform.position = railMiddle;
    }

    private void FixedUpdate()
    {
        if (runOnce && !isStunned)
        {
            runOnce = false;
            StartCoroutine(MoveToCo(movementSpeed, InputManager.MovementDirection));
        }else if (isStunned)
        {
            StartCoroutine(MoveToCo(movementSpeed, 0));
        }
    }

    private void Awake()
    {
        eggb_EasterEgg.onObtainEgg += OnRottenObtain;
    }

    private void OnDestroy()
    {
        eggb_EasterEgg.onObtainEgg -= OnRottenObtain;
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
    /// A Coroutine that waits to trigger if stunned.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StunnedCo()
    {
        isStunned = true;
        Bc.enabled = false;
        yield return new WaitForSeconds(stunTime);
        Bc.enabled = true;
        isStunned = false;
    }

    /// <summary>
    /// A Coroutine responsible for animating stunned.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StunnedAnimationCo()
    {
        var tempColor = Mr.material.color;
        for (int i = 0; i < 8; i++)
        {
            //alpha 0
            tempColor.a = 0f;
            Mr.material.color = tempColor;
            yield return new WaitForSeconds(stunTime/16);
            //alpha1
            tempColor.a = 1f;
            Mr.material.color = tempColor;
            yield return new WaitForSeconds(stunTime/16);
        }
    }
    #endregion

    /// <summary>
    /// Check if the player has reached a certain vector3 position.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private bool CheckIfReachedPosition(Vector3 v)
    {
        return transform.position.x == v.x;
    }

    private void OnRottenObtain(int scoreModifier)
    {
        if(scoreModifier == -1)
        {

            StartCoroutine("StunnedCo");
            //StartCoroutine(StunnedAnimationCo());
        }
    }

    public bool GetIfStunned()
    {
        return isStunned;
    }
}
