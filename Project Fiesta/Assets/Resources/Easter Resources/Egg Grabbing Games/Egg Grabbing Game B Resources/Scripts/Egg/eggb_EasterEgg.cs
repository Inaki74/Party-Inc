using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The falling egg, in charge of itself.
/// </summary>
public class eggb_EasterEgg : MonoBehaviour
{
    public delegate void ActionObtain(int score);
    public static event ActionObtain onObtainEgg;

    public delegate void ActionSpawn(int score);
    public static event ActionSpawn onSpawnEgg;

    public EggType eggType;

    public enum EggType
    {
        normal,
        rotten,
        golden
    }

    public LayerMask whatIsGround;

    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material rottenMaterial;
    [SerializeField] private Material goldenMaterial;

    private MeshRenderer Mr;

    private int scoreModifier;

    private Vector3 directionVector;

    private float fallingSpeed;
    private float collisionDistance;

    private bool isMoving;
    private bool hitGround;
    private bool hitPlayer;

    private void Start()
    {
        Mr = GetComponent<MeshRenderer>();

        SetEgg(eggType);

        fallingSpeed = 9.8f;
        collisionDistance = 0.1f;
        directionVector = Vector3.down;
    }

    private void Update()
    {
        hitGround = CheckIfHitGround();
        hitPlayer = CheckIfHitPlayer();

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            OnObtain();
        }
    }

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

    private void OnObtain()
    {
        if (scoreModifier != -1)
            onSpawnEgg?.Invoke(scoreModifier);
        //Add a score of scoreModifier
        onObtainEgg?.Invoke(scoreModifier);
        //Play OnObtain animation (specific to each egg though)
        gameObject.SetActive(false);
    }

    private void OnBreak()
    {
        if (scoreModifier != -1)
            onSpawnEgg?.Invoke(scoreModifier);
        //Play OnBreak animation (specific to each egg though)
        gameObject.SetActive(false);
    }

    public void SetDirectionVector(Vector3 v)
    {
        directionVector = v;
    }

    private bool CheckIfHitGround()
    {
        return transform.position.y < collisionDistance;
    }

    private bool CheckIfHitPlayer()
    {
        RaycastHit hitDown;

        if(Physics.Raycast(transform.position, Vector3.down, out hitDown, collisionDistance))
        {
            if (hitDown.collider.gameObject.tag == "Player")
            {
                return true;
            }
        }
        
        return false;
    }
}
