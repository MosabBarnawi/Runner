using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    [Header("Ray Casting")]
    public LayerMask layerMask;
    public float RaycastDistance = 2f;

    private bool hasTakenDamage = false;

    [Header("Debug Options")]
    public bool isDebugEnabled;

    private IMove imove;

    #region Unity Callbacks
    protected override void Start()
    {
        base.Start(); // Calls start from LivingEnitiy
        OnDeath += PlayerDeath;
        OnAddHealth += AddHealthFX;
        OnTakeHit += TakeHit;

        imove = GetComponent<IMove>();

        if (imove == null)
        {
            Debug.LogError("IMove Not Found");
        }
    }

    private void FixedUpdate()
    {
        if (isDebugEnabled)
        {
            Debug.DrawRay(transform.position , transform.TransformDirection(Vector3.right * RaycastDistance) , Color.red);
        }

        if (!isDead)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position , transform.TransformDirection(Vector3.right) , out hit , RaycastDistance , layerMask))
            {
                if (!hasTakenDamage)
                {
                    TakeDamage(1f);
                    imove.StopMovement();
                    hasTakenDamage = true;
                }
            }
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
        imove.StopMovement();
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
