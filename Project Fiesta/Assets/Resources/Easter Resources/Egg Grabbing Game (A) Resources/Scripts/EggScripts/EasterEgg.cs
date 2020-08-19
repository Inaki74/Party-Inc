using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGGA_EasterEgg
{
public class EasterEgg : MonoBehaviour
{
    public LayerMask whatIsGround;

    [SerializeField] protected Material material;
    protected MeshRenderer Mr;

    protected int scoreModifier;
    protected int id;
    
    protected Vector3 directionVector;

    protected float fallingSpeed;
    protected float collisionDistance;

    protected bool isMoving;
    protected bool hitGround;

    protected virtual void Start()
    {
        Mr = GetComponent<MeshRenderer>();
        Mr.material = material;
        scoreModifier = 1;
        fallingSpeed = 9.8f;
        collisionDistance = 0.1f;
        id = 0;
    }

    protected void Update()
    {
        hitGround = CheckIfHitGround();

        Debug.Log(hitGround);

        if (hitGround)
        {
            OnBreak();
        }
    }

    protected void FixedUpdate()
    {
        if (isMoving)
        {
            transform.Translate(directionVector * fallingSpeed * Time.deltaTime);
        }
    }

    protected void OnEnable()
    {
        isMoving = true;
    }

    protected void OnDisable()
    {
        isMoving = false;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            OnObtain();
        }
    }

    protected virtual void OnObtain()
    {
        //Add a score of scoreModifier
        //Play OnObtain animation (specific to each egg though)
        gameObject.SetActive(false);
    }

    protected virtual void OnBreak()
    {
        //Play OnBreak animation (specific to each egg though)
        gameObject.SetActive(false);
    }

    public void SetDirectionVector(Vector3 v)
    {
        directionVector = v;
    }

    private bool CheckIfHitGround()
    {
        return transform.position.y < 0.2;
    }
}
}
