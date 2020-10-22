using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    [Header("Ray Casting")]
    public LayerMask layerMask;
    public float RaycastDistance = 2f;

    #region Unity Callbacks
    protected override void Start()
    {
        base.Start(); // Calls start from LivingEnitiy
        OnDeath += PlayerDeath;
        OnAddHealth += AddHealthFX;
        OnTakeHit += TakeHit;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position , transform.TransformDirection(Vector3.right) , out hit , RaycastDistance , layerMask))
            {
                TakeDamage(1f);
            }

            Debug.DrawRay(transform.position , transform.TransformDirection(Vector3.right * RaycastDistance) , Color.red);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1f);
        }
    }
    #endregion

    #region Private API


    private void PlayerDeath()
    {
        Debug.Log("Died");   
        // DISABLE MOVEMENT
    }

    private void AddHealthFX()
    {
        Debug.Log("Sparks");
    }
    public void TakeHit()
    {

    }
    #endregion
}
