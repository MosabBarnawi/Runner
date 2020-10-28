using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidMovement : MonoBehaviour, IMove
{
    private Rigidbody rb;

    [Range(-1f , 1.0f)]
    public float dir;

    public bool HardStop = true;
    public bool isConstantMovement;
    public bool isAcceleration = false;

    [Header("Controls")]
    public float JumpVelocity;
    public float MovementSpeed = 10f;
    private bool canMove = true;
    private Player player;
    private Animator playerAnim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        playerAnim = player.Anim;

        if (rb == null)
        {
            Debug.LogError("RigidBody not Found");
        }
    }

    void FixedUpdate()
    {
        if (player.isGrounded())
        {
            Jump();
        }

        if (canMove)
        {
            if (isConstantMovement)
            {
                MovePlayer(dir);
            }
            else
            {
                float direction = Input.GetAxis(Constants.HORIONTAL);

                if (HardStop)
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


    #region Private API
    private void Jump()
    {
        if (Input.GetButtonDown(Constants.JUMP))
        {
            rb.velocity = Vector3.up * JumpVelocity;
        }
    }

    private void ErrorChecking()
    {
        if (playerAnim == null) Debug.LogError("Anim Not Assigned");
        if (player == null) Debug.LogError("Player Script not found");
    }

    private void MovePlayer( float direction )
    {
        float speed = direction * MovementSpeed * Time.deltaTime;

        playerAnim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , direction);

        if (isAcceleration)
        {
            rb.velocity += new Vector3(speed , rb.velocity.y , 0);
        }
        else
        {
            rb.velocity = new Vector3(speed * 100 , rb.velocity.y , 0);
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
