using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerRigidMovement : MonoBehaviour, IMove
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

    private bool canMove = true;
    private Player _player;
    private Animator _playerAnim;

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


    #region Unity Callbacks

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _playerAnim = _player.Anim;

        if (rb == null)
        {
            Debug.LogError("RigidBody not Found");
        }
    }

    private void Update()
    {
        Jump();
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
                float direction = Input.GetAxisRaw(Constants.HORIONTAL);

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

    #endregion

    #region Private API
    private void Jump()
    {
        if (_player.isGrounded())
        {
            _jumpCounter = 0;
        }

        if (_jumpCounter < maxNumberOfJumps)
        {
            if (Input.GetButtonDown(Constants.JUMP))
            {
                rb.velocity = Vector3.up * JumpVelocity;
            }

            if (Input.GetButton(Constants.JUMP) && _fallingTimerDelay <= maxJumpTimePerJump)
            {
                rb.velocity = Vector3.up * JumpVelocity;
                _fallingTimerDelay += Time.fixedDeltaTime;
            }

            if (Input.GetButtonUp(Constants.JUMP))
            {

                if(_fallingTimerDelay < maxJumpTimePerJump)
                {
                    rb.velocity = Vector3.up * JumpVelocity * 0.1f;
                }

                _jumpCounter++;

                _fallingTimerDelay = 0;
            }
        }       
    }

    private void ErrorChecking()
    {
        if (_playerAnim == null) Debug.LogError("Anim Not Assigned");
        if (_player == null) Debug.LogError("Player Script not found");
    }

    private void MovePlayer( float direction )
    {
        float speed = direction * MovementSpeed * Time.deltaTime;

        _playerAnim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , direction);

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
    public void SetVelocity( Vector3 VelocityVector )
    {
        //transform.position += VelocityVector;
    }

    public void StopMovement()
    {
        canMove = false;
    }

    #endregion

}
