using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Range(-1f , 1.0f)]
    public float dir;


    public bool isConstantMovement;
    public bool isAcceleration;

    public float MovementSpeed = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(rb == null)
        {
            Debug.LogError("RigidBody not Found");
        }
    }

    void Update()
    {
        if(isConstantMovement)
        {
            MovePlayer(dir);
        }
        else
        {
            if(Input.GetAxis(Constants.HORIONTAL) !=0 )
            {
                MovePlayer(Input.GetAxis(Constants.HORIONTAL));
            }
        }
    }

    private void MovePlayer(float direction)
    {
        float speed = direction * MovementSpeed * Time.deltaTime;

        if(isAcceleration)
        {
            rb.velocity += new Vector3(speed , 0 , 0);
        }
        else
        {
            rb.velocity = new Vector3(speed * 100 , 0 , 0);
        }
    }

    
}
