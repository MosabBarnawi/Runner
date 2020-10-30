using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    [Header("Debug Options")]
    public bool isDebugEnabled;

    [Space(10)]
    [Header("Ray Casting")]
    [SerializeField]
    private LayerMask ObsticleLayerMask;
    [SerializeField]
    private float RaycastDistance = 1.5f;

    [Space(5)]
    [SerializeField]
    private LayerMask GroundLayerMask;
    [SerializeField]
    private float GroundRaycastDistance = 0.5f;

    [Space(10)]
    [Header("Animator")]
    public Animator Anim;

    [Header("Caching")]
    private BoxCollider _boxCollider;
    private IMove imove;

    private bool hasTakenDamage = false;

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

        _boxCollider = GetComponent<BoxCollider>();
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
        //bool hitGround = Physics.BoxCast(boxCollider.bounds.center , new Vector3(0.5f , 0.5f , 0.5f) , Vector3.down , transform.rotation , GroundRaycastDistance , GroundLayerMask);
        bool hitGround = Physics.BoxCast(_boxCollider.bounds.center , _boxCollider.bounds.size.normalized , Vector3.down , transform.rotation , GroundRaycastDistance , GroundLayerMask);
        Color rayColor = Color.green;

        if (hitGround)
        {
            rayColor = Color.red;
        }

        //Debug.DrawRay(boxCollider.bounds.center + new Vector3(0.5f , 0) , Vector3.down * ( 0.5f + GroundRaycastDistance ) , rayColor);
        //Debug.DrawRay(boxCollider.bounds.center - new Vector3(0.5f , 0) , Vector3.down * ( 0.5f + GroundRaycastDistance ) , rayColor);
        //Debug.DrawRay(boxCollider.bounds.center - new Vector3(0.5f , 0.5f) , Vector3.right * ( 0.5f ) , rayColor);

        Debug.DrawRay(_boxCollider.bounds.center + new Vector3(_boxCollider.bounds.extents.x , 0) , Vector3.down * ( _boxCollider.bounds.extents.y + GroundRaycastDistance ) , rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x , 0) , Vector3.down * ( _boxCollider.bounds.extents.y + GroundRaycastDistance ) , rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x , _boxCollider.bounds.extents.y) , Vector3.right * ( _boxCollider.bounds.extents.x ) , rayColor);

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
