using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    [Header("Ray Casting")]
    [SerializeField]
    LayerMask ObsticleLayerMask;
    public float RaycastDistance = 2f;

    [SerializeField]
    LayerMask GroundLayerMask;
    public float GroundRaycastDistance = 0.01f;

    private BoxCollider boxCollider;
    private bool hasTakenDamage = false;

    [Header("Debug Options")]
    public bool isDebugEnabled;

    [Header("Animator")]
    public Animator Anim;

    private IMove imove;

    #region Unity Callbacks
    protected override void Start()
    {
        base.Start(); // Calls start from LivingEnitiy
        OnDeath += PlayerDeath;
        OnAddHealth += AddHealthFX;
        OnTakeHit += TakeHit;

        imove = GetComponent<IMove>();

        if (imove == null)
        {
            Debug.LogError("IMove Not Found");
        }

        if (Anim == null) Debug.LogError("Player Animator Not Assigned");

        boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (isDebugEnabled)
        {
            Debug.DrawRay(transform.position , transform.TransformDirection(Vector3.right * RaycastDistance) , Color.red);
        }

        if (!isDead)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position , transform.TransformDirection(Vector3.right) , out hit , RaycastDistance , ObsticleLayerMask))
            {
                //if (!hasTakenDamage)
                //{
                //    TakeDamage(1f);
                //    imove.StopMovement();
                //    hasTakenDamage = true;
                //}
            }
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    TakeDamage(1f);
        //}
    }

    private void OnDrawGizmos()
    {

    }
    #endregion

    #region Private API
    public bool isGrounded()
    {

        //bool hitGround = Physics.Raycast(boxCollider.bounds.center , Vector3.down , boxCollider.bounds.extents.y + GroundRaycastDistance , GroundLayerMask);
        //bool hitGround = Physics.BoxCast(boxCollider.bounds.center , boxCollider.bounds.size , Vector3.down , Quaternion.identity , GroundRaycastDistance , GroundLayerMask);
        bool hitGround = Physics.BoxCast(boxCollider.bounds.center , new Vector3(0.5f , 0.5f , 0.5f) , Vector3.down , transform.rotation , GroundRaycastDistance , GroundLayerMask);
        Color rayColor = Color.green;

        if (hitGround)
        {
            rayColor = Color.red;
        }

        //Debug.DrawRay(boxCollider.bounds.center + new Vector3(0.5f , 0) , Vector3.down * ( 0.5f + GroundRaycastDistance ) , rayColor);
        //Debug.DrawRay(boxCollider.bounds.center - new Vector3(0.5f , 0) , Vector3.down * ( 0.5f + GroundRaycastDistance ) , rayColor);
        //Debug.DrawRay(boxCollider.bounds.center - new Vector3(0.5f , 0.5f) , Vector3.right * ( 0.5f ) , rayColor);

        Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x , 0) , Vector3.down * ( boxCollider.bounds.extents.y + GroundRaycastDistance ) , rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x , 0) , Vector3.down * ( boxCollider.bounds.extents.y + GroundRaycastDistance ) , rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x , boxCollider.bounds.extents.y) , Vector3.right * ( boxCollider.bounds.extents.x ) , rayColor);

        return hitGround;
    }

    private void PlayerDeath()
    {
        Debug.Log("Died");
        imove.StopMovement();
        // DISABLE MOVEMENT
    }

    private void AddHealthFX()
    {
        Debug.Log("Sparks");
    }
    public void TakeHit()
    {

    }
    #endregion
}
