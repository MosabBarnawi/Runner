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
    private float JumpVelocity = 30f;
    [SerializeField]
    private float MovementSpeed = 10f;

    private float direction;
    private bool canMove = true;

    [Space(10)]

    [Header("Jump Controls")]
    [SerializeField]
    private int maxNumberOfJumps = 2;
    //[SerializeField]
    //private float maxJumpHeigh = 5f;
    [SerializeField]
    private float maxJumpTimePerJump = 0.4f;

    [SerializeField]
    [Tooltip("When the Jump Button Is Held it Applies This Gravity Value")]
    private float gravityScale = 0.4f;
    [SerializeField]
    private float defultGravityScale = 0.8f;
    private float _gravity;

    private float _fallingTimerDelay;
    private int _jumpCounter;

    [Header("Caching")]
    private Rigidbody rb;
    private Player _player;

    private bool _isJumpPressed;
    protected bool isGrounded { get; private set; }

    #region Unity Callbacks

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();

        if (rb == null)
        {
            Debug.LogError("RigidBody not Found");
        }

        _gravity = defultGravityScale;
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

        if (_isJumpPressed)
        {
            if (_jumpCounter < maxNumberOfJumps)
            {
                //if (input == Input.GetButtonDown(Constants.INPUT_JUMP))
                //{
                //    rb.velocity = Vector3.up * JumpVelocity;
                //}

                if (_isJumpPressed == Input.GetButton(Constants.INPUT_JUMP) && _fallingTimerDelay <= maxJumpTimePerJump)

                {
                    _player.Anim.SetBool(Constants.ANIM_JUMP , true);
                    _player.Anim.SetBool(Constants.ANIM_HARD_LAND , false);

                    _gravity = gravityScale;

                    //if(transform.position.y < maxJumpHeigh)
                    rb.velocity = Vector3.up * JumpVelocity / 2;


                    _fallingTimerDelay += Time.deltaTime;
                }
                else _gravity = defultGravityScale;

                if (_isJumpPressed == Input.GetButtonUp(Constants.INPUT_JUMP))
                {
                    if (_fallingTimerDelay < maxJumpTimePerJump)
                    {
                        //rb.velocity = Vector3.up * JumpVelocity * 0.1f;

                        if (rb.velocity.y <= 5) _player.Anim.SetBool(Constants.ANIM_HARD_LAND , true);
                        else _player.Anim.SetBool(Constants.ANIM_HARD_LAND , false);
                    }

                    _player.Anim.SetBool(Constants.ANIM_JUMP , false);

                    _jumpCounter++;

                    _fallingTimerDelay = 0;
                }
            }
        }
    }

    private void MovePlayer( float direction )
    {
        _player.Anim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , direction);

        float speed = direction * MovementSpeed * Time.fixedDeltaTime;

        if (isAcceleration)
        {
            rb.velocity += new Vector3(speed , rb.velocity.y - _gravity , 0);
        }
        else
        {
            rb.velocity = new Vector3(speed * 100 , rb.velocity.y - _gravity , 0);
        }
    }
    #endregion

    #region Public API
    public void SetJumpInput( bool pressed )
    {
        _isJumpPressed = pressed;
    }

    public void SetIsGrounded( bool isGrounded )
    {
        this.isGrounded = isGrounded;
    }

    public void StopMovement()
    {
        canMove = false;
    }

    public void SetVelocity( float VelocityVector )
    {
        if (!isConstantMovement)
            direction = VelocityVector;
    }

    #endregion

}
