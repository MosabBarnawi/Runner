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

    [Header("Animator")]
    public Animator Anim;

    #region Unity Callbacks

    void Start()
    {
        ErrorChecking();
    }

    void Update()
    {
        if (canMove)
        {
            if (isConstantMovement)
            {
                MovePlayer(dir);
                Anim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , dir);
            }
            else
            {
                float direction = Input.GetAxis(Constants.HORIONTAL);

                if (direction != 0)
                {
                    //Vector3 velocityVector = transform.position + new Vector3(direction * MovementSpeed * Time.deltaTime , 0 , 0).normalized;
                    //SetVelocity(velocityVector);
                    MovePlayer(direction);
                }

                Anim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , direction);
            }
        }
        else
        {
            Anim.SetFloat(Constants.ANIM_MOVEMENT_SPEED , 0);
        }
    }

    #endregion

    #region Private API

    private void ErrorChecking()
    {
        if (Anim == null) Debug.LogError("Anim Not Assigned");
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

    #endregion

}
