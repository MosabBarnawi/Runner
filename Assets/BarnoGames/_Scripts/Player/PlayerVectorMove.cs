using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVectorMove : MonoBehaviour, IMove
{
    [Range(-1f , 1.0f)]
    public float dir;

    public bool isConstantMovement;
    public float MovementSpeed = 10f;

    private bool canMove = true;

    //private Player player;
    //[Header("Animator")]
    private Animator playerAnim;

    #region Unity Callbacks

    void Start()
    {
        //player = GetComponent<Player>();
        playerAnim = GetComponent<Player>().Anim;

        ErrorChecking();
    }

    void Update()
    {
        if (canMove)
        {
            if (isConstantMovement)
            {
                MovePlayer(dir);
                playerAnim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , dir);
            }
            else
            {
                float direction = Input.GetAxis(Constants.INPUT_HORIONTAL);

                if (direction != 0)
                {
                    //Vector3 velocityVector = transform.position + new Vector3(direction * MovementSpeed * Time.deltaTime , 0 , 0).normalized;
                    //SetVelocity(velocityVector);
                    MovePlayer(direction);
                }

                playerAnim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , direction);
            }
        }
        else
        {
            playerAnim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , 0);
        }
    }

    #endregion

    #region Private API

    private void ErrorChecking()
    {
        if (playerAnim == null) Debug.LogError("Anim Not Assigned");
        //if (player == null) Debug.LogError("Player Script not found");
    }

    private void MovePlayer( float speed )
    {
        transform.position = transform.position + new Vector3(speed * MovementSpeed * Time.deltaTime , 0 , 0);
        //Vector3 moveVector = transform.position + new Vector3(speed * MovementSpeed * Time.deltaTime , 0 , 0).normalized;
        //SetVelocity(moveVector);
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

    public void SetVelocity( float VelocityVector )
    {
        throw new System.NotImplementedException();
    }

    public void SetJumpInput( bool pressed )
    {
        throw new System.NotImplementedException();
    }

    public void SetIsGrounded( bool grounded )
    {
        throw new System.NotImplementedException();
    }

    #endregion

}
