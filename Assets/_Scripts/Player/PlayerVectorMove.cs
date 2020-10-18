using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVectorMove : MonoBehaviour
{
    [Range(-1f , 1.0f)]
    public float dir;

    public bool isConstantMovement;
    public float MovementSpeed = 10f;


    void Start()
    {

    }

    void Update()
    {
        if (isConstantMovement)
        {
            MovePlayer(dir);
        }
        else
        {
            if (Input.GetAxis(Constants.HORIONTAL) != 0)
            {
                float direction = Input.GetAxis(Constants.HORIONTAL);
                MovePlayer(direction);
            }
        }
    }

    private void MovePlayer( float speed )
    {
        transform.position = transform.position + new Vector3(speed * MovementSpeed * Time.deltaTime , 0 , 0);
    }
}
