using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    public bool isConstantMovement;

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
            MovePlayer(1f);
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
        rb.velocity += new Vector3(speed , 0 , 0);
    }

    
}
