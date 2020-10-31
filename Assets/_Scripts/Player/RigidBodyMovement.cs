using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RigidBodyMovement : MonoBehaviour, IMove
{
    [Header("Movement Controls")]

    [Range(-1f , 1.0f)]
    public float MovementDirection;

    [SerializeField]
    private bool
        hardStop = true,
        isConstantMovement = false,
        isAcceleration = false;

    [SerializeField]
    private float JumpVelocity = 8f;
    [SerializeField]
    private float MovementSpeed = 10f;

    public float direction;

    private bool canMove = true;

    [Space(10)]

    [Header("Jump Controls")]
    [SerializeField]
    private int maxNumberOfJumps = 2;
    [SerializeField]
    private float maxJumpTimePerJump = 0.4f;
    [SerializeField]
    private float gravityScale = 0.4f;

    private float _fallingTimerDelay;
    private int _jumpCounter;

    [Header("Caching")]
    private Rigidbody rb;

    private bool _isJumpPressed;
    protected bool isGrounded { get; private set;}

    #region Unity Callbacks

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("RigidBody not Found");
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (isConstantMovement)
            {
                MovePlayer(MovementDirection);
            }
            else
            {
                if (hardStop)
                {
                    if (direction != 0)
                    {
                        MovePlayer(direction);
                    }
                    else
                    {
                        MovePlayer(0);
                    }
                }
                else
                {
                    MovePlayer(direction);
                }
            }
        }
    }

    private void Update()
    {
        Jump();
    }

    #endregion

    #region Private API
    private void Jump()
    {
        if (isGrounded)
        {
            _jumpCounter = 0;
        }

        if(_isJumpPressed)
        {
            if (_jumpCounter < maxNumberOfJumps)
            {
                //if (input == Input.GetButtonDown(Constants.INPUT_JUMP))
                //{
                //    rb.velocity = Vector3.up * JumpVelocity;
                //}

                if (_isJumpPressed == Input.GetButton(Constants.INPUT_JUMP) && _fallingTimerDelay <= maxJumpTimePerJump)
                {
                    rb.velocity = Vector3.up * JumpVelocity;
                    _fallingTimerDelay += Time.fixedDeltaTime;
                }

                if (_isJumpPressed == Input.GetButtonUp(Constants.INPUT_JUMP))
                {

                    if (_fallingTimerDelay < maxJumpTimePerJump)
                    {
                        rb.velocity = Vector3.up * JumpVelocity * 0.1f;
                    }

                    _jumpCounter++;

                    _fallingTimerDelay = 0;
                }
            }
        }
    }

    private void MovePlayer( float direction )
    {
        float speed = direction * MovementSpeed * Time.deltaTime;

        if (isAcceleration)
        {
            rb.velocity += new Vector3(speed , rb.velocity.y - gravityScale , 0);
        }
        else
        {
            rb.velocity = new Vector3(speed * 100 , rb.velocity.y - gravityScale , 0);
        }
    }
    #endregion

    #region Public API
    public void SetJumpInput(bool pressed)
    {
        _isJumpPressed = pressed;
    }

    public void SetIsGrounded( bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    public void StopMovement()
    {
        canMove = false;
    }

    public void SetVelocity( float VelocityVector )
    {
        direction = VelocityVector;
    }

    #endregion

}
