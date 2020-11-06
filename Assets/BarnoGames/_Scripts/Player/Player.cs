using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Player : LivingEntity, IControls
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
    private float GroundRaycastDistance = 0.11f;

    [Space(10)]
    [Header("Animator")]
    public Animator Anim;

    [Header("Caching")]
    private BoxCollider _boxCollider;
    private IMove imove;
    private IAttack iattack;

    private bool hasTakenDamage = false;

    private bool isGrounded;

    #region Unity Callbacks
    protected override void Start()
    {
        base.Start(); // Calls start from LivingEnitiy
        OnDeath += PlayerDeath;
        OnAddHealth += AddHealthFX;
        OnTakeHit += TakeHit;

        imove = GetComponent<IMove>();
        iattack = GetComponent<IAttack>();

        if (imove == null)
        {
            Debug.LogError("IMove Not Found");
        }

        if (Anim == null) Debug.LogError("Player Animator Not Assigned");

        _boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();

        if (isDebugEnabled)
        {
            Debug.DrawRay(transform.position , transform.TransformDirection(Vector3.right * RaycastDistance) , Color.red);
        }

        //if (!isDead)
        //{
        //    RaycastHit hit;

        //    if (Physics.Raycast(transform.position , transform.TransformDirection(Vector3.right) , out hit , RaycastDistance , ObsticleLayerMask))
        //    {
        //        //if (!hasTakenDamage)
        //        //{
        //        //    TakeDamage(1f);
        //        //    imove.StopMovement();
        //        //    hasTakenDamage = true;
        //        //}
        //    }
        //}
    }

    private void Update()
    {
        MovementDirection();
        JumpInput();
        AttackInput();
    }

    #endregion

    #region Private API
    private bool CheckIfGrounded()
    {
        //bool hitGround = Physics.BoxCast(boxCollider.bounds.center , new Vector3(0.5f , 0.5f , 0.5f) , Vector3.down , transform.rotation , GroundRaycastDistance , GroundLayerMask);
        bool hitGround = Physics.BoxCast(_boxCollider.bounds.center , _boxCollider.bounds.size.normalized , Vector3.down , transform.rotation , GroundRaycastDistance , GroundLayerMask);
        Color rayColor = Color.green;

        if (hitGround)
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(_boxCollider.bounds.center + new Vector3(_boxCollider.bounds.extents.x , 0) , Vector3.down * ( _boxCollider.bounds.extents.y + GroundRaycastDistance ) , rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x , 0) , Vector3.down * ( _boxCollider.bounds.extents.y + GroundRaycastDistance ) , rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x , _boxCollider.bounds.extents.y) , Vector3.right * ( _boxCollider.bounds.extents.x ) , rayColor);

        return hitGround;
    }

    private void PlayerDeath()
    {
        Debug.Log("Died");
        imove.StopMovement();
    }

    private void AddHealthFX()
    {
        Debug.Log("Sparks");
    }
    public void TakeHit()
    {

    }
    #endregion

    public void MovementDirection()
    {
        float direction = Input.GetAxisRaw(Constants.INPUT_HORIONTAL);
        imove.SetVelocity(direction);
    }

    #region Public API

    public void JumpInput()
    {
        imove.SetIsGrounded(isGrounded);

        Anim.SetBool(Constants.ANIM_IS_GROUNDED , isGrounded);

        if (isGrounded) Anim.SetBool(Constants.ANIM_JUMP , false);

        //bool ButtonDown = Input.GetButtonDown(Constants.INPUT_JUMP);
        bool ButtonHold = Input.GetButton(Constants.INPUT_JUMP);
        bool ButtonUp = Input.GetButtonUp(Constants.INPUT_JUMP);


        //if (ButtonDown)
        //    imove.SetJumpInput(ButtonDown);
        if (ButtonHold) imove.SetJumpInput(ButtonHold);
        if (ButtonUp) imove.SetJumpInput(ButtonUp);

    }

    public void AttackInput()
    {
        if (Input.GetButtonDown(Constants.INPUT_ATTACK))
        {
            iattack.Attack();
        }
    }

    #endregion
}
